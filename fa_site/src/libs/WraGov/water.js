import { useQuery } from 'react-query';
import { apiClient } from '../api/request';


/**
 * @typedef {Object} WaterStation
 * @property {string} [Address] - 水�?站�??�地
 * @property {string} CityCode - �??�?��
 * @property {number} [WarningLevel1] - 一級警?�水位�??�尺�? * @property {number} [WarningLevel2] - 二�?警�?水�?（公尺�?
 * @property {number} [WarningLevel3] - 三�?警�?水�?（公尺�?
 * @property {number} [TopLevel] - ?�高水位�??�尺�? * @property {number} [Latitude] - 緯度(WGS84)
 * @property {number} [Longitude] - 經度(WGS84)
 * @property {number} [PlanFloodLevel] - 計畫洪水位�??�尺�? * @property {string} StationNo - 測�?�?��
 * @property {string} StationName - 測�??�稱
 * @property {string} BasinNo - 流�?�?��
 * @property {string} BasinName - 流�??�稱
 */

/**
 * @typedef {Object} WaterRealTimeInfo
 * @property {string} StationNo - 水�?站代�? * @property {string} Time - 資�??��?（格式�?yyyy-MM-dd HH:mm�? * @property {number} WaterLevel - 水�?（公尺�?
 */

/**
 * @typedef {Object} WaterWarning
 * @property {string} StationNo - 測�?�?��
 * @property {string} CityCode - �??�?��
 * @property {string} TownCode - ?�鎮�?��
 * @property {string} Time - 資�??��?（格式�?yyyy-MM-dd HH:mm�? * @property {number} WaterLevel - 水�?（公尺�?
 * @property {number} WarningLevel - 警�?級別
 */

/**
 * ?��?水�?站基?��??? * @param {object} [params]
 * @returns {Promise<Array<WaterStation>>} 水�?站基?��??�陣?? */
export const fetchWaterStation = (params) => apiClient.get(`/api/watergov/water/station`, { params });

/**
 * ?��?水�??��?資�?
 * @param {object} [params]
 * @returns {Promise<Array<WaterRealTimeInfo>>} 水�??��?資�?資�????
 */
export const fetchWaterRealTimeInfo = (params) => apiClient.get(`/api/watergov/water/real-time-info`, { params });

/**
 * ?��?水�?警示資�?
 * @param {object} [params]
 * @returns {Promise<Array<WaterWarning>>} 水�?警示資�????
 */
export const fetchWaterWarning = (params) => apiClient.get(`/api/watergov/water/warning`, { params });

export const useWaterStation = (params, options) => useQuery({ queryKey: ['wraWaterStation', params], queryFn: () => fetchWaterStation(params), ...options });
export const useWaterRealTimeInfo = (params, options) => useQuery({ queryKey: ['wraWaterRealTimeInfo', params], queryFn: () => fetchWaterRealTimeInfo(params), ...options });
export const useWaterWarning = (params, options) => useQuery({ queryKey: ['wraWaterWarning', params], queryFn: () => fetchWaterWarning(params), ...options });
