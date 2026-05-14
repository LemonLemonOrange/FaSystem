import { useQuery } from 'react-query';
import axios from 'axios';

const API_BASE_URL = process.env.REACT_APP_API_BASE_URL || 'http://localhost:65326';

/**
 * @typedef {Object} WaterStation
 * @property {string} [Address] - 水位站所在地
 * @property {string} CityCode - 縣市代碼
 * @property {number} [WarningLevel1] - 一級警戒水位（公尺）
 * @property {number} [WarningLevel2] - 二級警戒水位（公尺）
 * @property {number} [WarningLevel3] - 三級警戒水位（公尺）
 * @property {number} [TopLevel] - 最高水位（公尺）
 * @property {number} [Latitude] - 緯度(WGS84)
 * @property {number} [Longitude] - 經度(WGS84)
 * @property {number} [PlanFloodLevel] - 計畫洪水位（公尺）
 * @property {string} StationNo - 測站代碼
 * @property {string} StationName - 測站名稱
 * @property {string} BasinNo - 流域代碼
 * @property {string} BasinName - 流域名稱
 */

/**
 * @typedef {Object} WaterRealTimeInfo
 * @property {string} StationNo - 水位站代碼
 * @property {string} Time - 資料時間（格式：yyyy-MM-dd HH:mm）
 * @property {number} WaterLevel - 水位（公尺）
 */

/**
 * @typedef {Object} WaterWarning
 * @property {string} StationNo - 測站代碼
 * @property {string} CityCode - 縣市代碼
 * @property {string} TownCode - 鄉鎮代碼
 * @property {string} Time - 資料時間（格式：yyyy-MM-dd HH:mm）
 * @property {number} WaterLevel - 水位（公尺）
 * @property {number} WarningLevel - 警戒級別
 */

/**
 * 取得水位站基本資料
 * @param {object} [params]
 * @returns {Promise<Array<WaterStation>>} 水位站基本資料陣列
 */
export const fetchWaterStation = async (params) => {
  const response = await axios.get(`${API_BASE_URL}/api/watergov/water/station`, { params });
  return response.data;
};

/**
 * 取得水位即時資訊
 * @param {object} [params]
 * @returns {Promise<Array<WaterRealTimeInfo>>} 水位即時資訊資料陣列
 */
export const fetchWaterRealTimeInfo = async (params) => {
  const response = await axios.get(`${API_BASE_URL}/api/watergov/water/real-time-info`, { params });
  return response.data;
};

/**
 * 取得水位警示資料
 * @param {object} [params]
 * @returns {Promise<Array<WaterWarning>>} 水位警示資料陣列
 */
export const fetchWaterWarning = async (params) => {
  const response = await axios.get(`${API_BASE_URL}/api/watergov/water/warning`, { params });
  return response.data;
};

export const useWaterStation = (params, options) => useQuery({ queryKey: ['wraWaterStation', params], queryFn: () => fetchWaterStation(params), ...options });
export const useWaterRealTimeInfo = (params, options) => useQuery({ queryKey: ['wraWaterRealTimeInfo', params], queryFn: () => fetchWaterRealTimeInfo(params), ...options });
export const useWaterWarning = (params, options) => useQuery({ queryKey: ['wraWaterWarning', params], queryFn: () => fetchWaterWarning(params), ...options });
