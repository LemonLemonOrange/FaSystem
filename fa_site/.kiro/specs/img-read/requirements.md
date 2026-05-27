# 需求文件：ImgRead 影像辨識模組

## 簡介

ImgRead 是 Code WMS 系統的新功能模組，讓內部人員能夠上傳影像並透過後端 YOLO 物件偵測 API 執行自動辨識，並在頁面上呈現偵測結果清單與執行時間。

本模組遵循專案現有技術堆疊：React 18 + Ant Design v5、axios（直接使用，不透過 apiClient）、react-query v3、prop-types。

---

## 詞彙表

- **ImgRead_Page**：影像辨識功能的主頁面元件，路由路徑為 `/img-read`
- **ImgUploader**：負責接收使用者選擇影像並觸發上傳的子元件
- **DetectionResult**：YOLO API 回傳的單筆偵測結果，包含類別標籤、信心分數與邊界框座標
- **YoloResponseDto**：後端 `POST /api/yolo/detect` 的回傳資料結構，包含 `detections`（`DetectionResult` 陣列）與 `elapsedMs`（執行時間，毫秒）
- **YOLO_API**：後端 YOLO 物件偵測端點，路徑為 `POST /api/yolo/detect`，接受 `multipart/form-data` 格式的影像檔案，FormData 欄位名稱為 `image`
- **Detect_Hook**：封裝 YOLO API 呼叫的 React Query mutation hook（`useDetectImage`），位於 `src/libs/imgRead/imgRead.js`
- **支援格式**：JPEG、PNG、BMP
- **檔案大小上限**：10 MB

---

## 需求

### 需求 1：影像上傳

**使用者故事：** 身為內部人員，我希望能夠選擇本機影像檔案並上傳，以便執行 YOLO 物件偵測。

#### 驗收標準

1. THE **ImgRead_Page** SHALL 提供一個影像上傳區域，支援點擊選擇檔案與拖曳放置兩種操作方式，且每次僅允許選擇單一檔案。
2. WHEN 使用者選擇檔案，THE **ImgUploader** SHALL 驗證檔案格式是否為 JPEG、PNG 或 BMP（依 MIME type 判斷：`image/jpeg`、`image/png`、`image/bmp`）。
3. WHEN 使用者選擇檔案，THE **ImgUploader** SHALL 驗證檔案大小是否不超過 10 MB（10 × 1024 × 1024 bytes）。
4. IF 使用者選擇的檔案格式不符合支援格式，THEN THE **ImgUploader** SHALL 顯示繁體中文錯誤訊息「僅支援 JPEG、PNG、BMP 格式」，並清除已選擇的檔案及預覽縮圖（若存在）。
5. IF 使用者選擇的檔案大小超過 10 MB，THEN THE **ImgUploader** SHALL 顯示繁體中文錯誤訊息「檔案大小不得超過 10 MB」，並清除已選擇的檔案及預覽縮圖（若存在）。
6. WHEN 使用者選擇有效檔案，THE **ImgUploader** SHALL 在上傳區域顯示所選影像的預覽縮圖，縮圖尺寸不得小於 100×100 像素，且維持原始長寬比。
7. WHEN 使用者點擊「開始辨識」按鈕，THE **ImgUploader** SHALL 將已選擇的有效影像檔案以 `multipart/form-data` 格式送出，並在送出期間顯示載入狀態。

---

### 需求 2：執行 YOLO 物件偵測

**使用者故事：** 身為內部人員，我希望能夠點擊按鈕送出影像，以便取得 YOLO 物件偵測結果。

#### 驗收標準

1. WHEN 使用者已選擇有效影像，THE **ImgRead_Page** SHALL 啟用「開始辨識」按鈕。
2. WHILE 尚未選擇有效影像，THE **ImgRead_Page** SHALL 停用「開始辨識」按鈕，且按鈕呈現 disabled 視覺狀態。
3. WHEN 使用者點擊「開始辨識」按鈕，THE **Detect_Hook** SHALL 以 `multipart/form-data` 格式將影像檔案 POST 至 `/api/yolo/detect`，FormData 欄位名稱為 `image`。
4. WHILE YOLO_API 請求進行中，THE **ImgRead_Page** SHALL 在「開始辨識」按鈕上顯示 Ant Design `loading` 狀態，並停用該按鈕以防止重複送出。
5. IF YOLO_API 回傳 HTTP 狀態碼 4xx 或 5xx，THEN THE **ImgRead_Page** SHALL 以 Ant Design `message.error` 顯示繁體中文錯誤訊息「辨識失敗，請稍後再試」。
6. IF 網路連線中斷或請求逾時（axios 拋出 network error 或 timeout error），THEN THE **ImgRead_Page** SHALL 以 Ant Design `message.error` 顯示繁體中文錯誤訊息「網路錯誤，請確認連線後重試」。

---

### 需求 3：顯示偵測結果

