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
| Props 型別驗證 | prop-types | ^15.x |

> **注意：** `package.json` 同時列有 `react-query` v3 與 `@tanstack/react-query` v5，但現有程式碼統一使用 v3 API（`import from 'react-query'`）。請勿混用兩個版本的 API。

## 常用指令

```bash
# 啟動開發伺服器（localhost:3300）
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

## PropTypes 型別驗證

所有元件（包含 `pages/`、`components/`）**必須**使用 `prop-types` 宣告 props 型別。

### 基本規則

- 每個接收 props 的元件，檔案底部必須加上 `ComponentName.propTypes = { ... }`
- 必填 props 加上 `.isRequired`
- 選填 props 提供 `ComponentName.defaultProps = { ... }` 預設值

### 常用型別對照

| Props 內容 | PropTypes 寫法 |
|---|---|
| 字串 | `PropTypes.string` |
| 數字 | `PropTypes.number` |
| 布林 | `PropTypes.bool` |
| 函式（callback） | `PropTypes.func` |
| 陣列 | `PropTypes.array` / `PropTypes.arrayOf(PropTypes.shape({...}))` |
| 物件 | `PropTypes.object` / `PropTypes.shape({...})` |
| React 節點 | `PropTypes.node` |
| 任意型別 | `PropTypes.any`（盡量避免） |

### 範例

```js
import PropTypes from 'prop-types';

function ReservoirCard({ stationName, percentage, onClick }) {
  // ...
}

ReservoirCard.propTypes = {
  stationName: PropTypes.string.isRequired,
  percentage:  PropTypes.number,
  onClick:     PropTypes.func,
};

ReservoirCard.defaultProps = {
  percentage: 0,
  onClick:    () => {},
};
```

## 樣式

- 全域樣式放在 `src/index.css` 與 `src/App.css`
- 元件專屬 CSS 與元件檔案放在同一資料夾（例如 `WaterDashboard.css`、`ReservoirCard.css`）
- 版面微調使用 Ant Design 元件的 `style={{ ... }}` prop
