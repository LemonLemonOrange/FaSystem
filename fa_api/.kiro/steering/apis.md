# API 文件

本文件描述本專案對外提供的 REST API 端點，以及所整合的第三方資料來源。

---

## 一、本專案提供的 API 端點

Base URL（開發）：`http://localhost:5000`

所有 API 回傳 `application/json`，快取時間依資料類型而異。

---

### 1. WaterGov — 水利署資料（`/api/watergov`）

#### 基本資料（快取 24 小時）

| 方法 | 路由 | 說明 |
|------|------|------|
| GET | `/api/watergov/basic/city` | 取得所有縣市清單 |
| GET | `/api/watergov/basic/{cityName}/town` | 取得指定縣市的鄉鎮清單（路徑參數為縣市名稱，例：`新北市`） |

#### 水庫資料（快取 10 分鐘）

| 方法 | 路由 | 說明 |
|------|------|------|
| GET | `/api/watergov/reservoir/station` | 水庫測站基本資料 |
| GET | `/api/watergov/reservoir/real-time-info` | 水庫即時水情資訊 |
| GET | `/api/watergov/reservoir/daily` | 水庫每日資料 |
| GET | `/api/watergov/reservoir/warning` | 水庫警示資料 |
| GET | `/api/watergov/reservoir/affected-area` | 水庫影響區域 |

#### 雨量資料（快取 10 分鐘）

| 方法 | 路由 | 說明 |
|------|------|------|
| GET | `/api/watergov/rain/station` | 雨量站基本資料 |
| GET | `/api/watergov/rain/real-time-info` | 雨量即時資訊 |
| GET | `/api/watergov/rain/warning` | 雨量警示資料 |
| GET | `/api/watergov/rain/affected-area` | 雨量影響區域 |

#### 供水資料（快取 10 分鐘）

| 方法 | 路由 | 說明 |
|------|------|------|
| GET | `/api/watergov/supply-condition` | 各地區供水狀況（正常/減壓/限水/停水） |

#### 水位站資料（快取 10 分鐘）

| 方法 | 路由 | 說明 |
|------|------|------|
| GET | `/api/watergov/water/station` | 水位站基本資料 |
| GET | `/api/watergov/water/real-time-info` | 水位即時資訊 |
| GET | `/api/watergov/water/warning` | 水位警示資料 |

#### 水文即時資料（快取 10 分鐘）

| 方法 | 路由 | 說明 |
|------|------|------|
| GET | `/api/watergov/water-level?stationNo={站號}` | 即時水位（`stationNo` 選填，不填回傳全部） |
| GET | `/api/watergov/rainfall?stationNo={站號}` | 即時雨量（`stationNo` 選填，不填回傳全部） |

#### 事件資料（快取 1 小時）

| 方法 | 路由 | 說明 |
|------|------|------|
| GET | `/api/watergov/event/{year}` | 指定年份的防汛事件清單（例：`/api/watergov/event/2026`） |

#### 統計資料（快取 30 分鐘）

| 方法 | 路由 | 說明 |
|------|------|------|
| GET | `/api/watergov/statistics/flooding/{eventNo}` | 指定事件的淹水統計 |
| GET | `/api/watergov/statistics/water-facility/{eventNo}` | 指定事件的水利設施損失統計 |
| GET | `/api/watergov/statistics/flood-defense-material` | 防汛資材統計 |

#### 災情明細（無快取，即時查詢）

| 方法 | 路由 | 說明 |
|------|------|------|
| GET | `/api/watergov/disaster/flooding/{eventNo}` | 指定事件的淹水災情明細 |
| GET | `/api/watergov/disaster/water-facility/{eventNo}` | 指定事件的水利設施災情明細 |

#### 防汛資材（快取 24 小時）

| 方法 | 路由 | 說明 |
|------|------|------|
| GET | `/api/watergov/flood-defense/material-location` | 防汛資材位置資料 |

---

