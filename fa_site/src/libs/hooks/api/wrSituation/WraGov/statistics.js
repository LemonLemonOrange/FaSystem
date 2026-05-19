import { useQuery } from 'react-query';
import axios from 'axios';

const API_BASE_URL = process.env.REACT_APP_API_BASE_URL || 'http://localhost:65326';

/**
 * @typedef {Object} DisasterFloodingStatistics
 * @property {string} [CityCode] - ç¸??ä»?¢¼
 * @property {number} [TownCount] - ?—å½±?¿é??®æ•¸
 * @property {number} [RecededCount] - å·²é€€æ°´æ•¸
 * @property {number} [FloodingCount] - ?ªé€€æ°´æ•¸
 * @property {number} [Total] - ?½å®³ç¸½æ•¸
 */

/**
 * @typedef {Object} DisasterWaterFacilityStatistics
 * @property {string} [CityCode] - ç¸??ä»?¢¼
 * @property {number} [RepairedCount] - å·²ä¿®å¾©æ•¸?? * @property {number} [RepairingCount] - ?¶ä¿®ä¸­æ•¸?? * @property {number} [Total] - ç¸½æ•¸?? */

/**
 * @typedef {Object} FloodDefenseMaterial
 * @property {string} [Name] - ?²æ?è³‡æ??ç¨±
 * @property {number} [Total] - ç¸½æ•¸
 */

/**
 * @typedef {Object} FloodDefenseOperator
 * @property {string} [OperatorName] - æ²³å??†ç½²
 * @property {Array<FloodDefenseMaterial>} [Material] - ?²æ?è³‡æ?
 */

/**
 * æ·¹æ°´?½æ?çµ±è? - ?–å??®æ¬¡äº‹ä»¶?„æ·¹æ°´ç½?…çµ±è¨? * @param {string} eventNo - äº‹ä»¶ç·¨è?
 * @returns {Promise<Array<DisasterFloodingStatistics>>} æ·¹æ°´?½æ?çµ±è?è³‡æ????
 */
export const fetchStatisticsFlooding = async (eventNo) => {
  const response = await axios.get(`${API_BASE_URL}/api/watergov/statistics/flooding/${eventNo}`);
  return response.data;
};

/**
 * æ°´åˆ©è¨­æ–½?½æ?çµ±è? - ?–å??®æ¬¡äº‹ä»¶?„æ°´?©è¨­?½ç½?…çµ±è¨? * @param {string} eventNo - äº‹ä»¶ç·¨è?
 * @returns {Promise<Array<DisasterWaterFacilityStatistics>>} æ°´åˆ©è¨­æ–½?½æ?çµ±è?è³‡æ????
 */
export const fetchStatisticsWaterFacility = async (eventNo) => {
  const response = await axios.get(`${API_BASE_URL}/api/watergov/statistics/water-facility/${eventNo}`);
  return response.data;
};

/**
 * ?²æ?è³‡æ?çµ±è?
 * @param {object} [params]
 * @returns {Promise<Array<FloodDefenseOperator>>} ?²æ?è³‡æ?çµ±è?è³‡æ????
 */
export const fetchStatisticsFloodDefenseMaterial = async (params) => {
  const response = await axios.get(`${API_BASE_URL}/api/watergov/statistics/flood-defense-material`, { params });
  return response.data;
};

export const useStatisticsFlooding = (eventNo, options) => useQuery({ queryKey: ['wraStatisticsFlooding', eventNo], queryFn: () => fetchStatisticsFlooding(eventNo), enabled: !!eventNo, ...options });
export const useStatisticsWaterFacility = (eventNo, options) => useQuery({ queryKey: ['wraStatisticsWaterFacility', eventNo], queryFn: () => fetchStatisticsWaterFacility(eventNo), enabled: !!eventNo, ...options });
export const useStatisticsFloodDefenseMaterial = (params, options) => useQuery({ queryKey: ['wraStatisticsFloodDefenseMaterial', params], queryFn: () => fetchStatisticsFloodDefenseMaterial(params), ...options });
