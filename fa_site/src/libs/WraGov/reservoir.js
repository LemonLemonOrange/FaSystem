import { useQuery } from 'react-query';
import axios from 'axios';

    const API_BASE_URL = process.env.REACT_APP_API_BASE_URL || 'http://localhost:65326';

/**
 * @typedef {Object} ReservoirStation
 * @property {string} CityCode - 縣市代碼
 * @property {number} [EffectiveCapacity] - 有效容量
 * @property {number} [FullWaterHeight] - 滿水位（公尺）
 * @property {number} [DeadWaterHeight] - 呆水位（公尺）
 * @property {number} [Latitude] - 緯度(WGS84)
 * @property {number} [Longitude] - 經度(WGS84)
 * @property {number} Storage - 總蓄水量
 * @property {number} ProtectionFlood - 是否具防洪功能（0:否, 1:是）
 * @property {number} HydraulicConstruction - 水工構造類型（1:水庫, 2:攔河堰）
 * @property {number} Importance - 水庫壩堰重要性（1:主要, 0:次要）
 * @property {string} StationNo - 測站代碼
 * @property {string} StationName - 測站名稱
 * @property {string} BasinNo - 流域代碼
 * @property {string} BasinName - 流域名稱
 */

/**
 * @typedef {Object} ReservoirRealTimeInfo
 * @property {string} StationNo - 測站代碼
 * @property {string} Time - 資料時間（格式：yyyy-MM-dd HH:mm）
 * @property {number} [AccumulatedRainfall] - 當日累積雨量(mm)
 * @property {number} WaterHeight - 水位（公尺）
 * @property {number} [EffectiveCapacity] - 有效容量
 * @property {number} [EffectiveStorage] - 有效蓄水量
 * @property {number} [PercentageOfStorage] - 蓄水率
 * @property {number} [OperationalStorage] - 可用水量
 * @property {number} [Inflow] - 入流量(cms)
 * @property {number} [Outflow] - 出流量(cms)
 * @property {string} [Status] - 水庫狀態代碼 = ['0: 蓄水', '1: 洩水', '-1: 排放']
 * @property {string} [NextSpillTime] - 預計洩洪時間（格式：yyyy-MM-dd HH:mm）
 * @property {number} [Discharge] - 放流量(cms)
 * @property {number} [DischargeOfProtectionFlood] - 防洪放流量(cms)
 * @property {number} [DischargeOfEscapeSand] - 排砂放流量(cms)
 * @property {number} [DischargeOfHydroelectric] - 發電放流量(cms)
 * @property {number} [DischargeOfOthers] - 其他放流量(cms)
 */

/**
 * @typedef {Object} ReservoirDaily
 * @property {string} StationNo - 測站代碼
 * @property {string} Time - 資料時間（格式：yyyy-MM-dd HH:mm）
 * @property {number} [EffectiveCapacity] - 有效容量
 * @property {number} [DeadWaterHeight] - 呆水位（公尺）
 * @property {number} [FullWaterHeight] - 滿水位（公尺）
 * @property {number} [AccumulatedRainfall] - 當日累積雨量(mm)
 * @property {number} [InflowTotal] - 當日總進水量
 * @property {number} [OutflowTotal] - 當日總出水量
 */

/**
 * @typedef {Object} ReservoirWarning
 * @property {string} StationNo - 測站代碼
 * @property {string} [CityCode] - 縣市代碼
 * @property {string} [TownCode] - 鄉鎮代碼
 * @property {string} Time - 資料時間（格式：yyyy-MM-dd HH:mm）
 * @property {number} [WaterHeight] - 水位（公尺）
 * @property {number} [DischargeOfProtectionFlood] - 防洪放流量(cms)
 * @property {string} [NextSpillTime] - 預計放流時間（格式：yyyy-MM-dd HH:mm）
 * @property {number} [Discharge] - 放流量(cms)
 * @property {string} Status - 水庫狀態代碼 = ['0: 蓄水', '1: 洩水', '-1: 排放']
 */

/**
 * @typedef {Object} ReservoirAffectedArea
 * @property {string} StationNo - 水庫代碼
 * @property {string} CityCode - 縣市代碼
 * @property {string} TownCode - 鄉鎮代碼
 */

/**
 * 取得水庫基本資料
 * @param {object} [params]
 * @returns {Promise<Array<ReservoirStation>>} 水庫基本資料陣列
 */
export const fetchReservoirStation = async (params) => {
  const response = await axios.get(`${API_BASE_URL}/api/watergov/reservoir/station`, { params });
  return response.data;
};

/**
 * 取得水庫即時資訊
 * @param {object} [params]
 * @returns {Promise<Array<ReservoirRealTimeInfo>>} 水庫即時資訊資料陣列
 */
export const fetchReservoirRealTimeInfo = async (params) => {
  const response = await axios.get(`${API_BASE_URL}/api/watergov/reservoir/real-time-info`, { params });
  return response.data;
};

/**
 * 取得水庫統計資料
 * @param {object} [params]
 * @returns {Promise<Array<ReservoirDaily>>} 水庫統計資料陣列
 */
export const fetchReservoirDaily = async (params) => {
  const response = await axios.get(`${API_BASE_URL}/api/watergov/reservoir/daily`, { params });
  return response.data;
};

/**
 * 取得水庫警示資料
 * @param {object} [params]
 * @returns {Promise<Array<ReservoirWarning>>} 水庫警示資料陣列
 */
export const fetchReservoirWarning = async (params) => {
  const response = await axios.get(`${API_BASE_URL}/api/watergov/reservoir/warning`, { params });
  return response.data;
};

/**
 * 取得水庫警戒影響範圍
 * @param {object} [params]
 * @returns {Promise<Array<ReservoirAffectedArea>>} 水庫警戒影響範圍資料陣列
 */
export const fetchReservoirAffectedArea = async (params) => {
  const response = await axios.get(`${API_BASE_URL}/api/watergov/reservoir/affected-area`, { params });
  return response.data;
};

export const useReservoirStation = (params, options) => useQuery({ queryKey: ['wraReservoirStation', params], queryFn: () => fetchReservoirStation(params), ...options });
export const useReservoirRealTimeInfo = (params, options) => useQuery({ queryKey: ['wraReservoirRealTimeInfo', params], queryFn: () => fetchReservoirRealTimeInfo(params), ...options });
export const useReservoirDaily = (params, options) => useQuery({ queryKey: ['wraReservoirDaily', params], queryFn: () => fetchReservoirDaily(params), ...options });
export const useReservoirWarning = (params, options) => useQuery({ queryKey: ['wraReservoirWarning', params], queryFn: () => fetchReservoirWarning(params), ...options });
export const useReservoirAffectedArea = (params, options) => useQuery({ queryKey: ['wraReservoirAffectedArea', params], queryFn: () => fetchReservoirAffectedArea(params), ...options });
