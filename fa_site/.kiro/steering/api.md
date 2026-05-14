# API 架構文件

所有 API 呼叫皆透過本地 .NET 後端，Base URL 由環境變數 `REACT_APP_API_BASE_URL` 決定，預設為 `http://localhost:65326`。

---

## 一、WraGov — 水利署資料（`src/libs/WraGov/`）

後端路徑前綴：`/api/watergov/`

---

### 1. 基本資料（`basic.js`）

#### `fetchCity(params?)`
- **GET** `/api/watergov/basic/city`
- **輸入：** 選填 query params
- **輸出：** `Array<City>`

| 欄位 | 型別 | 說明 |
|---|---|---|
| `CityCode` | string | 縣市代碼 |
| `CityName_Ch` | string | 縣市名稱（中文） |
| `CityName_En` | string | 縣市名稱（英文） |

#### `fetchTown(city, params?)`
- **GET** `/api/watergov/basic/{city}/town`
- **輸入：** `city` — 縣市名稱（例：臺北市）；選填 query params
- **輸出：** `Array<Town>`

| 欄位 | 型別 | 說明 |
|---|---|---|
| `TownCode` | string | 鄉鎮代碼 |
| `TownName` | string | 鄉鎮名稱 |

**React Query Hooks：**
```js
useCity(params?, options?)                    // queryKey: ['wraCity', params]
useTown(city, params?, options?)              // queryKey: ['wraTown', city, params]，city 為空時不執行
```

---

### 2. 水庫資料（`reservoir.js`）

#### `fetchReservoirStation(params?)`
- **GET** `/api/watergov/reservoir/station`
- **輸出：** `Array<ReservoirStation>`

| 欄位 | 型別 | 說明 |
|---|---|---|
| `StationNo` | string | 測站代碼 |
| `StationName` | string | 測站名稱 |
| `CityCode` | string | 縣市代碼 |
| `BasinNo` | string | 流域代碼 |
| `BasinName` | string | 流域名稱 |
| `EffectiveCapacity` | number? | 有效容量 |
| `FullWaterHeight` | number? | 滿水位（公尺） |
| `DeadWaterHeight` | number? | 呆水位（公尺） |
| `Storage` | number | 總蓄水量 |
| `Latitude` | number? | 緯度 (WGS84) |
| `Longitude` | number? | 經度 (WGS84) |
| `ProtectionFlood` | number | 是否具防洪功能（0:否, 1:是） |
| `HydraulicConstruction` | number | 水工構造類型（1:水庫, 2:攔河堰） |
| `Importance` | number | 重要性（1:主要, 0:次要） |

#### `fetchReservoirRealTimeInfo(params?)`
- **GET** `/api/watergov/reservoir/real-time-info`
- **輸出：** `Array<ReservoirRealTimeInfo>`

| 欄位 | 型別 | 說明 |
|---|---|---|
| `StationNo` | string | 測站代碼 |
| `Time` | string | 資料時間（yyyy-MM-dd HH:mm） |
| `WaterHeight` | number | 水位（公尺） |
| `EffectiveCapacity` | number? | 有效容量 |
| `EffectiveStorage` | number? | 有效蓄水量 |
| `PercentageOfStorage` | number? | 蓄水率 |
| `OperationalStorage` | number? | 可用水量 |
| `AccumulatedRainfall` | number? | 當日累積雨量(mm) |
| `Inflow` | number? | 入流量(cms) |
| `Outflow` | number? | 出流量(cms) |
| `Discharge` | number? | 放流量(cms) |
| `DischargeOfProtectionFlood` | number? | 防洪放流量(cms) |
| `DischargeOfEscapeSand` | number? | 排砂放流量(cms) |
| `DischargeOfHydroelectric` | number? | 發電放流量(cms) |
| `DischargeOfOthers` | number? | 其他放流量(cms) |
| `Status` | string? | 狀態代碼（'0':蓄水, '1':洩水, '-1':排放） |
| `NextSpillTime` | string? | 預計洩洪時間（yyyy-MM-dd HH:mm） |