**使用者故事：** 身為內部人員，我希望能夠看到 YOLO 偵測結果清單與執行時間，以便了解影像中偵測到的物件。

#### 驗收標準

1. WHEN YOLO_API 回傳成功結果且 `detections` 陣列不為空，THE **ImgRead_Page** SHALL 以 Ant Design `Table` 元件顯示 `YoloResponseDto.detections` 清單，欄位包含：類別標籤（`label`）、信心分數（`confidence`，顯示為百分比格式，保留小數點後兩位，例如「85.42%」）、邊界框座標（`x`、`y`、`width`、`height`，單位為像素整數）。
2. WHEN YOLO_API 回傳成功結果，THE **ImgRead_Page** SHALL 在結果區域顯示執行時間，格式為「辨識耗時：{elapsedMs} ms」。
3. WHEN YOLO_API 回傳的 `detections` 陣列為空，THE **ImgRead_Page** SHALL 顯示繁體中文提示訊息「未偵測到任何物件」，且不顯示物件總數行。
4. WHEN YOLO_API 回傳成功結果且 `detections` 陣列長度大於 0，THE **ImgRead_Page** SHALL 在結果表格上方顯示偵測到的物件總數，格式為「共偵測到 {n} 個物件」，其中 {n} 為 `detections.length`。
5. WHILE 尚未執行任何偵測，THE **ImgRead_Page** SHALL 不顯示結果區域（包含表格、執行時間與物件總數）。

---

### 需求 4：資料層封裝

**使用者故事：** 身為開發人員，我希望 YOLO API 呼叫被封裝為標準的 React Query mutation hook，以便元件能以一致的方式使用。

#### 驗收標準

1. THE **Detect_Hook** SHALL 位於 `src/libs/imgRead/imgRead.js`，並透過 `src/api/queries.js` 統一重新匯出。
2. THE **Detect_Hook** SHALL 使用 `axios.post()` 搭配 `process.env.REACT_APP_API_BASE_URL` 環境變數，不透過 `apiClient` 實例，目標路徑為 `/api/yolo/detect`。
3. THE **Detect_Hook** SHALL 以 react-query v3 的 `useMutation` 實作，mutation function 接受 `File` 物件並回傳 `YoloResponseDto`，其結構至少包含 `detections`（陣列，每筆含 `label: string`、`confidence: number`、`bbox: { x, y, width, height }`）與 `elapsedMs: number`。
4. WHEN `useMutation` 的 `mutate` 被呼叫，THE **Detect_Hook** SHALL 自動將 `File` 物件以欄位名稱 `image` 包裝為 `FormData`，並設定 `Content-Type: multipart/form-data` 標頭。
5. THE **Detect_Hook** SHALL 匯出 `useDetectImage` hook，其回傳值包含 `mutate`、`isLoading`、`data`、`error`、`reset` 屬性。
6. IF YOLO_API 回傳 HTTP 錯誤或網路錯誤，THEN THE **Detect_Hook** SHALL 將錯誤物件暴露於 `error` 屬性，供呼叫端元件判斷錯誤類型並顯示對應訊息。

---

### 需求 5：路由與導覽整合

**使用者故事：** 身為內部人員，我希望能夠透過側邊選單進入影像辨識頁面，以便快速存取此功能。

#### 驗收標準

1. THE **ImgRead_Page** SHALL 以路由路徑 `/img-read` 在 `src/routes/AppRoutes.js` 中完成路由定義，對應元件為 `src/pages/imgRead/ImgReadPage.js`。
2. THE **App** SHALL 在側邊導覽選單中新增「影像辨識」選單項目，`key` 值為 `/img-read`，對應路由路徑 `/img-read`。
3. WHEN 使用者點擊側邊選單的「影像辨識」項目，THE **App** SHALL 透過 `useNavigate` 導覽至 `/img-read` 頁面，並更新選單的 `selectedKeys` 狀態。

---

### 需求 6：元件 Props 型別驗證

**使用者故事：** 身為開發人員，我希望所有元件都有完整的 prop-types 宣告，以便在開發期間快速發現介面錯誤。

#### 驗收標準

1. THE **ImgUploader** SHALL 以 `prop-types` 宣告所有接收的 props；必填 props 加上 `.isRequired`；選填 props 須在 `ImgUploader.defaultProps` 中提供預設值。
2. IF **ImgRead_Page** 接收任何 props，THEN THE **ImgRead_Page** SHALL 以 `prop-types` 宣告對應的型別，必填 props 加上 `.isRequired`。
3. IF ImgRead 頁面內的任何子元件接收 props，THEN THE 子元件 SHALL 以 `prop-types` 宣告對應的型別，必填 props 加上 `.isRequired`。
4. WHEN 呼叫端傳入不符合宣告型別的 prop 值，THE 元件 SHALL 在瀏覽器開發者工具的 console 中產生 PropTypes 警告，使開發人員能在開發期間即時發現介面錯誤。
