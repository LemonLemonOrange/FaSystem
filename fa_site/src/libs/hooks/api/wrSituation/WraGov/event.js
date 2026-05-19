import { useQuery } from 'react-query';
import { wraApi } from './wraApi';

/** @typedef {import('./wraApi').QueryParams} QueryParams */

/**
 * @typedef {Object} Event
 * @property {string} EventNo - äº‹ä»¶ç·¨è?
 * @property {string} EventName - äº‹ä»¶?ç¨±
 * @property {string} BeginTime - ?‹å??‚é?
 * @property {string} [EndTime] - çµæ??‚é?
 * @property {number} IsActive - ?¯å¦?Ÿç”¨ï¼ˆé€²è?ä¸­ï?
 */

/**
 * ?–å?å¹´åº¦äº‹ä»¶?—è¡¨ï¼ˆä?å¹´åº¦?¥è©¢å¤§é›¨?è±ª?¨ã€é¢±é¢¨ç?äº‹ä»¶ï¼? * @param {number} year - è¥¿å?å¹´ä»½
 * @param {QueryParams} [params]
 * @returns {Promise<Array<Event>>} å¹´åº¦äº‹ä»¶è³‡æ????
 */
export const fetchEventByYear = (year, params) => wraApi.get(`/v1/Event/Year/${year}`, { params }).then(res => res.data);

export const useEventByYear = (year, params, options) => useQuery({ queryKey: ['wraEvent', year, params], queryFn: () => fetchEventByYear(year, params), enabled: !!year, ...options });
