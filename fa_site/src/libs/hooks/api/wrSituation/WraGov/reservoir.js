import { useQuery } from 'react-query';
import axios from 'axios';

    const API_BASE_URL = process.env.REACT_APP_API_BASE_URL || 'http://localhost:65326';

/**
 * @typedef {Object} ReservoirStation
 * @property {string} CityCode - ç¸??ä»?¢¼
 * @property {number} [EffectiveCapacity] - ?‰æ?å®¹é?
 * @property {number} [FullWaterHeight] - æ»¿æ°´ä½ï??¬å°ºï¼? * @property {number} [DeadWaterHeight] - ?†æ°´ä½ï??¬å°ºï¼? * @property {number} [Latitude] - ç·¯åº¦(WGS84)
 * @property {number} [Longitude] - ç¶“åº¦(WGS84)
 * @property {number} Storage - ç¸½è?æ°´é?
 * @property {number} ProtectionFlood - ?¯å¦?·é˜²æ´ªå??½ï?0:?? 1:?¯ï?
 * @property {number} HydraulicConstruction - æ°´å·¥æ§‹é€ é??‹ï?1:æ°´åº«, 2:?”æ²³?°ï?
 * @property {number} Importance - æ°´åº«å£©å °?è??§ï?1:ä¸»è?, 0:æ¬¡è?ï¼? * @property {string} StationNo - æ¸¬ç?ä»?¢¼
 * @property {string} StationName - æ¸¬ç??ç¨±
 * @property {string} BasinNo - æµå?ä»?¢¼
 * @property {string} BasinName - æµå??ç¨±
 */

/**
 * @typedef {Object} ReservoirRealTimeInfo
 * @property {string} StationNo - æ¸¬ç?ä»?¢¼
 * @property {string} Time - è³‡æ??‚é?ï¼ˆæ ¼å¼ï?yyyy-MM-dd HH:mmï¼? * @property {number} [AccumulatedRainfall] - ?¶æ—¥ç´¯ç??¨é?(mm)
 * @property {number} WaterHeight - æ°´ä?ï¼ˆå…¬å°ºï?
 * @property {number} [EffectiveCapacity] - ?‰æ?å®¹é?
 * @property {number} [EffectiveStorage] - ?‰æ??„æ°´?? * @property {number} [PercentageOfStorage] - ?„æ°´?? * @property {number} [OperationalStorage] - ?¯ç”¨æ°´é?
 * @property {number} [Inflow] - ?¥æ???cms)
 * @property {number} [Outflow] - ?ºæ???cms)
 * @property {string} [Status] - æ°´åº«?€?‹ä»£ç¢?= ['0: ?„æ°´', '1: æ´©æ°´', '-1: ?’æ”¾']
 * @property {string} [NextSpillTime] - ?è?æ´©æ´ª?‚é?ï¼ˆæ ¼å¼ï?yyyy-MM-dd HH:mmï¼? * @property {number} [Discharge] - ?¾æ???cms)
 * @property {number} [DischargeOfProtectionFlood] - ?²æ´ª?¾æ???cms)
 * @property {number} [DischargeOfEscapeSand] - ?’ç??¾æ???cms)
 * @property {number} [DischargeOfHydroelectric] - ?¼é›»?¾æ???cms)
 * @property {number} [DischargeOfOthers] - ?¶ä??¾æ???cms)
 */

/**
 * @typedef {Object} ReservoirDaily
 * @property {string} StationNo - æ¸¬ç?ä»?¢¼
 * @property {string} Time - è³‡æ??‚é?ï¼ˆæ ¼å¼ï?yyyy-MM-dd HH:mmï¼? * @property {number} [EffectiveCapacity] - ?‰æ?å®¹é?
 * @property {number} [DeadWaterHeight] - ?†æ°´ä½ï??¬å°ºï¼? * @property {number} [FullWaterHeight] - æ»¿æ°´ä½ï??¬å°ºï¼? * @property {number} [AccumulatedRainfall] - ?¶æ—¥ç´¯ç??¨é?(mm)
 * @property {number} [InflowTotal] - ?¶æ—¥ç¸½é€²æ°´?? * @property {number} [OutflowTotal] - ?¶æ—¥ç¸½å‡ºæ°´é?
 */

/**
 * @typedef {Object} ReservoirWarning
 * @property {string} StationNo - æ¸¬ç?ä»?¢¼
 * @property {string} [CityCode] - ç¸??ä»?¢¼
 * @property {string} [TownCode] - ?‰é®ä»?¢¼
 * @property {string} Time - è³‡æ??‚é?ï¼ˆæ ¼å¼ï?yyyy-MM-dd HH:mmï¼? * @property {number} [WaterHeight] - æ°´ä?ï¼ˆå…¬å°ºï?
 * @property {number} [DischargeOfProtectionFlood] - ?²æ´ª?¾æ???cms)
 * @property {string} [NextSpillTime] - ?è??¾æ??‚é?ï¼ˆæ ¼å¼ï?yyyy-MM-dd HH:mmï¼? * @property {number} [Discharge] - ?¾æ???cms)
 * @property {string} Status - æ°´åº«?€?‹ä»£ç¢?= ['0: ?„æ°´', '1: æ´©æ°´', '-1: ?’æ”¾']
 */

/**
 * @typedef {Object} ReservoirAffectedArea
 * @property {string} StationNo - æ°´åº«ä»?¢¼
 * @property {string} CityCode - ç¸??ä»?¢¼
 * @property {string} TownCode - ?‰é®ä»?¢¼
 */

/**
 * ?–å?æ°´åº«?ºæœ¬è³‡æ?
 * @param {object} [params]
 * @returns {Promise<Array<ReservoirStation>>} æ°´åº«?ºæœ¬è³‡æ????
 */
export const fetchReservoirStation = async (params) => {
  const response = await axios.get(`${API_BASE_URL}/api/watergov/reservoir/station`, { params });
  return response.data;
};

/**
 * ?–å?æ°´åº«?³æ?è³‡è?
 * @param {object} [params]
 * @returns {Promise<Array<ReservoirRealTimeInfo>>} æ°´åº«?³æ?è³‡è?è³‡æ????
 */
export const fetchReservoirRealTimeInfo = async (params) => {
  const response = await axios.get(`${API_BASE_URL}/api/watergov/reservoir/real-time-info`, { params });
  return response.data;
};

/**
 * ?–å?æ°´åº«çµ±è?è³‡æ?
 * @param {object} [params]
 * @returns {Promise<Array<ReservoirDaily>>} æ°´åº«çµ±è?è³‡æ????
 */
export const fetchReservoirDaily = async (params) => {
  const response = await axios.get(`${API_BASE_URL}/api/watergov/reservoir/daily`, { params });
  return response.data;
};

/**
 * ?–å?æ°´åº«è­¦ç¤ºè³‡æ?
 * @param {object} [params]
 * @returns {Promise<Array<ReservoirWarning>>} æ°´åº«è­¦ç¤ºè³‡æ????
 */
export const fetchReservoirWarning = async (params) => {
  const response = await axios.get(`${API_BASE_URL}/api/watergov/reservoir/warning`, { params });
  return response.data;
};

/**
 * ?–å?æ°´åº«è­¦æ?å½±éŸ¿ç¯„å?
 * @param {object} [params]
 * @returns {Promise<Array<ReservoirAffectedArea>>} æ°´åº«è­¦æ?å½±éŸ¿ç¯„å?è³‡æ????
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
