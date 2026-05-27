# 實作任務清單：ImgRead 影像辨識模組

## 任務相依圖

```
Task 1 (資料層 hook)
    └── Task 2 (queries.js 匯出)
            └── Task 4 (ImgUploader 元件)
                    └── Task 5 (ImgReadPage 主頁面)
                            ├── Task 6 (路由整合)
                            └── Task 7 (App.js 選單)

Task 3 (驗證純函式) ──→ Task 4
Task 8 (屬性測試)   ──→ 依賴 Task 1, 3
Task 9 (單元測試)   ──→ 依賴 Task 4, 5
```

---

## Tasks

- [x] 1. 建立 useDetectImage mutation hook
  - 建立目錄 `src/libs/imgRead/`
  - 建立 `src/libs/imgRead/imgRead.js`
  - 實作 `detectImage(file)` async 函式：建立 `FormData`，以欄位名稱 `image` 附加 `File` 物件，使用 `axios.post()` 搭配 `process.env.REACT_APP_API_BASE_URL || 'http://localhost:65326'` 發送至 `/api/yolo/detect`，設定 `Content-Type: multipart/form-data` 標頭，回傳 `response.data`
  - 實作 `useDetectImage` hook：使用 react-query v3 的 `useMutation(detectImage)` 包裝，匯出 `{ mutate, isLoading, data, error, reset }`
  - **驗收：** hook 可正確建構 FormData 並呼叫正確的 API 端點；不透過 `apiClient` 實例
  - **對應需求：** 需求 4.1, 4.2, 4.3, 4.4, 4.5, 4.6

- [x] 2. 更新 queries.js 統一匯出
  - 在 `src/api/queries.js` 新增 `export * from '../libs/imgRead/imgRead';`
  - **驗收：** 可從 `src/api/queries.js` 成功 import `useDetectImage`
  - **對應需求：** 需求 4.1

- [x] 3. 建立驗證純函式與格式化工具
  - 在 `src/pages/imgRead/components/ImgUploader.js` 中具名匯出以下純函式：
    - `isValidMimeType(mimeType)` — 僅對 `'image/jpeg'`、`'image/png'`、`'image/bmp'` 回傳 `true`
    - `isValidFileSize(sizeInBytes)` — `sizeInBytes <= 10 * 1024 * 1024` 時回傳 `true`
    - `formatConfidence(confidence)` — 回傳 `${(confidence * 100).toFixed(2)}%`
    - `formatElapsedMs(elapsedMs)` — 回傳 `辨識耗時：${elapsedMs} ms`
  - 定義常數 `ALLOWED_MIME_TYPES` 與 `MAX_FILE_SIZE`
  - **驗收：** 四個純函式可獨立 import 並測試，不依賴 React 或元件狀態
  - **對應需求：** 需求 1.2, 1.3, 3.1, 3.2

- [x] 4. 建立 ImgUploader 元件
  - 建立目錄 `src/pages/imgRead/components/`
  - 完成 `src/pages/imgRead/components/ImgUploader.js`（Task 3 已建立純函式骨架）
  - 使用 Ant Design `Upload` 元件，設定 `beforeUpload` 回傳 `false` 阻止自動上傳，支援點擊與拖曳（`Dragger` 或 `Upload` + `drag`）
  - `beforeUpload` 中依序驗證 MIME type 與檔案大小：
    - 格式不符：呼叫 `message.warning('僅支援 JPEG、PNG、BMP 格式')`，清除狀態，回傳 `false`
    - 大小超限：呼叫 `message.warning('檔案大小不得超過 10 MB')`，清除狀態，回傳 `false`
    - 有效：以 `URL.createObjectURL(file)` 產生預覽 URL，呼叫 `onFileSelect(file)`
  - 預覽縮圖：以 `<img>` 顯示，CSS 設定 `min-width: 100px; min-height: 100px; object-fit: contain;`
  - 宣告 PropTypes：`onFileSelect: PropTypes.func.isRequired`、`onFileRemove: PropTypes.func`、`disabled: PropTypes.bool`
  - 設定 `defaultProps`：`onFileRemove: () => {}`、`disabled: false`
  - **驗收：** 選擇有效檔案後顯示預覽；無效格式或超大檔案顯示對應警告並清除選擇；`disabled` 為 `true` 時上傳區域不可操作
  - **對應需求：** 需求 1.1, 1.2, 1.3, 1.4, 1.5, 1.6, 1.7, 需求 6.1

