# 產品概覽 Product Overview

**Code WMS** 是一套供內部人員使用的網頁版管理系統（WMS），介面語言為繁體中文。

## 核心模組

- **水資源管理 (Water Resource Management)** — 水庫蓄水情形與水情燈號儀表板。資料透過本地 .NET 後端代理 WRA（水利署）與 NCDR（國家災害防救科技中心）的開放 API 取得。
- **系統管理 (System Administration)** — 帳號與權限管理。
- **水費管理 (Water Fees)** — 水費資料的新增、查詢、修改、刪除（CRUD）。

## 目標使用者

負責水資源資料管理與系統帳號維護的內部人員。

## 後端

連接本地 .NET 後端，預設位址為 `http://localhost:65326`，可透過環境變數 `REACT_APP_API_BASE_URL` 覆寫。後端作為外部政府 API 的代理層，避免 CORS 問題。JWT 驗證機制已建立骨架但尚未完整啟用。

### 後端 API 命名空間

- `/api/watergov/...` — WRA 水利署資料（水庫、雨量、防洪等）
- `/api/ncdr/...` — NCDR 災害／乾旱警示資料
- `/api/water-fees` — 水費 CRUD