### 2. NCDR — 枯旱預警（`/api/ncdr`）

| 方法 | 路由 | 說明 | 快取 |
|------|------|------|------|
| GET | `/api/ncdr/drought-alert` | 最新枯旱預警（含嚴重程度與影響縣市） | 10 分鐘 |

回傳結構：
```json
{
  "identifier": "WRA_Drought_20260427193326",
  "sent": "2026-04-27T19:33:26+08:00",
  "status": "Actual",
  "info": {
    "severity": "Minor | Moderate | Severe | Extreme",
    "headline": "...",
    "description": "...",
    "effective": "...",
    "expires": "...",
    "area": [{ "areaDesc": "台南市" }]
  }
}
```

無預警時回傳 `404 { "message": "目前無枯旱預警資料" }`

---

## 二、整合的第三方 API（水利署 WraApi）

- **Base URL**：`https://fhy.wra.gov.tw/WraApi/v1`
- **格式**：JSON（`JArray`）
- **認證**：無需 API Key
- **OData 查詢參數**：所有端點皆支援 `$filter`、`$select`、`$orderby`、`$top`（預設 30）、`$skip`

---

### Basic API

#### `GET /v1/Basic/City`
取得所有縣市資料

| 欄位 | 型別 | 說明 |
|------|------|------|
| CityCode | string | 縣市代碼 |
| CityName_Ch | string | 縣市名稱（中文）|
| CityName_En | string | 縣市名稱（英文）|

#### `GET /v1/Basic/{City}/Town`
取得該縣市鄉鎮資料（路徑參數為縣市名稱，例：`新北市`）

| 欄位 | 型別 | 說明 |
|------|------|------|
| TownCode | string | 鄉鎮代碼 |
| TownName | string | 鄉鎮名稱 |

---

### Disaster API

#### `GET /v1/Disaster/Flooding/{EventNo}`
取得某次事件內的淹水災情明細

| 欄位 | 型別 | 必填 | 說明 |
|------|------|------|------|
| DisasterFloodingID | integer | ✓ | 災情序號 |
| Time | string | ✓ | 通報時間（yyyy-MM-dd HH:mm）|
| SourceCode | string | ✓ | 災情來源：`1` 水利署、`2` EMIC、`3` 電視媒體、`4` 防汛護水志工、`5` 消防署、`6` CHT、`7` CCTV、`8` 其他 |
| SourceRemarks | string | | 來源說明 |
| SourceNo | string | | 資料來源序號 |
| OperatorName | string | | 災點分區 |
| CityCode | string | | 縣市代碼 |
| TownCode | string | | 鄉鎮代碼 |
| Situation | string | | 災情描述 |
| Location | string | | 災害地點 |
| Depth | number | | 淹水深度 |
| Treatment | string | | 災情處置情形 |
| IsReceded | boolean | | 是否退水 |
| RecededDate | string | | 退水時間（yyyy-MM-dd HH:mm）|
| Latitude | number | | 緯度（WGS84）|
| Longitude | number | | 經度（WGS84）|
| Type | string | | 災害種類：`0` 住戶、`1` 工(商)業區、`2` 農田/漁塭、`3` 道路、`4` 其他、`5` 待查 |

#### `GET /v1/Disaster/WaterFacility/{EventNo}`
取得某次事件內的水利設施災情明細

| 欄位 | 型別 | 必填 | 說明 |
|------|------|------|------|
| WaterFacilityID | integer | ✓ | 災情序號 |
| Time | string | ✓ | 通報時間（yyyy-MM-dd HH:mm）|
| OperatorName | string | | 災點分區 |
| CityCode | string | | 縣市代碼 |
| TownCode | string | | 鄉鎮代碼 |
| Situation | string | | 情況說明 |
| Treatment | string | | 處理說明 |
| Latitude | number | | 緯度（WGS84）|
| Longitude | number | | 經度（WGS84）|
| Type | string | | 災害類別：`1` 河堤、`2` 海堤、`3` 排水、`4` 水庫、`5` 水門、`6` 抽水站、`7` 其他 |

