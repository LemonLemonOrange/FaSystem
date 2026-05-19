import { useQuery } from 'react-query';
import { apiClient } from '../api/request';

    
/**
 * @typedef {Object} ReservoirStation
 * @property {string} CityCode - �??�?��
 * @property {number} [EffectiveCapacity] - ?��?容�?
 * @property {number} [FullWaterHeight] - 滿水位�??�尺�? * @property {number} [DeadWaterHeight] - ?�水位�??�尺�? * @property {number} [Latitude] - 緯度(WGS84)
 * @property {number} [Longitude] - 經度(WGS84)
 * @property {number} Storage - 總�?水�?
 * @property {number} ProtectionFlood - ?�否?�防洪�??��?0:?? 1:?��?
 * @property {number} HydraulicConstruction - 水工構造�??��?1:水庫, 2:?�河?��?
 * @property {number} Importance - 水庫壩堰?��??��?1:主�?, 0:次�?�? * @property {string} StationNo - 測�?�?��
 * @property {string} StationName - 測�??�稱
 * @property {string} BasinNo - 流�?�?��
 * @property {string} BasinName - 流�??�稱
 */

/**
 * @typedef {Object} ReservoirRealTimeInfo
 * @property {string} StationNo - 測�?�?��
 * @property {string} Time - 資�??��?（格式�?yyyy-MM-dd HH:mm�? * @property {number} [AccumulatedRainfall] - ?�日累�??��?(mm)
 * @property {number} WaterHeight - 水�?（公尺�?
 * @property {number} [EffectiveCapacity] - ?��?容�?
 * @property {number} [EffectiveStorage] - ?��??�水?? * @property {number} [PercentageOfStorage] - ?�水?? * @property {number} [OperationalStorage] - ?�用水�?
 * @property {number} [Inflow] - ?��???cms)
 * @property {number} [Outflow] - ?��???cms)
 * @property {string} [Status] - 水庫?�?�代�?= ['0: ?�水', '1: 洩水', '-1: ?�放']
 * @property {string} [NextSpillTime] - ?��?洩洪?��?（格式�?yyyy-MM-dd HH:mm�? * @property {number} [Discharge] - ?��???cms)
 * @property {number} [DischargeOfProtectionFlood] - ?�洪?��???cms)
 * @property {number} [DischargeOfEscapeSand] - ?��??��???cms)
 * @property {number} [DischargeOfHydroelectric] - ?�電?��???cms)
 * @property {number} [DischargeOfOthers] - ?��??��???cms)
 */

/**
 * @typedef {Object} ReservoirDaily
 * @property {string} StationNo - 測�?�?��
 * @property {string} Time - 資�??��?（格式�?yyyy-MM-dd HH:mm�? * @property {number} [EffectiveCapacity] - ?��?容�?
 * @property {number} [DeadWaterHeight] - ?�水位�??�尺�? * @property {number} [FullWaterHeight] - 滿水位�??�尺�? * @property {number} [AccumulatedRainfall] - ?�日累�??��?(mm)
 * @property {number} [InflowTotal] - ?�日總進水?? * @property {number} [OutflowTotal] - ?�日總出水�?
 */

/**
 * @typedef {Object} ReservoirWarning
 * @property {string} StationNo - 測�?�?��
 * @property {string} [CityCode] - �??�?��
 * @property {string} [TownCode] - ?�鎮�?��
 * @property {string} Time - 資�??��?（格式�?yyyy-MM-dd HH:mm�? * @property {number} [WaterHeight] - 水�?（公尺�?
 * @property {number} [DischargeOfProtectionFlood] - ?�洪?��???cms)
 * @property {string} [NextSpillTime] - ?��??��??��?（格式�?yyyy-MM-dd HH:mm�? * @property {number} [Discharge] - ?��???cms)
 * @property {string} Status - 水庫?�?�代�?= ['0: ?�水', '1: 洩水', '-1: ?�放']
 */

/**
 * @typedef {Object} ReservoirAffectedArea
 * @property {string} StationNo - 水庫�?��
 * @property {string} CityCode - �??�?��
 * @property {string} TownCode - ?�鎮�?��
 */

/**
 * ?��?水庫?�本資�?
 * @param {object} [params]
 * @returns {Promise<Array<ReservoirStation>>} 水庫?�本資�????
 */
export const fetchReservoirStation = (params) => apiClient.get(`/api/watergov/reservoir/station`, { params });

/**
 * ?��?水庫?��?資�?
 * @param {object} [params]
 * @returns {Promise<Array<ReservoirRealTimeInfo>>} 水庫?��?資�?資�????
 */
export const fetchReservoirRealTimeInfo = (params) => apiClient.get(`/api/watergov/reservoir/real-time-info`, { params });

/**
 * ?��?水庫統�?資�?
 * @param {object} [params]
 * @returns {Promise<Array<ReservoirDaily>>} 水庫統�?資�????
 */
export const fetchReservoirDaily = (params) => apiClient.get(`/api/watergov/reservoir/daily`, { params });

/**
 * ?��?水庫警示資�?
 * @param {object} [params]
 * @returns {Promise<Array<ReservoirWarning>>} 水庫警示資�????
 */
export const fetchReservoirWarning = (params) => apiClient.get(`/api/watergov/reservoir/warning`, { params });

/**
 * ?��?水庫警�?影響範�?
 * @param {object} [params]
 * @returns {Promise<Array<ReservoirAffectedArea>>} 水庫警�?影響範�?資�????
 */
export const fetchReservoirAffectedArea = (params) => apiClient.get(`/api/watergov/reservoir/affected-area`, { params });

export const useReservoirStation = (params, options) => useQuery({ queryKey: ['wraReservoirStation', params], queryFn: () => fetchReservoirStation(params), ...options });
export const useReservoirRealTimeInfo = (params, options) => useQuery({ queryKey: ['wraReservoirRealTimeInfo', params], queryFn: () => fetchReservoirRealTimeInfo(params), ...options });
export const useReservoirDaily = (params, options) => useQuery({ queryKey: ['wraReservoirDaily', params], queryFn: () => fetchReservoirDaily(params), ...options });
export const useReservoirWarning = (params, options) => useQuery({ queryKey: ['wraReservoirWarning', params], queryFn: () => fetchReservoirWarning(params), ...options });
export const useReservoirAffectedArea = (params, options) => useQuery({ queryKey: ['wraReservoirAffectedArea', params], queryFn: () => fetchReservoirAffectedArea(params), ...options });
