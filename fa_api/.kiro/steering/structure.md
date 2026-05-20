# Project Structure

```
fa_api/
├── Controllers/                    # API 控制器
│   ├── WaterGovController.cs       # 水利署資料 API（水庫、供水、水位、雨量）
│   └── NcdrController.cs           # NCDR 枯旱預警 API
│
├── Services/                       # 業務邏輯與外部 API 整合
│   ├── WraGov/
│   │   ├── IWraGovService.cs       # 水利署服務介面（含所有資料模型定義）
│   │   └── WraGovService.cs        # 水利署服務實作（HttpClient + Newtonsoft.Json）
│   ├── Ncdr/
│   │   ├── INcdrDroughtService.cs  # NCDR 枯旱預警服務介面
│   │   └── NcdrDroughtService.cs   # NCDR 服務實作（JSON feed + CAP XML 解析）
│   └── Mail/
│       ├── IMailService.cs         # 郵件服務介面
│       ├── MailService.cs          # MailKit SMTP 實作
│       ├── MailRequest.cs          # 郵件請求模型（To、Subject、Body、Cc、IsHtml）
│       └── SmtpSettings.cs         # SMTP 設定模型（對應 appsettings SmtpSettings 區段）
│
├── Dtos/                           # API 回傳用資料傳輸物件（Swagger 文件化）
│   ├── Ncdr/
│   │   └── DroughtAreaDto.cs       # DroughtAlertDto、DroughtInfoDto、DroughtAreaDto
│   └── WraGov/
│       ├── BasicDto.cs             # CityDto、TownDto
│       ├── DisasterDto.cs          # 淹水/水利設施災情 DTO
│       ├── EventDto.cs             # 防汛事件 DTO
│       ├── FloodDefenseDto.cs      # 防汛資材 DTO
│       ├── RainDto.cs              # 雨量站/即時雨量/警示 DTO
│       ├── ReservoirDto.cs         # 水庫站/即時/每日/警示 DTO
│       ├── StatisticsDto.cs        # 淹水/水利設施統計 DTO
│       ├── WaterStationDto.cs      # 水位站/即時水位/警示 DTO
│       └── WaterSupplyDto.cs       # WaterSupplyConditionDto
│
├── Models/                         # 資料庫實體模型（EF Core）
│   └── FaWrSignalLevel.cs          # FA_WR_SignalLevel 資料表（枯旱預警歷史紀錄）
│
├── Data/                           # EF Core DbContext
│   └── FaDbContext.cs              # FaDbContext（DbSet<FaWrSignalLevel>）
│
├── Schedule/                       # Hangfire 排程封裝
│   └── MailSchedule.cs             # 郵件排程（Fire-and-forget / 延遲 / 週期）
│
├── ServiceExtensions/              # DI 與 Swagger 設定擴充方法
│   └── DISetup.cs                  # AddApplicationServices()、AddSwaggerDocumentation()
│
├── DocSQL/                         # 資料庫建置 SQL 腳本
│   └── Step1_Create_WaterSignal.sql
│
├── appsettings.json                # 基本設定（Logging、ConnectionStrings、SmtpSettings）
├── appsettings.Development.json    # 開發環境設定
├── .env                            # 敏感環境變數（不提交版控）
├── Startup.cs                      # 呼叫 DISetup 擴充方法、設定 Middleware 管線
├── Program.cs                      # 應用程式進入點
└── fa_api.csproj                   # 專案設定（.NET Core 3.1）
```

## 架構慣例

### 控制器
- API 控制器繼承 `ControllerBase`，加上 `[ApiController]` 與 `[Route("api/[controller]")]`
- 建構子注入：`ILogger<T>`、對應 Service 介面、`IMemoryCache`
- 快取鍵以 `const string` 定義，快取時間統一 `TimeSpan.FromMinutes(10)`
- 例外處理：try/catch 包覆所有 async 操作，回傳 `StatusCode(500, new { message, error })`
- 回傳型別使用 `Dtos/` 中的 DTO 類別（非直接回傳 Service 內部模型）

### Services
- 每個外部資料來源對應一個 Service 資料夾（`Services/{Source}/`）
- 介面（`I{Name}Service.cs`）與實作（`{Name}Service.cs`）分開
- 水利署資料模型定義在 `IWraGovService.cs` 的 `#region 資料模型` 區塊
- 使用 `IHttpClientFactory` 建立 HttpClient（不直接 new）
- JSON 解析使用 `Newtonsoft.Json`（`JArray.Parse`、`JObject.Parse`）
- XML 解析使用 `System.Xml.Linq.XDocument`（用於 CAP 格式）
- 數值解析使用私有輔助方法 `ParseDecimal(JToken)` / `ParseInt(JToken)` 避免例外

### Dtos
- 每個 Controller 對應的回傳結構定義在 `Dtos/` 資料夾
- 加上 XML 文件註解（`/// <summary>`）以產生 Swagger 說明
- 命名慣例：`{Entity}Dto`（例：`ReservoirStationDto`、`DroughtAlertDto`）

### Models（EF Core 實體）
- 資料庫實體放在 `Models/` 資料夾，以 `[Table]`、`[Key]`、`[Column]` 標記
- DbContext 放在 `Data/FaDbContext.cs`
- 連線字串從 `appsettings.json` 的 `ConnectionStrings:DefaultConnection` 讀取

### Mail 服務
- `MailRequest`：郵件請求（To、Subject、Body、IsHtml、Cc）
- `SmtpSettings`：從 `appsettings.json` 的 `SmtpSettings` 區段注入（`IOptions<SmtpSettings>`）
- `MailSchedule`：Hangfire 排程封裝，提供 `EnqueueSend`、`ScheduleSend`、`AddRecurringMail`

### DI 註冊（ServiceExtensions/DISetup.cs）
- 所有服務在 `AddApplicationServices()` 擴充方法中集中註冊
- Swagger 設定在 `AddSwaggerDocumentation()` 擴充方法中
- Services 以 `AddScoped<IInterface, Implementation>()` 註冊
- `AddHttpClient()`、`AddMemoryCache()` 已全域註冊
- Hangfire 使用 `MemoryStorage`（開發環境）

### 命名空間
- 根命名空間：`fa_api`
- 服務：`fa_api.Services.{Source}`
- DTO：`fa_api.Dtos.{Source}`
- 模型：`fa_api.Models`
- 資料：`fa_api.Data`
- 控制器：`fa_api.Controllers`
- 排程：`fa_api.Schedule`
- 擴充方法：`fa_api.ServiceExtensions`
