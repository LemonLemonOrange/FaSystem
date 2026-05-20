# 實作計畫：供水狀態燈號定期記錄排程（water-signal-recorder）

## 概覽

依照設計文件，逐步建立 `FaWrWaterSignalSnapshot` 資料模型、`IFaWrSignalSnapshotService` 介面與實作、`FaWrSignalSnapshotSchedule` 排程封裝，最後修改 `FaDbContext` 與 `DISetup` 完成整合。測試專案需安裝 FsCheck 以執行屬性測試。

## 任務

- [x] 1. 建立 SQL 建表腳本與資料模型
  - [x] 1.1 建立 `DocSQL/Step2_Create_WaterSignalSnapshot.sql`
    - 依設計文件撰寫 `IF NOT EXISTS` 建表 DDL，包含所有欄位定義、`CHECK` 約束（`SignalLevel` 限制為 `綠燈`/`黃燈`/`橙燈`/`紅燈`）及三個索引（`BatchId`、`RecordTime`、`AreaName`）
    - _需求：4.1, 4.2, 4.3_

  - [x] 1.2 建立 `Models/FaWrWaterSignalSnapshot.cs`
    - 以 `[Table("FA_WR_WaterSignalSnapshot")]` 標記，定義 `Id`、`BatchId`、`AreaName`、`SignalLevel`、`SupplyStatus`、`StatusDescription`、`RecordTime` 屬性，加上對應的 `[Key]`、`[Required]`、`[StringLength]`、`[Column]` 標記，命名空間為 `fa_api.Models`
    - _需求：4.1_

- [x] 2. 更新 FaDbContext
  - [x] 2.1 在 `Data/FaDbContext.cs` 新增 `DbSet` 與 `OnModelCreating` 設定
    - 新增 `public virtual DbSet<FaWrWaterSignalSnapshot> FaWrWaterSignalSnapshot { get; set; }`
    - 在 `OnModelCreating` 中新增 `FaWrWaterSignalSnapshot` 的 `HasIndex`（`BatchId`、`RecordTime`、`AreaName`）與 `HasDefaultValueSql("(getdate())")` 設定
    - _需求：4.4_

- [x] 3. 實作服務層
  - [x] 3.1 建立 `Services/FaWrSignalSnapshot/IFaWrSignalSnapshotService.cs`
    - 定義 `IFaWrSignalSnapshotService` 介面，包含 `Task RecordWaterSupplySignalsAsync()` 方法，命名空間為 `fa_api.Services.FaWrSignalSnapshot`
    - _需求：1.1_

  - [x] 3.2 建立 `Services/FaWrSignalSnapshot/FaWrSignalSnapshotService.cs`
    - 實作 `FaWrSignalSnapshotService : IFaWrSignalSnapshotService`
    - 建構子注入 `IWraGovService`、`FaDbContext`、`ILogger<FaWrSignalSnapshotService>`
    - 實作 `internal static string MapToSignalLevel(int supplyStatus)`：`1→綠燈`、`2→黃燈`、`3→橙燈`、`4→紅燈`、其他→`null`
    - 實作 `RecordWaterSupplySignalsAsync()`：產生 `BatchId`（`Guid.NewGuid().ToString()`）、呼叫 `GetWaterSupplyConditionAsync()`、逐筆轉換燈號（無效值以 `LogWarning` 略過）、`AddRange` 後單次 `SaveChangesAsync()`、完成後 `LogInformation` 記錄 `BatchId`/筆數/執行時間
    - API 例外與 DB 例外均以 `LogError` 記錄後重新拋出
    - _需求：1.1, 1.2, 1.3, 1.4, 1.5, 1.6, 1.7, 2.1, 2.2, 2.3, 2.4, 2.5_

  - [ ] 3.3 為 `MapToSignalLevel` 撰寫屬性測試（Property 1）
    - **Property 1：燈號對應完整性**
    - 使用 FsCheck 對 `{1, 2, 3, 4}` 中的任意值驗證回傳值為 `綠燈`/`黃燈`/`橙燈`/`紅燈` 之一且非 null，最少 100 次迭代
    - **Validates: Requirements 1.2, 1.3, 1.4, 1.5**

  - [ ] 3.4 為 `MapToSignalLevel` 撰寫屬性測試（Property 2）
    - **Property 2：無效狀態碼被略過**
    - 使用 FsCheck 對不在 `{1, 2, 3, 4}` 範圍內的任意整數驗證回傳值為 `null`，最少 100 次迭代
    - **Validates: Requirements 1.6**

  - [ ] 3.5 為 `RecordWaterSupplySignalsAsync` 撰寫屬性測試（Property 3）
    - **Property 3：快照欄位完整性**
    - 使用 FsCheck 產生包含一或多筆有效供水狀況（`SupplyStatus` 1-4）的輸入清單，mock `IWraGovService` 回傳該清單，捕捉 `AddRange` 的輸入，驗證每筆快照的 `AreaName`、`SignalLevel`、`BatchId` 均非空且 `SupplyStatus` 與原始值一致，最少 100 次迭代
    - **Validates: Requirements 1.7**

  - [ ] 3.6 為 `RecordWaterSupplySignalsAsync` 撰寫屬性測試（Property 4）
    - **Property 4：同批次 BatchId 一致性**
    - 使用 FsCheck 產生包含多筆有效供水狀況的輸入清單，驗證單次執行所產生的所有快照共用同一個 `BatchId`（有效 GUID 格式），且連續兩次執行的 `BatchId` 不同，最少 100 次迭代
    - **Validates: Requirements 2.1**

  - [ ] 3.7 為 `FaWrSignalSnapshotService` 撰寫單元測試
    - 驗證 `GetWaterSupplyConditionAsync()` 被呼叫一次
    - 驗證 `SaveChangesAsync()` 被呼叫一次（無論輸入筆數）
    - 驗證 API 拋出例外時 `SaveChangesAsync` 不被呼叫且例外向上傳遞
    - 驗證 `SaveChangesAsync` 拋出例外時例外向上傳遞
    - 驗證 `MapToSignalLevel` 邊界值（0、5、-1、`int.MinValue`、`int.MaxValue`）回傳 `null`
    - _需求：1.1, 2.2, 2.3, 2.4_

