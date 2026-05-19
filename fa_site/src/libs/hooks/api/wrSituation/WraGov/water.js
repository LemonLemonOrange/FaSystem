import { useQuery } from 'react-query';
import axios from 'axios';

const API_BASE_URL = process.env.REACT_APP_API_BASE_URL || 'http://localhost:65326';

/**
 * @typedef {Object} WaterStation
 * @property {string} [Address] - æ°´ä?ç«™æ??¨åœ°
 * @property {string} CityCode - ç¸??ä»?¢¼
 * @property {number} [WarningLevel1] - ä¸€ç´šè­¦?’æ°´ä½ï??¬å°ºï¼? * @property {number} [WarningLevel2] - äºŒç?è­¦æ?æ°´ä?ï¼ˆå…¬å°ºï?
 * @property {number} [WarningLevel3] - ä¸‰ç?è­¦æ?æ°´ä?ï¼ˆå…¬å°ºï?
 * @property {number} [TopLevel] - ?€é«˜æ°´ä½ï??¬å°ºï¼? * @property {number} [Latitude] - ç·¯åº¦(WGS84)
 * @property {number} [Longitude] - ç¶“åº¦(WGS84)
 * @property {number} [PlanFloodLevel] - è¨ˆç•«æ´ªæ°´ä½ï??¬å°ºï¼? * @property {string} StationNo - æ¸¬ç?ä»?¢¼
 * @property {string} StationName - æ¸¬ç??ç¨±
 * @property {string} BasinNo - æµå?ä»?¢¼
 * @property {string} BasinName - æµå??ç¨±
 */

/**
 * @typedef {Object} WaterRealTimeInfo
 * @property {string} StationNo - æ°´ä?ç«™ä»£ç¢? * @property {string} Time - è³‡æ??‚é?ï¼ˆæ ¼å¼ï?yyyy-MM-dd HH:mmï¼? * @property {number} WaterLevel - æ°´ä?ï¼ˆå…¬å°ºï?
 */

/**
 * @typedef {Object} WaterWarning
 * @property {string} StationNo - æ¸¬ç?ä»?¢¼
 * @property {string} CityCode - ç¸??ä»?¢¼
 * @property {string} TownCode - ?‰é®ä»?¢¼
 * @property {string} Time - è³‡æ??‚é?ï¼ˆæ ¼å¼ï?yyyy-MM-dd HH:mmï¼? * @property {number} WaterLevel - æ°´ä?ï¼ˆå…¬å°ºï?
 * @property {number} WarningLevel - è­¦æ?ç´šåˆ¥
 */

/**
 * ?–å?æ°´ä?ç«™åŸº?¬è??? * @param {object} [params]
 * @returns {Promise<Array<WaterStation>>} æ°´ä?ç«™åŸº?¬è??™é™£?? */
export const fetchWaterStation = async (params) => {
  const response = await axios.get(`${API_BASE_URL}/api/watergov/water/station`, { params });
  return response.data;
};

/**
 * ?–å?æ°´ä??³æ?è³‡è?
 * @param {object} [params]
 * @returns {Promise<Array<WaterRealTimeInfo>>} æ°´ä??³æ?è³‡è?è³‡æ????
 */
export const fetchWaterRealTimeInfo = async (params) => {
  const response = await axios.get(`${API_BASE_URL}/api/watergov/water/real-time-info`, { params });
  return response.data;
};

/**
 * ?–å?æ°´ä?è­¦ç¤ºè³‡æ?
 * @param {object} [params]
 * @returns {Promise<Array<WaterWarning>>} æ°´ä?è­¦ç¤ºè³‡æ????
 */
export const fetchWaterWarning = async (params) => {
  const response = await axios.get(`${API_BASE_URL}/api/watergov/water/warning`, { params });
  return response.data;
};

export const useWaterStation = (params, options) => useQuery({ queryKey: ['wraWaterStation', params], queryFn: () => fetchWaterStation(params), ...options });
export const useWaterRealTimeInfo = (params, options) => useQuery({ queryKey: ['wraWaterRealTimeInfo', params], queryFn: () => fetchWaterRealTimeInfo(params), ...options });
export const useWaterWarning = (params, options) => useQuery({ queryKey: ['wraWaterWarning', params], queryFn: () => fetchWaterWarning(params), ...options });
