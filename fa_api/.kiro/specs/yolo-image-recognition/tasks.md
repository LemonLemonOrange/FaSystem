# Implementation Plan: YOLO Image Recognition

## Overview

依照設計文件，在現有 `fa_api` ASP.NET Core 3.1 專案中新增 YOLO 影像辨識功能。
實作順序：DTO → Settings → Service 介面與實作 → Controller → DI 註冊 → 設定檔 → 測試專案。
測試框架採用 xUnit + FsCheck.Xunit + Moq，與現有 .NET Core 3.1 相容。

## Tasks

- [x] 1. 建立 DTO 與設定模型
  - [x] 1.1 建立 `Dtos/Yolo/YoloDto.cs`
    - 在命名空間 `fa_api.Dtos.Yolo` 中定義 `DetectionResultDto`（含 `Label`、`Confidence`、`X`、`Y`、`Width`、`Height` 欄位）
    - 在同一檔案中定義 `YoloResponseDto`（含 `Detections`、`ElapsedMs` 欄位）
    - 每個欄位加上 `/// <summary>` XML 文件註解以產生 Swagger 說明
    - _Requirements: 3.1, 3.2, 6.3_

  - [x] 1.2 建立 `Services/Yolo/YoloSettings.cs`
    - 在命名空間 `fa_api.Services.Yolo` 中定義 `YoloSettings` 類別
    - 包含 `PythonPath`（string）、`ScriptPath`（string）、`TimeoutSeconds`（int，預設值 30）三個屬性
    - 每個屬性加上 `/// <summary>` XML 文件註解
    - _Requirements: 4.1_

- [x] 2. 建立 IYoloService 介面
  - [x] 2.1 建立 `Services/Yolo/IYoloService.cs`
    - 在命名空間 `fa_api.Services.Yolo` 中定義 `IYoloService` 介面
    - 宣告 `Task<YoloResponseDto> DetectAsync(byte[] imageBytes)` 方法
    - 加上完整 XML 文件註解，包含 `<summary>`、`<param>`、`<returns>`、`<exception>` 標記
    - _Requirements: 2.1, 2.2, 2.3, 2.4_