---

### Event API

#### `GET /v1/Event/Year/{Year}`
取得年度大雨、豪雨、颱風事件資料

| 欄位 | 型別 | 必填 | 說明 |
|------|------|------|------|
| EventNo | string | ✓ | 事件代碼 |
| EventName | string | ✓ | 事件名稱 |
| BeginTime | string | ✓ | 起始時間 |
| EndTime | string | | 結束時間 |
| IsActive | integer | ✓ | 是否成立 |

---

### FloodDefense API

#### `GET /v1/FloodDefense/MaterialLocation`
防汛備料場所

| 欄位 | 型別 | 必填 | 說明 |
|------|------|------|------|
| OperatorName | string | ✓ | 河川局 |
| CityCode | string | | 縣市代碼 |
| Type | string | ✓ | `1` 防汛倉庫、`2` 防汛堆置廠 |
| Watershed | string | | 水系 |
| River | string | | 河川 |
| Remarks | string | | 備註 |
| SrcUpdateTime | string | | 來源資料更新時間 |

---

### Rain API

#### `GET /v1/Rain/Station`
取得雨量站基本資料

| 欄位 | 型別 | 說明 |
|------|------|------|
| StationNo | string | 測站代碼 |
| StationName | string | 測站中文名稱 |
| CityCode | string | 縣市代碼 |
| BasinNo | string | 流域代碼 |
| BasinName | string | 流域名稱 |
| Address | string | 雨量站所在地址 |
| Latitude | number | 緯度（WGS84）|
| Longitude | number | 經度（WGS84）|

#### `GET /v1/Rain/RealTimeInfo`
取得雨量統計資料

| 欄位 | 型別 | 說明 |
|------|------|------|
| StationNo | string | 雨量站代碼 |
| Time | string | 水情時間（yyyy-MM-dd HH:mm）|
| M10 | number | 10 分鐘雨量（mm）|
| H1 | number | 1 小時累計雨量（mm）|
| H3 | number | 3 小時累計雨量（mm）|
| H6 | number | 6 小時累計雨量（mm）|
| H12 | number | 12 小時累計雨量（mm）|
| H24 | number | 24 小時累計雨量（mm）|

#### `GET /v1/Rain/Warning`
取得淹水警示資料

| 欄位 | 型別 | 說明 |
|------|------|------|
| StationNo | string | 測站代碼 |
| CityCode | string | 縣市代碼 |
| TownCode | string | 鄉鎮代碼 |
| Time | string | 水情時間（yyyy-MM-dd HH:mm）|
| M10 ~ H24 | number | 各時段累計雨量（mm）|
| WarningLevel | integer | 警戒級別 |
| AffectedArea | string | 影響範圍 |

#### `GET /v1/Rain/AffectedArea`
取得雨量警戒範圍

| 欄位 | 型別 | 說明 |
|------|------|------|
| StationNo | string | 測站代碼 |
| CityCode | string | 縣市代碼 |
| TownCode | string | 鄉鎮代碼 |
| AlertLevel2_H1/H3/H6 | number | 二級警戒各時段雨量門檻（mm）|
| AlertLevel1_H1/H3/H6 | number | 一級警戒各時段雨量門檻（mm）|
| AffectedArea | string | 影響範圍 |

---

### Reservoir API

#### `GET /v1/Reservoir/Station`
取得水庫基本資料

