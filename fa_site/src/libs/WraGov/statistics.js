import { useQuery } from 'react-query';
import axios from 'axios';

const API_BASE_URL = process.env.REACT_APP_API_BASE_URL || 'http://localhost:65326';

/**
 * @typedef {Object} DisasterFloodingStatistics
 * @property {string} [CityCode] - 縣市代碼
 * @property {number} [TownCount] - 受影響鄉鎮數
 * @property {number} [RecededCount] - 已退水數
 * @property {number} [FloodingCount] - 未退水數
 * @property {number} [Total] - 災害總數
 */

/**
 * @typedef {Object} DisasterWaterFacilityStatistics
 * @property {string} [CityCode] - 縣市代碼
 * @property {number} [RepairedCount] - 已修復數量
 * @property {number} [RepairingCount] - 搶修中數量
 * @property {number} [Total] - 總數量
 */

/**
 * @typedef {Object} FloodDefenseMaterial
 * @property {string} [Name] - 防汛資材名稱
 * @property {number} [Total] - 總數
 */

/**
 * @typedef {Object} FloodDefenseOperator
 * @property {string} [OperatorName] - 河川分署
 * @property {Array<FloodDefenseMaterial>} [Material] - 防汛資材
 */

/**
 * 淹水災情統計 - 取得單次事件的淹水災情統計
 * @param {string} eventNo - 事件編號
 * @returns {Promise<Array<DisasterFloodingStatistics>>} 淹水災情統計資料陣列
 */
export const fetchStatisticsFlooding = async (eventNo) => {
  const response = await axios.get(`${API_BASE_URL}/api/watergov/statistics/flooding/${eventNo}`);
  return response.data;
};

/**
 * 水利設施災情統計 - 取得單次事件的水利設施災情統計
 * @param {string} eventNo - 事件編號
 * @returns {Promise<Array<DisasterWaterFacilityStatistics>>} 水利設施災情統計資料陣列
 */
export const fetchStatisticsWaterFacility = async (eventNo) => {
  const response = await axios.get(`${API_BASE_URL}/api/watergov/statistics/water-facility/${eventNo}`);
  return response.data;
};

/**
 * 防汛資材統計
 * @param {object} [params]
 * @returns {Promise<Array<FloodDefenseOperator>>} 防汛資材統計資料陣列
 */
export const fetchStatisticsFloodDefenseMaterial = async (params) => {
  const response = await axios.get(`${API_BASE_URL}/api/watergov/statistics/flood-defense-material`, { params });
  return response.data;
};

export const useStatisticsFlooding = (eventNo, options) => useQuery({ queryKey: ['wraStatisticsFlooding', eventNo], queryFn: () => fetchStatisticsFlooding(eventNo), enabled: !!eventNo, ...options });
export const useStatisticsWaterFacility = (eventNo, options) => useQuery({ queryKey: ['wraStatisticsWaterFacility', eventNo], queryFn: () => fetchStatisticsWaterFacility(eventNo), enabled: !!eventNo, ...options });
export const useStatisticsFloodDefenseMaterial = (params, options) => useQuery({ queryKey: ['wraStatisticsFloodDefenseMaterial', params], queryFn: () => fetchStatisticsFloodDefenseMaterial(params), ...options });
