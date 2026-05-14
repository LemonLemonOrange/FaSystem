# Project Structure

```
fa_api/
├── Controllers/                    # API 控制器
│   ├── HomeController.cs           # MVC 首頁（非 API）
│   ├── WaterGovController.cs       # 水利署資料 API（水庫、供水、水位、雨量）
│   └── NcdrController.cs           # NCDR 枯旱預警 API
│
├── Services/                       # 業務邏輯與外部 API 整合
│   ├── WraGov/
│   │   ├── IWraGovService.cs       # 水利署服務介面（含所有資料模型定義）
│   │   └── WraGovService.cs        # 水利署服務實作（HttpClient + Newtonsoft.Json）
│   └── Ncdr/
│       ├── INcdrDroughtService.cs  # NCDR 枯旱預警服務介面
│       └── NcdrDroughtService.cs   # NCDR 服務實作（JSON feed + CAP XML 解析）
│
├── Models/                         # 資料模型
│   ├── DroughtModels.cs            # 枯旱預警模型（DroughtAlert, DroughtInfo, DroughtArea）
│   └── ErrorViewModel.cs           # MVC 錯誤頁面模型
│
├── Views/                          # Razor 視圖（僅 HomeController 使用）
│   ├── Home/
│   ├── Shared/
│   ├── _ViewImports.cshtml
│   └── _ViewStart.cshtml
│
├── wwwroot/                        # 靜態資源（CSS、JS、Bootstrap、jQuery）
├── appsettings.json                # 基本設定（Logging）
├── appsettings.Development.json    # 開發環境設定
├── Startup.cs                      # DI 註冊、Middleware 管線、CORS 設定
├── Program.cs                      # 應用程式進入點
└── fa_api.csproj                   # 專案設定（.NET Core 3.1）
```

## 架構慣例

### 控制器
- API 控制器繼承 `ControllerBase`，加上 `[ApiController]` 與 `[Route("api/[controller]")]`
- MVC 控制器繼承 `Controller`（僅 HomeController）
- 建構子注入：`ILogger<T>`、對應 Service 介面、`IMemoryCache`
- 快取鍵以 `const string` 定義，快取時間統一 `TimeSpan.FromMinutes(10)`
- 例外處理：try/catch 包覆所有 async 操作，回傳 `StatusCode(500, new { message, error })`

### Services
- 每個外部資料來源對應一個 Service 資料夾（`Services/{Source}/`）
- 介面（`I{Name}Service.cs`）與實作（`{Name}Service.cs`）分開
- 水利署資料模型定義在 `IWraGovService.cs` 的 `#region 資料模型` 區塊
- 使用 `IHttpClientFactory` 建立 HttpClient（不直接 new）
- JSON 解析使用 `Newtonsoft.Json`（`JArray.Parse`、`JObject.Parse`）
- XML 解析使用 `System.Xml.Linq.XDocument`（用於 CAP 格式）
- 數值解析使用私有輔助方法 `ParseDecimal(JToken)` / `ParseInt(JToken)` 避免例外

### Models
- 枯旱預警等複雜模型放在 `Models/` 資料夾
- 水利署的簡單資料模型（`ReservoirData` 等）定義在 `IWraGovService.cs` 中

### DI 註冊（Startup.cs）
- Services 以 `AddScoped<IInterface, Implementation>()` 註冊
- `AddHttpClient()`、`AddMemoryCache()` 已全域註冊

### 命名空間
- 根命名空間：`fa_api`
- 服務：`fa_api.Services.{Source}`
- 模型：`fa_api.Models`
- 控制器：`fa_api.Controllers`
