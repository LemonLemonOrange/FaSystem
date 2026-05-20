# Product Overview

`fa_api` 是一個 ASP.NET Core Web API 後端，作為防汛水情監控系統（防汛系統）的資料聚合層。它整合多個台灣政府開放資料來源，提供水庫、供水、水位、雨量及枯旱預警等即時資料，供前端（React）應用程式消費。

## 外部資料來源

| 來源 | 說明 | Base URL |
|------|------|----------|
| 水利署 (WRA) | 水庫、供水、水位、雨量 | `https://fhy.wra.gov.tw/WraApi/v1` |
| NCDR 民生示警 | 枯旱預警 CAP 格式 | `https://alerts.ncdr.nat.gov.tw/webapi/JSONAtomFeed.ashx?AlertType=2099` |

## 主要功能

- **水庫水情**：即時蓄水量、蓄水率、進出水量
- **水庫營運**：每日累積雨量、放水量、發電量
- **放水警戒**：壩頂溢洪道放水警戒狀態
- **供水情勢**：各地區供水限制狀態（正常/減壓/限水/停水）
- **即時水位**：河川水情站水位與警戒值
- **即時雨量**：各測站累積雨量與時雨量
- **枯旱預警**：NCDR CAP 格式枯旱警示（嚴重程度 + 影響縣市）
- **預警歷史紀錄**：枯旱預警寫入 SQL Server（`FA_WR_SignalLevel` 資料表）
- **郵件通知**：透過 Hangfire 排程發送 SMTP 郵件（即時 / 延遲 / 週期）

## 前端整合

- CORS 允許 React 開發伺服器：`http://localhost:3000`、`http://localhost:3001`
- 所有 API 端點統一前綴：`/api/`
- Swagger UI 提供互動式 API 文件：`/swagger`

## 管理介面

- **Swagger UI**：`/swagger` — API 文件與測試介面
- **Hangfire Dashboard**：`/hangfire` — 背景排程監控
