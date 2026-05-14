# 專案結構 Project Structure

```
src/
├── api/
│   ├── apiClient.js       # Axios 實例，包含攔截器與 JWT 刷新骨架
│   └── queries.js         # 統一從 libs/ 重新匯出所有 React Query hooks，元件從此處 import
│
├── libs/                  # 資料層：依資料來源分組，每個模組包含 API 函式與 React Query hooks
│   ├── WraGov/            # WRA 水利署資料，透過本地後端代理
│   │   ├── index.js       # 重新匯出所有 WraGov 模組
│   │   ├── reservoir.js   # 水庫：基本資料、即時資訊、每日統計、警示、影響範圍
│   │   ├── basic.js       # 基本水文資料
│   │   ├── event.js       # 水文事件
│   │   ├── floodDefense.js # 防洪資料
│   │   ├── rain.js        # 雨量資料
│   │   ├── statistics.js  # 統計資料
│   │   └── water.js       # 水位資料
│   └── Ncdr/              # NCDR 國家災害防救科技中心資料
│       ├── index.js
│       └── disaster.js    # 災害警示、乾旱警示
│
├── pages/                 # 路由層級的頁面元件
│   ├── system/
│   │   └── Users.js       # 帳號權限管理頁面
│   └── waterDashboard/    # 水資源儀表板頁面與子元件
│       ├── components/
│       │   ├── ReservoirCard.js   # 水庫卡片元件
│       │   └── TaiwanMap.js       # 台灣地圖元件
│       ├── ReservoirDashboard.js  # 水庫蓄水情形圖
│       ├── WaterDashboard.js      # Tab 容器（水庫蓄水情形圖 / 水情燈號）
│       └── WaterWarningDashboard.js # 水情燈號頁面
│
├── components/            # 共用元件（部分為預留骨架）
│   ├── Applayout/         # （預留）
│   ├── Footer/            # （預留）
│   ├── Header/            # （預留）
│   ├── SideMenu/          # （預留）
│   └── Users/
│       └── Users.js
│
├── routes/
│   └── AppRoutes.js       # 集中管理路由定義
│
├── App.js                 # 根佈局：Header、Sider（導覽選單）、Content + Provider 設定
└── index.js               # React DOM 進入點
```

## 重要慣例

### 資料層（`libs/`）

- 模組依**資料來源**分組，而非依 UI 功能：`WraGov/` 對應水利署、`Ncdr/` 對應國家災害防救科技中心
- 每個模組匯出原始非同步 fetch 函式與對應的 React Query hooks
- Fetch 函式直接使用 `axios.get()` 搭配 `process.env.REACT_APP_API_BASE_URL`
- 後端 API 路徑規則：`/api/watergov/[resource]/[endpoint]`、`/api/ncdr/[resource]`
- 所有 hooks 透過 `src/api/queries.js` 統一重新匯出，元件應從此處 import，不直接引用 `libs/`

### 路由

路由定義集中在 `src/routes/AppRoutes.js`。`App.js` 的側邊選單以路由路徑作為 menu item key，透過 `useNavigate` 進行頁面切換。

目前有效路由：
- `/water-resource/dashboard` — 水資源儀表板（預設頁面）
- `/admin/users` — 帳號權限管理

### 命名規則

- 元件檔案與資料夾：PascalCase（例如 `WaterDashboard.js`、`WraGov/`）
- Lib / 工具檔案：camelCase（例如 `reservoir.js`、`disaster.js`）
- React Query 快取鍵：camelCase 字串，對應資料實體（例如 `'wraReservoirStation'`、`'wraReservoirRealTimeInfo'`）

### UI 規範

- 所有 UI 使用 Ant Design v5 元件
- 使用者介面文字一律使用繁體中文
- 表格頁面的 `rowKey` 設為實體主鍵欄位
- 狀態欄位使用 `<Tag color="green/red">` 顯示啟用／停用
- 刪除操作包裝在 `<Popconfirm>` 內，需使用者確認