| 欄位 | 型別 | 必填 | 說明 |
|------|------|------|------|
| StationNo | string | ✓ | 測站代碼 |
| StationName | string | ✓ | 測站中文名稱 |
| CityCode | string | ✓ | 縣市代碼 |
| BasinNo | string | ✓ | 流域代碼 |
| BasinName | string | ✓ | 流域名稱 |
| Storage | number | ✓ | 總蓄水量（萬立方公尺）|
| ProtectionFlood | integer | ✓ | 是否涉及防洪（0：否；1：是）|
| HydraulicConstruction | integer | ✓ | 水工結構物種類（1：水庫及壩；2：攔河堰）|
| Importance | integer | ✓ | 重要性（1：主要；0：其他）|
| EffectiveCapacity | number | | 有效容量（萬立方公尺）|
| FullWaterHeight | number | | 滿水位標高（公尺）|
| DeadWaterHeight | number | | 呆水位標高/底床高（公尺）|
| Latitude | number | | 緯度（WGS84）|
| Longitude | number | | 經度（WGS84）|

#### `GET /v1/Reservoir/RealTimeInfo`
取得水庫即時資料

| 欄位 | 型別 | 必填 | 說明 |
|------|------|------|------|
| StationNo | string | ✓ | 測站代碼 |
| Time | string | ✓ | 水情時間（yyyy-MM-dd HH:mm）|
| WaterHeight | number | ✓ | 水位高（公尺）|
| AccumulatedRainfall | number | | 本日集水區累積降雨量（mm）|
| EffectiveCapacity | number | | 有效容量（萬立方公尺）|
| EffectiveStorage | number | | 有效蓄水量（萬立方公尺）|
| PercentageOfStorage | number | | 蓄水百分比 |
| OperationalStorage | number | | 取用水量 |
| Inflow | number | | 進流量（cms）|
| Outflow | number | | 水庫出流量（cms）|
| Status | string | | 放水狀態：`0` 預計放水、`1` 放水中、`-1` 未放水 |
| NextSpillTime | string | | 預計洩洪時間（yyyy-MM-dd HH:mm）|
| Discharge | number | | 放流量（cms）|
| DischargeOfProtectionFlood | number | | 防洪排放流量（cms）|
| DischargeOfEscapeSand | number | | 排砂道放流量（cms）|
| DischargeOfHydroelectric | number | | 發電放流量（cms）|
| DischargeOfOthers | number | | 其他放流量（cms）|

#### `GET /v1/Reservoir/Daily`
取得水庫統計資料

| 欄位 | 型別 | 必填 | 說明 |
|------|------|------|------|
| StationNo | string | ✓ | 測站代碼 |
| Time | string | ✓ | 水情時間（yyyy-MM-dd HH:mm）|
| EffectiveCapacity | number | | 有效容量（萬立方公尺）|
| DeadWaterHeight | number | | 呆水位標高/底床高（公尺）|
| FullWaterHeight | number | | 滿水位標高（公尺）|
| AccumulatedRainfall | number | | 集水區本日降雨量（mm）|
| InflowTotal | number | | 本日總進水量（萬立方公尺）|
| OutflowTotal | number | | 本日總出水量（萬立方公尺）|

#### `GET /v1/Reservoir/Warning`
取得水庫警示資料

| 欄位 | 型別 | 必填 | 說明 |
|------|------|------|------|
| StationNo | string | ✓ | 測站代碼 |
| Time | string | ✓ | 水情時間（yyyy-MM-dd HH:mm）|
| Status | string | ✓ | 放水狀態：`0` 預計放水、`1` 放水中、`-1` 未放水 |
| CityCode | string | | 縣市代碼 |
| TownCode | string | | 鄉鎮代碼 |
| WaterHeight | number | | 水位高（公尺）|
| DischargeOfProtectionFlood | number | | 防洪排放流量（cms）|
| NextSpillTime | string | | 預計放水時間（yyyy-MM-dd HH:mm）|
| Discharge | number | | 放流量 |

#### `GET /v1/Reservoir/AffectedArea`
取得水庫警戒範圍

| 欄位 | 型別 | 說明 |
|------|------|------|
| StationNo | string | 水庫代碼 |
| CityCode | string | 縣市代碼 |
| TownCode | string | 鄉鎮代碼 |

---

### Statistics API

#### `GET /v1/Statistics/Flooding/{EventNo}`
取得某次事件內的淹水災情統計

