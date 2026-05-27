import { useQuery } from "react-query";
import { wraApi } from "./wraApi";

/** @typedef {import('./wraApi').QueryParams} QueryParams */

/**
 * @typedef {Object} RainStation
 * @property {string} Address - 測站地址
 * @property {string} CityCode - 縣市代碼
 * @property {number} Latitude - 緯度(WGS84)
 * @property {number} Longitude - 經度(WGS84)
 * @property {string} StationNo - 測站編號
 * @property {string} StationName - 測站名稱
 * @property {string} BasinNo - 流域編號
 * @property {string} BasinName - 流域名稱
 */

/**
 * @typedef {Object} RainRealTimeInfo
 * @property {string} StationNo - 測站編號
 * @property {string} Time - 資料時間（格式：yyyy-MM-dd HH:mm）
 * @property {number} M10 - 10 分鐘累積雨量(mm)
 * @property {number} H1 - 1 小時累積雨量(mm)
 * @property {number} H3 - 3 小時累積雨量(mm)
 * @property {number} H6 - 6 小時累積雨量(mm)
 * @property {number} H12 - 12 小時累積雨量(mm)
 * @property {number} H24 - 24 小時累積雨量(mm)
 */

/**
 * @typedef {Object} RainWarning
 * @property {string} StationNo - 測站編號
 * @property {string} CityCode - 縣市代碼
 * @property {string} TownCode - 鄉鎮代碼
 * @property {string} Time - 資料時間（格式：yyyy-MM-dd HH:mm）
 * @property {number} M10 - 10 分鐘累積雨量(mm)
 * @property {number} H1 - 1 小時累積雨量(mm)
 * @property {number} H3 - 3 小時累積雨量(mm)
 * @property {number} H6 - 6 小時累積雨量(mm)
 * @property {number} H12 - 12 小時累積雨量(mm)
 * @property {number} H24 - 24 小時累積雨量(mm)
 * @property {number} WarningLevel - 警戒級別
 * @property {string} AffectedArea - 影響範圍
 */

/**
 * @typedef {Object} RainAffectedArea
 * @property {string} StationNo - 測站編號
 * @property {string} CityCode - 縣市代碼
 * @property {string} TownCode - 鄉鎮代碼
 * @property {number} AlertLevel2_H1 - 二級警戒 1 小時累積雨量(mm)
 * @property {number} AlertLevel2_H3 - 二級警戒 3 小時累積雨量(mm)
 * @property {number} AlertLevel2_H6 - 二級警戒 6 小時累積雨量(mm)
 * @property {number} AlertLevel1_H1 - 一級警戒 1 小時累積雨量(mm)
 * @property {number} AlertLevel1_H3 - 一級警戒 3 小時累積雨量(mm)
 * @property {number} AlertLevel1_H6 - 一級警戒 6 小時累積雨量(mm)
 * @property {string} AffectedArea - 影響範圍
 */

/**
 * 取得雨量測站基本資料
 * @param {QueryParams} [params]
 * @returns {Promise<Array<RainStation>>} 雨量測站基本資料陣列
 */
export const fetchRainStation = (params) => wraApi.get("/v1/Rain/Station", { params }).then(res => res.data);

/**
 * 取得雨量即時資料
 * @param {QueryParams} [params]
 * @returns {Promise<Array<RainRealTimeInfo>>} 雨量即時資料陣列
 */
export const fetchRainRealTimeInfo = (params) => wraApi.get("/v1/Rain/RealTimeInfo", { params }).then(res => res.data);

/**
 * 取得淹水警示資料
 * @param {QueryParams} [params]
 * @returns {Promise<Array<RainWarning>>} 淹水警示資料陣列
 */
export const fetchRainWarning = (params) => wraApi.get("/v1/Rain/Warning", { params }).then(res => res.data);

/**
 * 取得雨量警戒影響範圍
 * @param {QueryParams} [params]
 * @returns {Promise<Array<RainAffectedArea>>} 雨量警戒影響範圍資料陣列
 */
export const fetchRainAffectedArea = (params) => wraApi.get("/v1/Rain/AffectedArea", { params }).then(res => res.data);

export const useRainStation = (params, options) => useQuery({ queryKey: ["wraRainStation", params], queryFn: () => fetchRainStation(params), ...options });
export const useRainRealTimeInfo = (params, options) => useQuery({ queryKey: ["wraRainRealTimeInfo", params], queryFn: () => fetchRainRealTimeInfo(params), ...options });
export const useRainWarning = (params, options) => useQuery({ queryKey: ["wraRainWarning", params], queryFn: () => fetchRainWarning(params), ...options });
export const useRainAffectedArea = (params, options) => useQuery({ queryKey: ["wraRainAffectedArea", params], queryFn: () => fetchRainAffectedArea(params), ...options });