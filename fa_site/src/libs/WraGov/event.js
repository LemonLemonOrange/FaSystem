import { useQuery } from "react-query";
import { wraApi } from "./wraApi";

/** @typedef {import('./wraApi').QueryParams} QueryParams */

/**
 * @typedef {Object} Event
 * @property {string} EventNo - 事件編號
 * @property {string} EventName - 事件名稱
 * @property {string} BeginTime - 開始時間
 * @property {string} [EndTime] - 結束時間
 * @property {number} IsActive - 是否啟用（進行中）
 */

/**
 * 取得年度事件列表（依年度查詢大雨、豪雨、颱風等事件）
 * @param {number} year - 西元年份
 * @param {QueryParams} [params]
 * @returns {Promise<Array<Event>>} 年度事件資料陣列
 */
export const fetchEventByYear = (year, params) => wraApi.get(`/v1/Event/Year/${year}`, { params }).then(res => res.data);

export const useEventByYear = (year, params, options) => useQuery({ queryKey: ["wraEvent", year, params], queryFn: () => fetchEventByYear(year, params), enabled: !!year, ...options });
