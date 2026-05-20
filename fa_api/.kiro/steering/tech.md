# Tech Stack

## Framework & Runtime
- **ASP.NET Core Web API** — .NET Core 3.1 (`netcoreapp3.1`)
- **C#** — 主要語言
- 模式：純 REST API（`[ApiController]`），根路徑重定向至 Swagger UI

## 已安裝套件（fa_api.csproj）

| 套件 | 版本 | 用途 |
|------|------|------|
| `Microsoft.EntityFrameworkCore` | 3.1.32 | ORM 核心 |
| `Microsoft.EntityFrameworkCore.SqlServer` | 3.1.32 | SQL Server 提供者 |
| `Microsoft.EntityFrameworkCore.Tools` | 3.1.32 | Migration 工具 |
| `Microsoft.EntityFrameworkCore.Design` | 3.1.32 | 設計時工具 |
| `Newtonsoft.Json` | 13.0.1 | JSON 解析（JArray/JObject） |
| `Hangfire.AspNetCore` | 1.8.14 | 背景排程框架 |
| `Hangfire.MemoryStorage` | 1.8.0 | Hangfire 記憶體儲存（開發用） |
| `MailKit` | 4.3.0 | SMTP 郵件發送 |
| `MimeKit` | 4.3.0 | MIME 郵件建構（MailKit 依賴） |
| `Swashbuckle.AspNetCore` | 5.6.3 | Swagger / OpenAPI 文件 |

## 中介軟體管線（Startup.cs）

開發環境：
`DeveloperExceptionPage` → `根路徑重定向(/→/swagger)` → `Swagger` → `SwaggerUI` → `HangfireDashboard` → `Routing` → `CORS("AllowReactApp")` → `Authorization` → `Endpoints`

生產環境：
`ExceptionHandler(/error)` → `HSTS` → `根路徑重定向` → `Swagger` → `SwaggerUI` → `HangfireDashboard` → `HttpsRedirection` → `Routing` → `CORS` → `Authorization` → `Endpoints`

## CORS 設定
允許來源：`http://localhost:3000`、`http://localhost:3001`（React 開發伺服器）
Policy 名稱：`"AllowReactApp"`

## Swagger UI
- 路徑：`/swagger`（根路徑 `/` 自動重定向）
- 標題：水利署災害預警 API v1
- XML 文件：自動產生（`GenerateDocumentationFile=true`）

## Hangfire Dashboard
- 路徑：`/hangfire`
- 儲存：記憶體（`Hangfire.MemoryStorage`，重啟後清空）

## 設定檔
- `appsettings.json` — 基本設定（Logging、AllowedHosts、ConnectionStrings、SmtpSettings）
- `appsettings.Development.json` — 開發環境覆寫
- `.env` — 敏感環境變數（不提交版控）
- 環境變數：`ASPNETCORE_ENVIRONMENT`

## 常用指令

### 執行（CLI）
```bash
dotnet run
```
執行於 `https://localhost:5001` / `http://localhost:5000`

### 建置
```bash
dotnet build
```

### 還原套件
```bash
dotnet restore
```

### 透過 IIS Express 執行
使用 Visual Studio，選擇 `IIS Express` 啟動設定（HTTP: 65326 / HTTPS: 44328）

### EF Core Migrations（Package Manager Console）
```
Add-Migration <MigrationName>
Update-Database
Remove-Migration
```

### 新增 NuGet 套件（Package Manager Console）
```
Install-Package <PackageName> -Version <Version>
```