- [x] 3. 實作 YoloService
  - [x] 3.1 建立 `Services/Yolo/YoloService.cs` 骨架與設定驗證
    - 在命名空間 `fa_api.Services.Yolo` 中建立 `YoloService : IYoloService`
    - 建構子注入 `ILogger<YoloService>` 與 `IOptions<YoloSettings>`
    - 在 `DetectAsync` 開頭驗證 `PythonPath` 與 `ScriptPath` 不為空，否則拋出 `InvalidOperationException("YoloSettings 設定不完整，請確認 PythonPath 與 ScriptPath")`
    - _Requirements: 4.2_

  - [x] 3.2 實作暫存檔案管理與 Process 呼叫
    - 使用 `Path.GetTempFileName()` 建立暫存檔案路徑（加上適當副檔名）
    - 以 `File.WriteAllBytesAsync` 將 `imageBytes` 寫入暫存檔案
    - 以 `_logger.LogDebug` 記錄 Python 路徑、腳本路徑、暫存影像路徑
    - 建立 `ProcessStartInfo`，設定 `FileName`、`Arguments`、`RedirectStandardOutput = true`、`RedirectStandardError = true`、`UseShellExecute = false`、`CreateNoWindow = true`
    - 在 `finally` 區塊中呼叫私有方法 `DeleteTempFile(path)` 刪除暫存檔案（刪除失敗時 `LogWarning`，不拋出例外）
    - _Requirements: 2.1, 2.5_

  - [x] 3.3 實作逾時控制與 Process 執行結果處理
    - 使用 `CancellationTokenSource` 設定 `TimeoutSeconds` 逾時
    - 呼叫 `process.WaitForExitAsync(cts.Token)`；若 token 取消，呼叫 `process.Kill(entireProcessTree: true)` 後拋出 `TimeoutException("YOLO 腳本執行逾時")`
    - 若 `ExitCode != 0`，讀取 stderr 並拋出 `InvalidOperationException($"YOLO 腳本執行失敗：{stderr}")`
    - _Requirements: 2.2, 2.4_

  - [x] 3.4 實作 stdout JSON 解析、confidence 過濾與 LogInformation
    - 讀取 stdout 字串，呼叫私有方法 `ParseDetections(string json)` 解析為 `List<DetectionResultDto>`
    - `ParseDetections` 使用 `Newtonsoft.Json` 的 `JArray.Parse` 解析，過濾 `confidence < 0.0m || confidence > 1.0m` 的項目
    - 計算 `ElapsedMs`，建立並回傳 `YoloResponseDto`
    - 以 `_logger.LogInformation` 記錄偵測到的物件數量與執行時間（毫秒）
    - _Requirements: 2.3, 3.1, 3.3, 3.4, 5.3, 5.4_

  - [ ]* 3.5 撰寫 Property 3 屬性測試：偵測結果解析完整性（Round-Trip）
    - **Property 3: 偵測結果解析完整性（解析 Round-Trip）**
    - 使用 FsCheck 生成任意 `label`（非空字串）、`confidence`（0.0~1.0 decimal）、`x`/`y`/`width`/`height`（整數）組合的 JSON 陣列
    - 驗證 `ParseDetections` 輸出的每筆 `DetectionResultDto` 欄位與輸入 JSON 完全對應，無欄位遺漏或型別錯誤
    - 標記：`// Feature: yolo-image-recognition, Property 3: 偵測結果解析完整性`
    - **Validates: Requirements 2.3, 3.1**

  - [ ]* 3.6 撰寫 Property 4 屬性測試：Confidence 過濾不變式
    - **Property 4: Confidence 過濾不變式**
    - 使用 FsCheck 生成包含任意 confidence 值（含負數與 >1.0）的 JSON 陣列
    - 驗證 `ParseDetections` 輸出清單中所有項目的 confidence 均在 [0.0, 1.0] 範圍內
    - 標記：`// Feature: yolo-image-recognition, Property 4: Confidence 過濾不變式`
    - **Validates: Requirements 3.3**

  - [ ]* 3.7 撰寫 Property 5 屬性測試：暫存檔案清理不變式
    - **Property 5: 暫存檔案清理不變式**
    - 使用 FsCheck 模擬成功（exit 0）、失敗（exit ≠ 0）、逾時三種執行結果
    - 驗證無論哪種結果，`DetectAsync` 回傳或拋出後，暫存檔案均已被刪除
    - 標記：`// Feature: yolo-image-recognition, Property 5: 暫存檔案清理不變式`
    - **Validates: Requirements 2.5**

  - [ ]* 3.8 撰寫 YoloService 單元測試
    - 測試情境：設定不完整時拋出 `InvalidOperationException`（Requirements 4.2）
    - 測試情境：ExitCode ≠ 0 時拋出 `InvalidOperationException` 並包含 stderr 訊息（Requirements 2.4）
    - 測試情境：Python 腳本逾時時拋出 `TimeoutException`（Requirements 2.2）
    - 測試情境：空偵測陣列時回傳 `detections` 為空陣列的 `YoloResponseDto`（Requirements 3.4）
    - 測試情境：成功辨識後呼叫 `LogInformation` 記錄偵測數量與執行時間（Requirements 5.3）
    - 測試情境：呼叫前呼叫 `LogDebug` 記錄路徑資訊（Requirements 5.4）
    - _Requirements: 2.2, 2.4, 3.4, 4.2, 5.3, 5.4_

- [x] 4. Checkpoint — 確認 Service 層測試全數通過
  - 確認所有 Service 層測試通過，如有問題請向使用者確認。