#### `fetchReservoirDaily(params?)`
- **GET** `/api/watergov/reservoir/daily`
- **輸出：** `Array<ReservoirDaily>`

| 欄位 | 型別 | 說明 |
|---|---|---|
| `StationNo` | string | 測站代碼 |
| `Time` | string | 資料時間（yyyy-MM-dd HH:mm） |
| `EffectiveCapacity` | number? | 有效容量 |
| `DeadWaterHeight` | number? | 呆水位（公尺） |
| `FullWaterHeight` | number? | 滿水位（公尺） |
| `AccumulatedRainfall` | number? | 當日累積雨量(mm) |
| `InflowTotal` | number? | 當日總進水量 |
| `OutflowTotal` | number? | 當日總出水量 |

#### `fetchReservoirWarning(params?)`
- **GET** `/api/watergov/reservoir/warning`
- **輸出：** `Array<ReservoirWarning>`

| 欄位 | 型別 | 說明 |
|---|---|---|
| `StationNo` | string | 測站代碼 |
| `CityCode` | string? | 縣市代碼 |
| `TownCode` | string? | 鄉鎮代碼 |
| `Time` | string | 資料時間（yyyy-MM-dd HH:mm） |
| `WaterHeight` | number? | 水位（公尺） |
| `DischargeOfProtectionFlood` | number? | 防洪放流量(cms) |
| `Discharge` | number? | 放流量(cms) |
| `NextSpillTime` | string? | 預計放流時間（yyyy-MM-dd HH:mm） |
| `Status` | string | 狀態代碼（'0':蓄水, '1':洩水, '-1':排放） |

#### `fetchReservoirAffectedArea(params?)`
- **GET** `/api/watergov/reservoir/affected-area`
- **輸出：** `Array<ReservoirAffectedArea>`

| 欄位 | 型別 | 說明 |
|---|---|---|
| `StationNo` | string | 水庫代碼 |
| `CityCode` | string | 縣市代碼 |
| `TownCode` | string | 鄉鎮代碼 |

**React Query Hooks：**
```js
useReservoirStation(params?, options?)        // queryKey: ['wraReservoirStation', params]
useReservoirRealTimeInfo(params?, options?)   // queryKey: ['wraReservoirRealTimeInfo', params]
useReservoirDaily(params?, options?)          // queryKey: ['wraReservoirDaily', params]
useReservoirWarning(params?, options?)        // queryKey: ['wraReservoirWarning', params]
useReservoirAffectedArea(params?, options?)   // queryKey: ['wraReservoirAffectedArea', params]
```

---

### 3. 雨量資料（`rain.js`）

#### `fetchRainStation(params?)`
- **GET** `/api/watergov/rain/station`（透過 wraApi proxy）
- **輸出：** `Array<RainStation>`

| 欄位 | 型別 | 說明 |
|---|---|---|
| `StationNo` | string | 測站代碼 |
| `StationName` | string | 測站名稱 |
| `CityCode` | string | 縣市代碼 |
| `BasinNo` | string | 流域代碼 |
| `BasinName` | string | 流域名稱 |
| `Address` | string | 測站所在地 |
| `Latitude` | number | 緯度 (WGS84) |
| `Longitude` | number | 經度 (WGS84) |

#### `fetchRainRealTimeInfo(params?)`
- **輸出：** `Array<RainRealTimeInfo>`

| 欄位 | 型別 | 說明 |
|---|---|---|
| `StationNo` | string | 測站代碼 |
| `Time` | string | 資料時間（yyyy-MM-dd HH:mm） |
| `M10` | number | 10 分鐘累積雨量(mm) |
| `H1` | number | 1 小時累積雨量(mm) |
| `H3` | number | 3 小時累積雨量(mm) |
| `H6` | number | 6 小時累積雨量(mm) |
| `H12` | number | 12 小時累積雨量(mm) |
| `H24` | number | 24 小時累積雨量(mm) |

#### `fetchRainWarning(params?)`
- **輸出：** `Array<RainWarning>`（同 RainRealTimeInfo 加上 `CityCode`、`TownCode`、`WarningLevel`、`AffectedArea`）

#### `fetchRainAffectedArea(params?)`
- **輸出：** `Array<RainAffectedArea>`