- [x] 4. 檢查點 — 確認所有測試通過
  - 確認所有測試通過，如有問題請向使用者提問。

- [x] 5. 建立排程封裝
  - [x] 5.1 建立 `Schedule/FaWrSignalSnapshotSchedule.cs`
    - 定義 `const string JobId = "water-signal-recorder"` 與 `const string CronExpression = "*/10 * * * *"`
    - 建構子注入 `IFaWrSignalSnapshotService`
    - 實作 `RegisterRecurringJob()`：呼叫 `RecurringJob.AddOrUpdate<IFaWrSignalSnapshotService>` 以 `JobId` 和 `CronExpression` 註冊
    - 實作 `RemoveRecurringJob()`：呼叫 `RecurringJob.RemoveIfExists(JobId)`
    - 實作 `EnqueueNow()`：呼叫 `BackgroundJob.Enqueue<IFaWrSignalSnapshotService>` 以 Fire-and-forget 方式排入一次執行
    - 命名空間為 `fa_api.Schedule`，對應 `MailSchedule.cs` 模式
    - _需求：3.1, 3.2, 3.3, 3.5_

- [ ] 6. 更新 DI 註冊
  - [ ] 6.1 修改 `ServiceExtensions/DISetup.cs`
    - 在 `AddApplicationServices()` 中新增 `services.AddScoped<IFaWrSignalSnapshotService, FaWrSignalSnapshotService>()`
    - 新增 `services.AddScoped<FaWrSignalSnapshotSchedule>()`
    - 在 `AddApplicationServices()` 回傳前，從 `services.BuildServiceProvider()` 解析 `FaWrSignalSnapshotSchedule` 並呼叫 `RegisterRecurringJob()`
    - 新增對應的 `using` 陳述式（`fa_api.Services.FaWrSignalSnapshot`、`fa_api.Schedule`）
    - _需求：5.1, 5.2, 5.3, 5.4_

  - [ ] 6.2 撰寫 DI 整合測試
    - 驗證 DI 容器能正確解析 `IFaWrSignalSnapshotService` 與 `FaWrSignalSnapshotSchedule`
    - 驗證 Hangfire 排程以 `*/10 * * * *` 和 `water-signal-recorder` 完成註冊（使用 Hangfire 記憶體儲存）
    - _需求：3.1, 5.1, 5.2_

- [ ] 7. 最終檢查點 — 確認所有測試通過
  - 確認所有測試通過，執行 `dotnet build` 確認無編譯錯誤，如有問題請向使用者提問。

## 備註

- 標記 `*` 的子任務為選用，可跳過以加速 MVP 開發
- 每個任務均對應具體需求，確保可追溯性
- 屬性測試需在測試專案安裝 FsCheck（`Install-Package FsCheck -Version <latest>`）
- `MapToSignalLevel` 設計為 `internal static`，可直接在測試中呼叫，無需完整 Service 實例
- `DISetup.cs` 中呼叫 `RegisterRecurringJob()` 的方式需注意 `BuildServiceProvider()` 在 `AddApplicationServices()` 內的使用，若專案已有 `IApplicationBuilder` 可用的 startup hook，可改為在 `Startup.Configure` 中解析並呼叫

## Task Dependency Graph

```json
{
  "waves": [
    { "id": 0, "tasks": ["1.1", "1.2"] },
    { "id": 1, "tasks": ["2.1"] },
    { "id": 2, "tasks": ["3.1"] },
    { "id": 3, "tasks": ["3.2"] },
    { "id": 4, "tasks": ["3.3", "3.4", "3.5", "3.6", "3.7", "5.1"] },
    { "id": 5, "tasks": ["6.1"] },
    { "id": 6, "tasks": ["6.2"] }
  ]
}
```
