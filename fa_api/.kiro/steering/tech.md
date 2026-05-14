# Tech Stack

## Framework & Runtime
- **ASP.NET Core MVC + Web API** — .NET Core 3.1 (`netcoreapp3.1`)
- **C#** — 主要語言
- 模式：MVC（首頁）+ REST API（`[ApiController]`）混合架構

## 已安裝套件（fa_api.csproj）

| 套件 | 版本 | 用途 |
|------|------|------|
| `Microsoft.EntityFrameworkCore` | 3.1.32 | ORM 核心 |
| `Microsoft.EntityFrameworkCore.SqlServer` | 3.1.32 | SQL Server 提供者 |
| `Microsoft.EntityFrameworkCore.Tools` | 3.1.32 | Migration 工具 |
| `Microsoft.EntityFrameworkCore.Design` | 3.1.32 | 設計時工具 |
| `Newtonsoft.Json` | （間接依賴） | JSON 解析（JArray/JObject） |

> 注意：`Newtonsoft.Json` 透過 EF Core 間接引入，直接使用於 Service 層的 JSON 解析。

## 前端函式庫（wwwroot/lib，勿修改）
- Bootstrap
- jQuery
- jQuery Validation + Unobtrusive Validation

## 中介軟體管線（Startup.cs）
開發環境：`DeveloperExceptionPage` → `StaticFiles` → `Routing` → `CORS("AllowReactApp")` → `Authorization` → `Endpoints`

生產環境：`ExceptionHandler` → `HSTS` → `HttpsRedirection` → `StaticFiles` → `Routing` → `CORS` → `Authorization` → `Endpoints`

## CORS 設定
允許來源：`http://localhost:3000`、`http://localhost:3001`（React 開發伺服器）
Policy 名稱：`"AllowReactApp"`

## 設定檔
- `appsettings.json` — 基本設定（Logging、AllowedHosts）
- `appsettings.Development.json` — 開發環境覆寫
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