- [x] 5. 建立 ImgReadPage 主頁面元件
  - 建立 `src/pages/imgRead/ImgReadPage.js`
  - 使用 `useState(null)` 管理 `selectedFile`
  - 呼叫 `useDetectImage()`，解構 `{ mutate, isLoading, data, error, reset }`
  - 使用 `useEffect` 監聽 `error`，依錯誤類型顯示對應訊息：
    - `error.response` 存在：`message.error('辨識失敗，請稍後再試')`
    - `error.request` 存在或 `error.code === 'ECONNABORTED'`：`message.error('網路錯誤，請確認連線後重試')`
  - 渲染 `ImgUploader`，傳入 `onFileSelect`（更新 `selectedFile`）、`onFileRemove`（清除 `selectedFile` 並呼叫 `reset()`）、`disabled={isLoading}`
  - 渲染「開始辨識」按鈕：`disabled={!selectedFile || isLoading}`、`loading={isLoading}`，點擊時呼叫 `mutate(selectedFile)`
  - 結果區域（僅在 `data` 存在時顯示）：
    - `data.detections.length > 0`：顯示「共偵測到 {n} 個物件」、Ant Design `Table`（欄位：類別標籤、信心分數、X、Y、寬度、高度）、`rowKey={(_, i) => i}`
    - `data.detections.length === 0`：顯示「未偵測到任何物件」
    - 顯示執行時間：`formatElapsedMs(data.elapsedMs)`
  - Table 欄位定義使用 `formatConfidence` 格式化信心分數，`dataIndex: ['bbox', 'x']` 等巢狀路徑存取邊界框
  - **驗收：** 初始狀態按鈕 disabled、無結果區域；選擇有效檔案後按鈕啟用；送出期間按鈕 loading；成功後顯示結果；錯誤時顯示對應 message
  - **對應需求：** 需求 2.1, 2.2, 2.3, 2.4, 2.5, 2.6, 需求 3.1, 3.2, 3.3, 3.4, 3.5, 需求 6.2

- [x] 6. 更新 AppRoutes.js 新增路由
  - 在 `src/routes/AppRoutes.js` import `ImgReadPage`
  - 在 `<Routes>` 內新增 `<Route path="/img-read" element={<ImgReadPage />} />`
  - **驗收：** 瀏覽 `/img-read` 可正確渲染 `ImgReadPage`
  - **對應需求：** 需求 5.1

- [x] 7. 更新 App.js 新增側邊選單項目
  - 在 `src/App.js` 的 `items` 陣列新增頂層選單項目：`{ key: '/img-read', icon: <ScanOutlined />, label: '影像辨識' }`
  - 從 `@ant-design/icons` import `ScanOutlined`
  - **驗收：** 側邊選單顯示「影像辨識」項目；點擊後導覽至 `/img-read`；`selectedKeys` 正確反映當前路徑
  - **對應需求：** 需求 5.2, 5.3

- [x] 8. 撰寫屬性測試（Property-Based Testing）
  - 安裝 `fast-check`：`npm install --save-dev fast-check`
  - 建立 `src/libs/imgRead/__tests__/imgRead.property.test.js`
  - 實作以下 7 個屬性測試（每個至少 100 次迭代）：
    1. **屬性 1**（MIME type 驗證完備性）：`fc.string()` — 僅 `image/jpeg`、`image/png`、`image/bmp` 回傳 `true`
    2. **屬性 2**（檔案大小邊界）：`fc.nat()` — `size <= 10485760` 時回傳 `true`，否則 `false`
    3. **屬性 3**（按鈕啟用狀態）：`fc.boolean()` — `disabled` 恰好等於 `!hasValidFile`
    4. **屬性 4**（FormData 建構）：mock `axios.post`，驗證 FormData 包含欄位 `image`
    5. **屬性 5**（confidence 格式化）：`fc.float({ min: 0, max: 1 })` — 結果符合 `XX.XX%` 格式
    6. **屬性 6**（elapsedMs 格式化）：`fc.nat()` — 結果為 `辨識耗時：{n} ms`
    7. **屬性 7**（物件總數一致性）：`fc.array(fc.record({...}), { minLength: 1 })` — 顯示數字等於 `detections.length`
  - **驗收：** `npm test -- --watchAll=false` 執行所有屬性測試通過
  - **對應需求：** 需求 1.2, 1.3, 2.1, 2.2, 2.3, 3.1, 3.2, 3.4, 4.3, 4.4

- [-] 9. 撰寫單元測試（Example-Based）
  - 建立 `src/pages/imgRead/__tests__/ImgReadPage.test.js`
  - 使用 `@testing-library/react` + `jest`，mock `useDetectImage` hook
  - 實作以下測試案例：
    - 初始渲染：結果區域不顯示，「開始辨識」按鈕為 disabled
    - 選擇無效 MIME type：`message.warning` 被呼叫，`selectedFile` 為 null
    - 選擇超過 10 MB 檔案：`message.warning` 被呼叫，`selectedFile` 為 null
    - 選擇有效檔案：預覽縮圖出現，按鈕變為 enabled
    - 點擊「開始辨識」：`mutate` 被呼叫，按鈕進入 loading 狀態
    - API 成功（有結果）：Table 顯示，物件總數顯示，執行時間顯示
    - API 成功（空結果）：顯示「未偵測到任何物件」，無 Table
    - API HTTP 錯誤：`message.error` 顯示「辨識失敗，請稍後再試」
    - API 網路錯誤：`message.error` 顯示「網路錯誤，請確認連線後重試」
  - **驗收：** `npm test -- --watchAll=false` 執行所有單元測試通過
  - **對應需求：** 需求 1, 2, 3, 6