| 欄位 | 型別 | 說明 |
|------|------|------|
| CityCode | string | 縣市代碼 |
| TownCount | integer | 目前鄉鎮數 |
| RecededCount | integer | 已退水（處）|
| FloodingCount | integer | 未退水（處）|
| Total | integer | 合計災害數 |

#### `GET /v1/Statistics/WaterFacility/{EventNo}`
取得某次事件內的水利設施統計

| 欄位 | 型別 | 說明 |
|------|------|------|
| CityCode | string | 縣市代碼 |
| RepairedCount | integer | 搶修完成數量（處）|
| RepairingCount | integer | 搶修中數量（處）|
| Total | integer | 受損總數量（處）|

#### `GET /v1/Statistics/FloodDefenseMaterial`
防汛備料統計

| 欄位 | 型別 | 說明 |
|------|------|------|
| OperatorName | string | 河川局 |
| Material[].Name | string | 防汛備料名稱 |
| Material[].Total | number | 總數 |

---

### Water API

#### `GET /v1/Water/Station`
取得水位站基本資料

| 欄位 | 型別 | 必填 | 說明 |
|------|------|------|------|
| StationNo | string | ✓ | 測站代碼 |
| StationName | string | ✓ | 測站中文名稱 |
| CityCode | string | ✓ | 縣市代碼 |
| BasinNo | string | ✓ | 流域代碼 |
| BasinName | string | ✓ | 流域名稱 |
| Address | string | | 水位站所在地址 |
| WarningLevel1 | number | | 一級警戒值（公尺）|
| WarningLevel2 | number | | 二級警戒值（公尺）|
| WarningLevel3 | number | | 三級警戒值（公尺）|
| TopLevel | number | | 水位堤頂高（公尺）|
| PlanFloodLevel | number | | 計畫洪水位（公尺）|
| Latitude | number | | 緯度（WGS84）|
| Longitude | number | | 經度（WGS84）|

#### `GET /v1/Water/RealTimeInfo`
取得水位即時資料

| 欄位 | 型別 | 說明 |
|------|------|------|
| StationNo | string | 水位站站碼 |
| Time | string | 水情時間（yyyy-MM-dd HH:mm）|
| WaterLevel | number | 水位高（公尺）|

#### `GET /v1/Water/Warning`
取得水位警示資料

| 欄位 | 型別 | 說明 |
|------|------|------|
| StationNo | string | 測站代碼 |
| CityCode | string | 縣市代碼 |
| TownCode | string | 鄉鎮代碼 |
| Time | string | 水情時間（yyyy-MM-dd HH:mm）|
| WaterLevel | number | 水位高（公尺）|
| WarningLevel | integer | 警戒級別 |

---

## 三、整合的第三方 API（NCDR）

- **Feed URL**：`https://alerts.ncdr.nat.gov.tw/webapi/JSONAtomFeed.ashx?AlertType=2099`
- **格式**：JSON（Atom Feed），再依 `entry[].link["@href"]` 下載 CAP XML 檔案
- **認證**：無需 API Key
- **CAP 標準**：CAP 1.2（`urn:oasis:names:tc:emergency:cap:1.2`）
- **AlertType 2099**：枯旱預警（已過濾）

**解析流程**：
1. GET Feed → 取得 JSON，解析 `entry[]`
2. 依 `updated` 欄位排序，取最新一筆
3. 從 `entry.link["@href"]` 取得 `.cap` 檔案 URL
4. GET `.cap` 檔案 → 解析 XML
5. 擷取 `identifier`、`sent`、`status`、`info`（含 `severity`、`headline`、`description`、`area[]`）

**Severity 對應**：
| 值 | 供水狀態 |
|----|---------|
| `Minor` | 正常供水 |
| `Moderate` | 減壓供水 |
| `Severe` | 限量供水 |
| `Extreme` | 停止供水或緊急措施 |
