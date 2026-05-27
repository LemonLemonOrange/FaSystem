# Requirements Document

## Introduction

本功能在現有的 `fa_api` ASP.NET Core 3.1 Web API 專案中，新增 YOLO 影像辨識能力。
系統透過 `Process.Start` 呼叫本機 Python 腳本，由 Python 腳本執行 YOLO 模型進行物件偵測，
辨識結果以 JSON 格式回傳給 .NET API，再包裝成標準 DTO 回應給前端。

架構遵循專案現有慣例：`Controllers` → `Services/Yolo` → `Dtos/Yolo`，
並在 `ServiceExtensions/DISetup.cs` 集中完成 DI 註冊。

**術語定義：**

- **YoloController**：處理影像辨識 HTTP 請求的 API 控制器（`Controllers/YoloController.cs`）
- **IYoloService**：YOLO 影像辨識服務介面（`Services/Yolo/IYoloService.cs`）
- **YoloService**：透過 `Process.Start` 呼叫 Python 腳本的服務實作（`Services/Yolo/YoloService.cs`）
- **Python_Script**：執行 YOLO 模型推論的 Python 腳本（路徑由 `appsettings.json` 的 `YoloSettings:ScriptPath` 設定）
- **YoloSettings**：從 `appsettings.json` 注入的設定模型，包含 `ScriptPath`、`PythonPath`、`TimeoutSeconds`
- **DetectionResultDto**：單一物件偵測結果 DTO（`Dtos/Yolo/YoloDto.cs`）
- **YoloResponseDto**：完整辨識回應 DTO，包含所有偵測結果清單與執行時間
- **Confidence**：YOLO 模型對偵測結果的信心分數（0.0 ~ 1.0 之間的浮點數）
- **BoundingBox**：偵測到的物件在影像中的邊界框座標（x, y, width, height，以像素為單位）

## Requirements

### Requirement 1

**User Story:** As a frontend developer, I want to upload an image via API and receive YOLO detection results, so that I can analyze field images in the flood monitoring system.

#### Acceptance Criteria

1. WHEN 用戶端以 `multipart/form-data` 格式 POST 影像檔案至 `/api/yolo/detect`，THE YoloController SHALL 接收該請求並將影像傳遞給 YoloService 進行辨識。
2. THE YoloController SHALL 僅接受 MIME 類型為 `image/jpeg`、`image/png`、`image/bmp` 的檔案；IF 上傳的檔案 MIME 類型不在允許清單內，THEN THE YoloController SHALL 回傳 HTTP 400 及錯誤訊息 `{ "message": "不支援的檔案格式，僅接受 JPEG、PNG、BMP" }`。
3. THE YoloController SHALL 限制上傳檔案大小上限為 10 MB；IF 上傳的檔案超過 10 MB，THEN THE YoloController SHALL 回傳 HTTP 400 及錯誤訊息 `{ "message": "檔案大小超過限制（最大 10 MB）" }`。
4. WHEN 辨識成功，THE YoloController SHALL 回傳 HTTP 200 及 `YoloResponseDto`，其中包含所有偵測到的物件清單與本次辨識的執行時間（毫秒）。
5. IF 上傳的影像檔案為空（長度為 0），THEN THE YoloController SHALL 回傳 HTTP 400 及錯誤訊息 `{ "message": "影像檔案不得為空" }`。

### Requirement 2

**User Story:** As a system integrator, I want the .NET service to reliably invoke the Python YOLO script and receive structured results, so that AI inference capability can be integrated into the existing API architecture.

#### Acceptance Criteria

1. WHEN YoloService 收到影像位元組，THE YoloService SHALL 將影像儲存為暫存檔案，並以 `Process.Start` 呼叫 `YoloSettings:PythonPath` 所指定的 Python 執行檔，傳入 `YoloSettings:ScriptPath` 腳本路徑與暫存影像路徑作為命令列引數。
2. THE YoloService SHALL 在 `YoloSettings:TimeoutSeconds`（預設 30 秒）內等待 Python 腳本執行完畢；IF Python 腳本執行時間達到 `YoloSettings:TimeoutSeconds`，THEN THE YoloService SHALL 優先終止該 Process 並拋出 `TimeoutException`（訊息為 `"YOLO 腳本執行逾時"`），即使腳本恰好在逾時當下完成執行亦同。
3. WHEN Python 腳本在逾時期限內執行完畢且 ExitCode 為 0，THE YoloService SHALL 從標準輸出（stdout）讀取 JSON 字串，並解析為 `DetectionResultDto` 清單。
4. IF Python 腳本在逾時期限內執行完畢且 ExitCode 不為 0，THEN THE YoloService SHALL 從標準錯誤（stderr）讀取錯誤訊息，並拋出 `InvalidOperationException`，訊息格式為 `"YOLO 腳本執行失敗：{stderr 內容}"`。
5. THE YoloService SHALL 在 Python 腳本執行完畢後（無論成功或失敗），刪除步驟 1 所建立的暫存影像檔案。

