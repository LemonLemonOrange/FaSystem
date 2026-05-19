import { useQuery } from 'react-query';
import { apiClient } from '../api/request';


/**
 * @typedef {Object} DisasterFloodingStatistics
 * @property {string} [CityCode] - �??�?��
 * @property {number} [TownCount] - ?�影?��??�數
 * @property {number} [RecededCount] - 已退水數
 * @property {number} [FloodingCount] - ?�退水數
 * @property {number} [Total] - ?�害總數
 */

/**
 * @typedef {Object} DisasterWaterFacilityStatistics
 * @property {string} [CityCode] - �??�?��
 * @property {number} [RepairedCount] - 已修復數?? * @property {number} [RepairingCount] - ?�修中數?? * @property {number} [Total] - 總數?? */

/**
 * @typedef {Object} FloodDefenseMaterial
 * @property {string} [Name] - ?��?資�??�稱
 * @property {number} [Total] - 總數
 */

/**
 * @typedef {Object} FloodDefenseOperator
 * @property {string} [OperatorName] - 河�??�署
 * @property {Array<FloodDefenseMaterial>} [Material] - ?��?資�?
 */

/**
 * 淹水?��?統�? - ?��??�次事件?�淹水災?�統�? * @param {string} eventNo - 事件編�?
 * @returns {Promise<Array<DisasterFloodingStatistics>>} 淹水?��?統�?資�????
 */
export const fetchStatisticsFlooding = (eventNo) => apiClient.get(`/api/watergov/statistics/flooding/${eventNo}`);

/**
 * 水利設施?��?統�? - ?��??�次事件?�水?�設?�災?�統�? * @param {string} eventNo - 事件編�?
 * @returns {Promise<Array<DisasterWaterFacilityStatistics>>} 水利設施?��?統�?資�????
 */
export const fetchStatisticsWaterFacility = (eventNo) => apiClient.get(`/api/watergov/statistics/water-facility/${eventNo}`);

/**
 * ?��?資�?統�?
 * @param {object} [params]
 * @returns {Promise<Array<FloodDefenseOperator>>} ?��?資�?統�?資�????
 */
export const fetchStatisticsFloodDefenseMaterial = (params) => apiClient.get(`/api/watergov/statistics/flood-defense-material`, { params });

export const useStatisticsFlooding = (eventNo, options) => useQuery({ queryKey: ['wraStatisticsFlooding', eventNo], queryFn: () => fetchStatisticsFlooding(eventNo), enabled: !!eventNo, ...options });
export const useStatisticsWaterFacility = (eventNo, options) => useQuery({ queryKey: ['wraStatisticsWaterFacility', eventNo], queryFn: () => fetchStatisticsWaterFacility(eventNo), enabled: !!eventNo, ...options });
export const useStatisticsFloodDefenseMaterial = (params, options) => useQuery({ queryKey: ['wraStatisticsFloodDefenseMaterial', params], queryFn: () => fetchStatisticsFloodDefenseMaterial(params), ...options });
