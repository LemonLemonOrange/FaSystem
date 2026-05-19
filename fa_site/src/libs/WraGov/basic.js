import { useQuery } from 'react-query';
import { apiClient } from '../api/request';


/**
 * @typedef {Object} City
 * @property {string} CityCode - 縣市代碼
 * @property {string} CityName_Ch - 縣市名稱（中文）
 * @property {string} CityName_En - 縣市名稱（英文）
 */

/**
 * @typedef {Object} Town
 * @property {string} TownCode - 鄉鎮代碼
 * @property {string} TownName - 鄉鎮名稱
 */

/**
 * 取得縣市清單
 * @param {object} [params]
 * @returns {Promise<Array<City>>} 包含所有縣市的資料陣列
 */
export const fetchCity = (params) => apiClient.get(`/api/watergov/basic/city`, { params });

/**
 * ?��??��?�???��??��??? * @param {string} city - �???�稱（�?如�??��?市�?
 * @param {object} [params]
 * @returns {Promise<Array<Town>>} ?�含該縣市�??��??��?資�????
 */
export const fetchTown = (city, params) => apiClient.get(`/api/watergov/basic/${city}/town`, { params });

export const useCity = (params, options) => useQuery({ queryKey: ['wraCity', params], queryFn: () => fetchCity(params), ...options });
export const useTown = (city, params, options) => useQuery({ queryKey: ['wraTown', city, params], queryFn: () => fetchTown(city, params), enabled: !!city, ...options });