| 欄位 | 型別 | 說明 |
|---|---|---|
| `StationNo` | string | 測站代碼 |
| `CityCode` | string | 縣市代碼 |
| `TownCode` | string | 鄉鎮代碼 |
| `AlertLevel2_H1/H3/H6` | number | 二級警戒各時段雨量門檻(mm) |
| `AlertLevel1_H1/H3/H6` | number | 一級警戒各時段雨量門檻(mm) |
| `AffectedArea` | string | 影響範圍 |

**React Query Hooks：**
```js
useRainStation(params?, options?)             // queryKey: ['wraRainStation', params]
useRainRealTimeInfo(params?, options?)        // queryKey: ['wraRainRealTimeInfo', params]
useRainWarning(params?, options?)             // queryKey: ['wraRainWarning', params]
useRainAffectedArea(params?, options?)        // queryKey: ['wraRainAffectedArea', params]
```

---

### 4. 水位資料（`water.js`）

#### `fetchWaterStation(params?)`
- **GET** `/api/watergov/water/station`
- **輸出：** `Array<WaterStation>`

| 欄位 | 型別 | 說明 |
|---|---|---|
| `StationNo` | string | 測站代碼 |
| `StationName` | string | 測站名稱 |
| `CityCode` | string | 縣市代碼 |
| `BasinNo` | string | 流域代碼 |
| `BasinName` | string | 流域名稱 |
| `Address` | string? | 測站所在地 |
| `Latitude` | number? | 緯度 (WGS84) |
| `Longitude` | number? | 經度 (WGS84) |
| `WarningLevel1` | number? | 一級警戒水位（公尺） |
| `WarningLevel2` | number? | 二級警戒水位（公尺） |
| `WarningLevel3` | number? | 三級警戒水位（公尺） |
| `TopLevel` | number? | 最高水位（公尺） |
| `PlanFloodLevel` | number? | 計畫洪水位（公尺） |

#### `fetchWaterRealTimeInfo(params?)`
- **GET** `/api/watergov/water/real-time-info`
- **輸出：** `Array<WaterRealTimeInfo>`

| 欄位 | 型別 | 說明 |
|---|---|---|
| `StationNo` | string | 水位站代碼 |
| `Time` | string | 資料時間（yyyy-MM-dd HH:mm） |
| `WaterLevel` | number | 水位（公尺） |

#### `fetchWaterWarning(params?)`
- **GET** `/api/watergov/water/warning`
- **輸出：** `Array<WaterWarning>`

| 欄位 | 型別 | 說明 |
|---|---|---|
| `StationNo` | string | 測站代碼 |
| `CityCode` | string | 縣市代碼 |
| `TownCode` | string | 鄉鎮代碼 |
| `Time` | string | 資料時間（yyyy-MM-dd HH:mm） |
| `WaterLevel` | number | 水位（公尺） |
| `WarningLevel` | number | 警戒級別 |

**React Query Hooks：**
```js
useWaterStation(params?, options?)            // queryKey: ['wraWaterStation', params]
useWaterRealTimeInfo(params?, options?)       // queryKey: ['wraWaterRealTimeInfo', params]
useWaterWarning(params?, options?)            // queryKey: ['wraWaterWarning', params]
```

---

### 5. 防汛資材（`floodDefense.js`）

#### `fetchFloodDefenseMaterialLocation(params?)`
- **GET** `/api/watergov/flood-defense/material-location`
- **輸出：** `Array<MaterialLocation>`

| 欄位 | 型別 | 說明 |
|---|---|---|
| `OperatorName` | string | 河川分署 |
| `CityCode` | string? | 縣市代碼 |
| `Type` | string | 資材類型（'1':防汛倉庫, '2':防汛設置點） |
| `Watershed` | string? | 水系 |
| `River` | string? | 河川 |
| `Remarks` | string? | 備註 |
| `SrcUpdateTime` | string? | 來源資料更新時間 |

**React Query Hooks：**
```js
useFloodDefenseMaterialLocation(params?, options?)  // queryKey: ['wraFloodDefenseMaterialLocation', params]
```

