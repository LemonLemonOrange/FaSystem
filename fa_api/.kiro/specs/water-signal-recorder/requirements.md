# 需求文件：供水狀態燈號定期記錄排程（water-signal-recorder）

## 簡介

本功能透過 Hangfire 週期性排程，定期從水利署（WRA）API 抓取各地區供水狀態，將供水狀態轉換為燈號等級後，以批次方式寫入 SQL Server 資料庫，以供歷史趨勢分析與前端查詢使用。

排程每 10 分鐘執行一次，與水利署 API 資料更新頻率一致。每次執行時，系統呼叫 WRA 供水狀況 API，將結果轉換為燈號等級後，批次寫入新資料表 `FA_WR_WaterSignalSnapshot`。

---

## 詞彙表

- **FaWrSignalSnapshotSchedule**：Hangfire 排程封裝類別，負責註冊與執行供水燈號記錄週期任務
- **IFaWrSignalSnapshotService**：供水燈號記錄服務介面，定義抓取與寫入邏輯
- **FaWrSignalSnapshotService**：`IFaWrSignalSnapshotService` 的實作類別
- **FA_WR_WaterSignalSnapshot**：新增資料表，儲存每次排程執行的供水燈號快照
- **FaDbContext**：現有 EF Core DbContext，需新增 `FA_WR_WaterSignalSnapshot` 的 `DbSet`
- **IWraGovService**：現有水利署服務介面，提供供水即時資料
- **SignalLevel**：燈號等級，包含：`綠燈`、`黃燈`、`橙燈`、`紅燈`
- **BatchId**：同一次排程執行的批次識別碼（GUID），用於關聯同批次的所有燈號紀錄
- **Cron 表達式**：Hangfire 使用的排程時間格式，`*/10 * * * *` 代表每 10 分鐘執行一次

---

## 需求

### 需求 1：供水狀態燈號判斷與記錄

**使用者故事：** 身為系統管理員，我希望系統能定期記錄各地區的供水狀態燈號，以便追蹤供水限制歷史與趨勢。

#### 驗收標準

1. WHEN `FaWrSignalSnapshotService` 執行供水燈號記錄，THE `IWraGovService` SHALL 呼叫 `GetWaterSupplyConditionAsync()` 取得所有地區供水狀況
2. WHEN 地區的 `SupplyStatus` 等於 1（正常供水），THE `FaWrSignalSnapshotService` SHALL 將該地區燈號判定為 `綠燈`
3. WHEN 地區的 `SupplyStatus` 等於 2（減壓供水），THE `FaWrSignalSnapshotService` SHALL 將該地區燈號判定為 `黃燈`
4. WHEN 地區的 `SupplyStatus` 等於 3（限量供水），THE `FaWrSignalSnapshotService` SHALL 將該地區燈號判定為 `橙燈`
5. WHEN 地區的 `SupplyStatus` 等於 4（停止供水），THE `FaWrSignalSnapshotService` SHALL 將該地區燈號判定為 `紅燈`
6. IF 地區的 `SupplyStatus` 不在 1 至 4 的範圍內，THEN THE `FaWrSignalSnapshotService` SHALL 略過該筆資料，不寫入資料庫
7. WHEN 燈號判定完成，THE `FaWrSignalSnapshotService` SHALL 將 `AreaName`、`SignalLevel`、`SupplyStatus`、`BatchId`、`RecordTime` 寫入 `FA_WR_WaterSignalSnapshot`

---

### 需求 2：批次執行與資料庫寫入

**使用者故事：** 身為系統管理員，我希望每次排程執行時，所有地區的供水燈號能以同一批次識別碼寫入資料庫，以便查詢同一時間點的完整供水快照。

#### 驗收標準

1. WHEN `FaWrSignalSnapshotService` 開始執行，THE `FaWrSignalSnapshotService` SHALL 產生一個新的 `BatchId`（GUID），作為本次執行所有紀錄的共同識別碼
2. WHEN 所有地區燈號均已判定完成，THE `FaWrSignalSnapshotService` SHALL 以單一 `SaveChangesAsync()` 呼叫將所有紀錄批次寫入 `FA_WR_WaterSignalSnapshot`
3. IF `GetWaterSupplyConditionAsync()` 拋出例外，THEN THE `FaWrSignalSnapshotService` SHALL 記錄錯誤日誌，並讓 Hangfire 依其重試策略處理後續重試，不寫入任何資料
4. IF `SaveChangesAsync()` 拋出例外，THEN THE `FaWrSignalSnapshotService` SHALL 記錄錯誤日誌，並讓 Hangfire 依其重試策略處理後續重試
5. THE `FaWrSignalSnapshotService` SHALL 在每次執行完成後，於日誌中記錄本次 `BatchId`、寫入筆數及總執行時間

