import { useQuery } from 'react-query';
import { wraApi } from './wraApi';

/** @typedef {import('./wraApi').QueryParams} QueryParams */

/**
 * @typedef {Object} RainStation
 * @property {string} Address - æ¸¬ç??€?¨åœ°
 * @property {string} CityCode - ç¸??ä»?¢¼
 * @property {number} Latitude - ç·¯åº¦(WGS84)
 * @property {number} Longitude - ç¶“åº¦(WGS84)
 * @property {string} StationNo - æ¸¬ç?ä»?¢¼
 * @property {string} StationName - æ¸¬ç??ç¨±
 * @property {string} BasinNo - æµå?ä»?¢¼
 * @property {string} BasinName - æµå??ç¨±
 */

/**
 * @typedef {Object} RainRealTimeInfo
 * @property {string} StationNo - æ¸¬ç?ä»?¢¼
 * @property {string} Time - è³‡æ??‚é?ï¼ˆæ ¼å¼ï?yyyy-MM-dd HH:mmï¼? * @property {number} M10 - 10 ?†é?ç´¯ç??¨é?(mm)
 * @property {number} H1 - 1 å°æ?ç´¯ç??¨é?(mm)
 * @property {number} H3 - 3 å°æ?ç´¯ç??¨é?(mm)
 * @property {number} H6 - 6 å°æ?ç´¯ç??¨é?(mm)
 * @property {number} H12 - 12 å°æ?ç´¯ç??¨é?(mm)
 * @property {number} H24 - 24 å°æ?ç´¯ç??¨é?(mm)
 */

/**
 * @typedef {Object} RainWarning
 * @property {string} StationNo - æ¸¬ç?ä»?¢¼
 * @property {string} CityCode - ç¸??ä»?¢¼
 * @property {string} TownCode - ?‰é®ä»?¢¼
 * @property {string} Time - è³‡æ??‚é?ï¼ˆæ ¼å¼ï?yyyy-MM-dd HH:mmï¼? * @property {number} M10 - 10 ?†é?ç´¯ç??¨é?(mm)
 * @property {number} H1 - 1 å°æ?ç´¯ç??¨é?(mm)
 * @property {number} H3 - 3 å°æ?ç´¯ç??¨é?(mm)
 * @property {number} H6 - 6 å°æ?ç´¯ç??¨é?(mm)
 * @property {number} H12 - 12 å°æ?ç´¯ç??¨é?(mm)
 * @property {number} H24 - 24 å°æ?ç´¯ç??¨é?(mm)
 * @property {number} WarningLevel - è­¦æ?ç´šåˆ¥
 * @property {string} AffectedArea - å½±éŸ¿ç¯„å?
 */

/**
 * @typedef {Object} RainAffectedArea
 * @property {string} StationNo - æ¸¬ç?ä»?¢¼
 * @property {string} CityCode - ç¸??ä»?¢¼
 * @property {string} TownCode - ?‰é®ä»?¢¼
 * @property {number} AlertLevel2_H1 - äºŒç?è­¦æ? 1 å°æ?ç´¯ç??¨é?(mm)
 * @property {number} AlertLevel2_H3 - äºŒç?è­¦æ? 3 å°æ?ç´¯ç??¨é?(mm)
 * @property {number} AlertLevel2_H6 - äºŒç?è­¦æ? 6 å°æ?ç´¯ç??¨é?(mm)
 * @property {number} AlertLevel1_H1 - ä¸€ç´šè­¦??1 å°æ?ç´¯ç??¨é?(mm)
 * @property {number} AlertLevel1_H3 - ä¸€ç´šè­¦??3 å°æ?ç´¯ç??¨é?(mm)
 * @property {number} AlertLevel1_H6 - ä¸€ç´šè­¦??6 å°æ?ç´¯ç??¨é?(mm)
 * @property {string} AffectedArea - å½±éŸ¿ç¯„å?
 */

/**
 * ?–å??¨é?æ¸¬ç??ºæœ¬è³‡æ?
 * @param {QueryParams} [params]
 * @returns {Promise<Array<RainStation>>} ?¨é?æ¸¬ç??ºæœ¬è³‡æ????
 */
export const fetchRainStation = (params) => wraApi.get('/v1/Rain/Station', { params }).then(res => res.data);

/**
 * ?–å??¨é??³æ?è³‡è?
 * @param {QueryParams} [params]
 * @returns {Promise<Array<RainRealTimeInfo>>} ?¨é??³æ?è³‡è?è³‡æ????
 */
export const fetchRainRealTimeInfo = (params) => wraApi.get('/v1/Rain/RealTimeInfo', { params }).then(res => res.data);

/**
 * ?–å?æ·¹æ°´è­¦ç¤ºè³‡æ?
 * @param {QueryParams} [params]
 * @returns {Promise<Array<RainWarning>>} æ·¹æ°´è­¦ç¤ºè³‡æ????
 */
export const fetchRainWarning = (params) => wraApi.get('/v1/Rain/Warning', { params }).then(res => res.data);

/**
 * ?–å??¨é?è­¦æ?å½±éŸ¿ç¯„å?
 * @param {QueryParams} [params]
 * @returns {Promise<Array<RainAffectedArea>>} ?¨é?è­¦æ?å½±éŸ¿ç¯„å?è³‡æ????
 */
export const fetchRainAffectedArea = (params) => wraApi.get('/v1/Rain/AffectedArea', { params }).then(res => res.data);

export const useRainStation = (params, options) => useQuery({ queryKey: ['wraRainStation', params], queryFn: () => fetchRainStation(params), ...options });
export const useRainRealTimeInfo = (params, options) => useQuery({ queryKey: ['wraRainRealTimeInfo', params], queryFn: () => fetchRainRealTimeInfo(params), ...options });
export const useRainWarning = (params, options) => useQuery({ queryKey: ['wraRainWarning', params], queryFn: () => fetchRainWarning(params), ...options });
export const useRainAffectedArea = (params, options) => useQuery({ queryKey: ['wraRainAffectedArea', params], queryFn: () => fetchRainAffectedArea(params), ...options });