---

### 6. 統計資料（`statistics.js`）

#### `fetchStatisticsFlooding(eventNo)`
- **GET** `/api/watergov/statistics/flooding/{eventNo}`
- **輸入：** `eventNo` — 事件編號（必填）
- **輸出：** `Array<DisasterFloodingStatistics>`

| 欄位 | 型別 | 說明 |
|---|---|---|
| `CityCode` | string? | 縣市代碼 |
| `TownCount` | number? | 受影響鄉鎮數 |
| `RecededCount` | number? | 已退水數 |
| `FloodingCount` | number? | 未退水數 |
| `Total` | number? | 災害總數 |

#### `fetchStatisticsWaterFacility(eventNo)`
- **GET** `/api/watergov/statistics/water-facility/{eventNo}`
- **輸入：** `eventNo` — 事件編號（必填）
- **輸出：** `Array<DisasterWaterFacilityStatistics>`

| 欄位 | 型別 | 說明 |
|---|---|---|
| `CityCode` | string? | 縣市代碼 |
| `RepairedCount` | number? | 已修復數量 |
| `RepairingCount` | number? | 搶修中數量 |
| `Total` | number? | 總數量 |

#### `fetchStatisticsFloodDefenseMaterial(params?)`
- **GET** `/api/watergov/statistics/flood-defense-material`
- **輸出：** `Array<FloodDefenseOperator>`

| 欄位 | 型別 | 說明 |
|---|---|---|
| `OperatorName` | string? | 河川分署 |
| `Material` | Array? | 防汛資材清單（含 `Name`、`Total`） |

**React Query Hooks：**
```js
useStatisticsFlooding(eventNo, options?)              // queryKey: ['wraStatisticsFlooding', eventNo]，eventNo 為空時不執行
useStatisticsWaterFacility(eventNo, options?)         // queryKey: ['wraStatisticsWaterFacility', eventNo]，eventNo 為空時不執行
useStatisticsFloodDefenseMaterial(params?, options?)  // queryKey: ['wraStatisticsFloodDefenseMaterial', params]
```

---

### 7. 事件資料（`event.js`）

#### `fetchEventByYear(year, params?)`
- **輸入：** `year` — 西元年份（必填）
- **輸出：** `Array<Event>`

| 欄位 | 型別 | 說明 |
|---|---|---|
| `EventNo` | string | 事件編號 |
| `EventName` | string | 事件名稱 |
| `BeginTime` | string | 開始時間 |
| `EndTime` | string? | 結束時間 |
| `IsActive` | number | 是否進行中（1:是, 0:否） |

**React Query Hooks：**
```js
useEventByYear(year, params?, options?)       // queryKey: ['wraEvent', year, params]，year 為空時不執行
```

---

## 二、NCDR — 國家災害防救科技中心（`src/libs/Ncdr/`）

後端路徑前綴：`/api/ncdr/`

---

### 1. 乾旱警示（`disaster.js`）

#### `fetchDroughtAlert()`（內部函式）
- **GET** `/api/ncdr/drought-alert`
- **輸入：** 無
- **輸出：** `DroughtAlert | null`（404 時回傳 null，表示目前無警示）
- **快取策略：** staleTime 10 分鐘（後端已有快取）

**React Query Hooks：**
```js
useDroughtAlert(options?)                     // queryKey: ['droughtAlert']
```

---

## 三、通用說明

### Query Params 通用格式
部分 API 支援 OData 風格的 query params（來自 WRA 原始 API 規格）：

| 參數 | 說明 |
|---|---|
| `$filter` | 過濾條件 |
| `$select` | 指定回傳欄位 |
| `$orderby` | 排序 |
| `$top` | 取前 N 筆（預設 30） |
| `$skip` | 跳過前 N 筆 |

### Hook 使用慣例
- 所有 hooks 的第二個參數為 React Query `options`，可傳入 `enabled`、`staleTime`、`onSuccess` 等
- 部分 hooks 在必填參數為空時會自動設定 `enabled: false`，不發出請求
- 所有 hooks 透過 `src/api/queries.js` 統一匯出，元件應從此處 import
