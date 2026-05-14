# 技術堆疊 Tech Stack

## 建置系統

- **Create React App**（react-scripts 5.0.1）— 無自訂 webpack 設定，非 Vite
- Node.js / npm

## 核心套件

| 類別 | 套件 | 版本 |
|---|---|---|
| UI 框架 | React | ^18.3.1 |
| 路由 | react-router-dom | ^7.6.0 |
| UI 元件庫 | Ant Design (antd) | ^5.24.7 |
| 圖示 | @ant-design/icons | ^5.6.1 |
| 伺服器狀態 / 資料請求 | react-query (v3) | ^3.39.3 |
| HTTP 客戶端 | axios | ^1.16.0 |
| Token 自動刷新 | axios-auth-refresh | ^3.3.6 |
| 地圖視覺化 | react-simple-maps | ^3.0.0 |
| XML 解析 | fast-xml-parser | ^4.5.3 |
| 測試 | @testing-library/react, jest-dom | ^16 / ^6 |

> **注意：** `package.json` 同時列有 `react-query` v3 與 `@tanstack/react-query` v5，但現有程式碼統一使用 v3 API（`import from 'react-query'`）。請勿混用兩個版本的 API。

## 常用指令

```bash
# 啟動開發伺服器（localhost:3000）
npm start

# 執行測試（watch 模式）
npm test

# 執行測試一次（CI / 非互動環境）
npm test -- --watchAll=false

# 建置正式版
npm run build
```

## API 客戶端

使用單一 axios 實例：**`src/api/apiClient.js`**

- Base URL：`process.env.REACT_APP_API_BASE_URL` 或預設 `http://localhost:65326`
- 包含 request / response 攔截器，以及透過 `axios-auth-refresh` 建立的 JWT 刷新骨架
- Response 攔截器會自動解包 `response.data`

> **注意：** `src/libs/WraGov/` 與 `src/libs/Ncdr/` 的模組直接使用 `axios.get()` 搭配相同的 `API_BASE_URL` 環境變數，而非透過 `apiClient` 實例。新增 lib 模組時請維持此模式。

## 樣式

- 全域樣式放在 `src/index.css` 與 `src/App.css`
- 元件專屬 CSS 與元件檔案放在同一資料夾（例如 `WaterDashboard.css`、`ReservoirCard.css`）
- 版面微調使用 Ant Design 元件的 `style={{ ... }}` prop