### Requirement 3

**User Story:** As a frontend developer, I want detection results to include object class, confidence score, and bounding box coordinates, so that I can overlay detection results on maps or images.

#### Acceptance Criteria

1. THE YoloService SHALL 將 Python 腳本輸出的每筆偵測結果解析為 `DetectionResultDto`，包含以下欄位：`label`（物件類別名稱，string）、`confidence`（信心分數，decimal，範圍 0.0 ~ 1.0）、`x`（邊界框左上角 X 座標，integer）、`y`（邊界框左上角 Y 座標，integer）、`width`（邊界框寬度，integer）、`height`（邊界框高度，integer）。
2. THE YoloResponseDto SHALL 包含 `detections`（`DetectionResultDto` 清單）與 `elapsedMs`（本次辨識執行時間，integer，單位毫秒）兩個欄位。
3. WHEN Python 腳本輸出的 JSON 中某筆偵測結果的 `confidence` 欄位值小於 0.0 或大於 1.0，THE YoloService SHALL 略過該筆結果，不納入回傳清單；`confidence` 值為 0.0 的結果視為有效，應納入回傳清單。
4. WHEN Python 腳本輸出的 JSON 陣列為空（無偵測到任何物件），THE YoloService SHALL 回傳 `detections` 為空陣列的 `YoloResponseDto`，而非錯誤。

### Requirement 4

**User Story:** As a system administrator, I want Python path, script path, and timeout settings to be centrally managed in `appsettings.json`, so that I can switch configurations across environments without recompiling.

#### Acceptance Criteria

1. THE YoloSettings SHALL 從 `appsettings.json` 的 `YoloSettings` 區段讀取以下設定：`PythonPath`（Python 執行檔路徑，string）、`ScriptPath`（YOLO Python 腳本路徑，string）、`TimeoutSeconds`（腳本執行逾時秒數，integer，預設值 30）。
2. IF 應用程式啟動時 `YoloSettings:PythonPath` 或 `YoloSettings:ScriptPath` 為空字串，THEN THE YoloService SHALL 在首次呼叫時拋出 `InvalidOperationException`，訊息為 `"YoloSettings 設定不完整，請確認 PythonPath 與 ScriptPath"`。
3. WHERE `appsettings.Development.json` 存在 `YoloSettings` 區段，THE YoloSettings SHALL 以開發環境設定覆蓋 `appsettings.json` 的對應值。

### Requirement 5

**User Story:** As an operations engineer, I want all recognition failures to have clear error responses and log records, so that I can quickly diagnose issues.

#### Acceptance Criteria

1. WHEN YoloService 拋出任何例外，THE YoloController SHALL 以 try/catch 攔截，記錄 `_logger.LogError(ex, ...)` 並回傳 HTTP 500 及 `{ "message": "影像辨識失敗", "error": "{ex.Message}" }`。
2. WHEN YoloService 拋出 `TimeoutException`，THE YoloController SHALL 回傳 HTTP 504 及 `{ "message": "影像辨識逾時，請稍後再試" }`。
3. WHEN YoloService 成功完成辨識，THE YoloService SHALL 以 `_logger.LogInformation` 記錄辨識到的物件數量與執行時間（毫秒）。
4. WHEN YoloService 呼叫 Python 腳本前，THE YoloService SHALL 以 `_logger.LogDebug` 記錄所使用的 Python 路徑、腳本路徑與暫存影像路徑。

### Requirement 6

**User Story:** As a developer, I want the new service to follow existing DI and Swagger conventions, so that it remains consistent with the project architecture.

#### Acceptance Criteria

1. THE DISetup SHALL 在 `AddApplicationServices()` 方法中以 `AddScoped<IYoloService, YoloService>()` 註冊 YOLO 服務，並以 `services.Configure<YoloSettings>(configuration.GetSection("YoloSettings"))` 注入設定。
2. THE YoloController SHALL 加上 `[ApiExplorerSettings(GroupName = "Yolo")]` 與 `[Produces("application/json")]` 標記，使 Swagger UI 能正確分組顯示。
3. THE YoloResponseDto 與 THE DetectionResultDto SHALL 各欄位加上 XML 文件註解（`/// <summary>`），以產生 Swagger 說明文字。