---

### 需求 3：Hangfire 週期排程註冊

**使用者故事：** 身為系統管理員，我希望供水燈號記錄排程能在應用程式啟動時自動註冊，並每 10 分鐘執行一次，無需手動觸發。

#### 驗收標準

1. WHEN 應用程式啟動，THE `FaWrSignalSnapshotSchedule` SHALL 以 Cron 表達式 `*/10 * * * *` 向 Hangfire 註冊名稱為 `water-signal-recorder` 的週期性任務
2. THE `FaWrSignalSnapshotSchedule` SHALL 提供 `RegisterRecurringJob()` 方法，供 `DISetup` 在應用程式啟動時呼叫
3. THE `FaWrSignalSnapshotSchedule` SHALL 提供 `RemoveRecurringJob()` 方法，以便在需要時移除排程
4. WHILE 應用程式執行中，THE Hangfire 排程器 SHALL 依 `*/10 * * * *` 週期自動觸發 `FaWrSignalSnapshotService.RecordWaterSupplySignalsAsync()`
5. WHERE 需要手動觸發，THE `FaWrSignalSnapshotSchedule` SHALL 提供 `EnqueueNow()` 方法，以 Fire-and-forget 方式立即排入一次執行

---

### 需求 4：資料表結構設計

**使用者故事：** 身為開發人員，我希望有一個專門的資料表儲存供水燈號快照，結構清晰且具備適當索引，以支援歷史查詢。

#### 驗收標準

1. THE `FA_WR_WaterSignalSnapshot` 資料表 SHALL 包含以下欄位：
   - `Id`（INT IDENTITY，主鍵）
   - `BatchId`（NVARCHAR(36) NOT NULL）
   - `AreaName`（NVARCHAR(100) NOT NULL，地區名稱）
   - `SignalLevel`（NVARCHAR(10) NOT NULL，燈號等級）
   - `SupplyStatus`（INT NOT NULL，原始供水狀態代碼）
   - `StatusDescription`（NVARCHAR(50) NULL，供水狀態說明）
   - `RecordTime`（DATETIME NOT NULL，預設 GETDATE()）
2. THE `FA_WR_WaterSignalSnapshot` 資料表 SHALL 在 `BatchId`、`RecordTime`、`AreaName` 欄位上建立索引，以支援常見查詢模式
3. THE `FA_WR_WaterSignalSnapshot` 資料表 SHALL 對 `SignalLevel` 欄位加上 CHECK 約束，限制值為 `綠燈`、`黃燈`、`橙燈`、`紅燈`
4. THE `FaDbContext` SHALL 新增 `DbSet<FaWrWaterSignalSnapshot>` 屬性，並在 `OnModelCreating` 中設定對應的索引與預設值

---

### 需求 5：DI 服務註冊

**使用者故事：** 身為開發人員，我希望新增的服務與排程類別能依照專案慣例正確註冊至 DI 容器，確保依賴注入正常運作。

#### 驗收標準

1. THE `DISetup.AddApplicationServices()` SHALL 以 `AddScoped<IFaWrSignalSnapshotService, FaWrSignalSnapshotService>()` 註冊供水燈號記錄服務
2. THE `DISetup.AddApplicationServices()` SHALL 以 `AddScoped<FaWrSignalSnapshotSchedule>()` 註冊排程封裝類別
3. WHEN `AddApplicationServices()` 執行完成，THE `DISetup` SHALL 從 DI 容器解析 `FaWrSignalSnapshotSchedule` 並呼叫 `RegisterRecurringJob()` 以完成 Hangfire 週期任務的初始化
4. THE `FaWrSignalSnapshotService` SHALL 透過建構子注入 `IWraGovService`、`FaDbContext`、`ILogger<FaWrSignalSnapshotService>`