- [x] 5. 建立 YoloController
  - [x] 5.1 建立 `Controllers/YoloController.cs`
    - 在命名空間 `fa_api.Controllers` 中建立 `YoloController : ControllerBase`
    - 加上 `[ApiController]`、`[Route("api/[controller]")]`、`[ApiExplorerSettings(GroupName = "Yolo")]`、`[Produces("application/json")]` 標記
    - 建構子注入 `ILogger<YoloController>` 與 `IYoloService`（不注入 `IMemoryCache`）
    - _Requirements: 1.1, 6.2_

  - [x] 5.2 實作 `POST /api/yolo/detect` 端點與輸入驗證
    - 實作 `[HttpPost("detect")] public async Task<ActionResult<YoloResponseDto>> Detect([FromForm] IFormFile image)` 方法
    - 依序驗證：`image == null || image.Length == 0` → HTTP 400 `{ "message": "影像檔案不得為空" }`
    - MIME 類型不在 `{ "image/jpeg", "image/png", "image/bmp" }` → HTTP 400 `{ "message": "不支援的檔案格式，僅接受 JPEG、PNG、BMP" }`
    - `image.Length > 10 * 1024 * 1024` → HTTP 400 `{ "message": "檔案大小超過限制（最大 10 MB）" }`
    - _Requirements: 1.1, 1.2, 1.3, 1.5_

  - [x] 5.3 實作 Service 呼叫與例外處理
    - 讀取 `image` 為 `byte[]`，呼叫 `_yoloService.DetectAsync(imageBytes)`
    - 成功時回傳 HTTP 200 + `YoloResponseDto`
    - 捕捉 `TimeoutException` → HTTP 504 `{ "message": "影像辨識逾時，請稍後再試" }`
    - 捕捉其他 `Exception` → `_logger.LogError(ex, ...)` + HTTP 500 `{ "message": "影像辨識失敗", "error": "{ex.Message}" }`
    - _Requirements: 1.4, 5.1, 5.2_

  - [ ]* 5.4 撰寫 Property 1 屬性測試：MIME 類型驗證
    - **Property 1: MIME 類型驗證拒絕非允許格式**
    - 使用 FsCheck 生成任意 MIME 類型字串（含允許清單內的三種值）
    - 驗證只有 `"image/jpeg"`、`"image/png"`、`"image/bmp"` 通過驗證，其餘均回傳 HTTP 400
    - 標記：`// Feature: yolo-image-recognition, Property 1: MIME 類型驗證拒絕非允許格式`
    - **Validates: Requirements 1.2**

  - [ ]* 5.5 撰寫 Property 2 屬性測試：檔案大小驗證
    - **Property 2: 檔案大小驗證拒絕超過上限的檔案**
    - 使用 FsCheck 生成 0 ~ 20 MB 範圍的任意檔案大小整數
    - 驗證超過 10 MB 的請求回傳 HTTP 400，非零且未超過 10 MB 的請求通過大小驗證
    - 標記：`// Feature: yolo-image-recognition, Property 2: 檔案大小驗證拒絕超過上限的檔案`
    - **Validates: Requirements 1.3**

  - [ ]* 5.6 撰寫 YoloController 單元測試
    - 使用 Moq mock `IYoloService`
    - 測試情境：空檔案上傳 → HTTP 400 + 正確訊息（Requirements 1.5）
    - 測試情境：不支援 MIME 類型 → HTTP 400 + 正確訊息（Requirements 1.2）
    - 測試情境：超過 10 MB → HTTP 400 + 正確訊息（Requirements 1.3）
    - 測試情境：成功辨識 → HTTP 200 + `YoloResponseDto` 結構（Requirements 1.4）
    - 測試情境：`TimeoutException` → HTTP 504 + 正確訊息（Requirements 5.2）
    - 測試情境：一般例外 → HTTP 500 + `LogError` 呼叫（Requirements 5.1）
    - 測試情境：Swagger 屬性驗證（Requirements 6.2）
    - _Requirements: 1.2, 1.3, 1.4, 1.5, 5.1, 5.2, 6.2_

- [x] 6. 更新 DI 註冊與設定檔
  - [x] 6.1 更新 `ServiceExtensions/DISetup.cs`
    - 在 `AddApplicationServices()` 方法的 YOLO 服務區段中新增：
      `services.Configure<YoloSettings>(configuration.GetSection("YoloSettings"));`
      `services.AddScoped<IYoloService, YoloService>();`
    - 加上 `using fa_api.Services.Yolo;` 引用
    - _Requirements: 6.1_

  - [x] 6.2 更新 `appsettings.json`
    - 在根層級新增 `YoloSettings` 區段，包含 `PythonPath`、`ScriptPath`、`TimeoutSeconds`（預設 30）三個鍵值
    - _Requirements: 4.1, 4.3_

  - [ ]* 6.3 撰寫 DI 與設定整合 Smoke Tests
    - 測試 `IYoloService` 能從 DI 容器正確解析為 `YoloService`（Requirements 6.1）
    - 測試 `YoloSettings` 能從 `appsettings.json` 的 `YoloSettings` 區段正確綁定（Requirements 4.1）
    - _Requirements: 4.1, 6.1_

- [x] 7. Final Checkpoint — 確認所有測試通過
  - 確認所有測試通過，如有問題請向使用者確認。

## Notes

- 標記 `*` 的子任務為選填，可跳過以加速 MVP 開發
- 每個任務均對應具體需求條款，確保可追溯性
- Property 測試使用 FsCheck.Xunit，每個屬性最少執行 100 次迭代
- 單元測試使用 xUnit + Moq，測試專案建議命名為 `fa_api.Tests`
- `YoloController` 不注入 `IMemoryCache`（影像辨識結果不適合快取）
- 逾時語意為嚴格語意：token 取消即視為逾時，即使腳本恰好在逾時當下完成亦同

## Task Dependency Graph

```json
{
  "waves": [
    { "id": 0, "tasks": ["1.1", "1.2"] },
    { "id": 1, "tasks": ["2.1"] },
    { "id": 2, "tasks": ["3.1"] },
    { "id": 3, "tasks": ["3.2"] },
    { "id": 4, "tasks": ["3.3"] },
    { "id": 5, "tasks": ["3.4"] },
    { "id": 6, "tasks": ["3.5", "3.6", "3.7", "3.8"] },
    { "id": 7, "tasks": ["5.1"] },
    { "id": 8, "tasks": ["5.2"] },
    { "id": 9, "tasks": ["5.3"] },
    { "id": 10, "tasks": ["5.4", "5.5", "5.6"] },
    { "id": 11, "tasks": ["6.1", "6.2"] },
    { "id": 12, "tasks": ["6.3"] }
  ]
}
```
