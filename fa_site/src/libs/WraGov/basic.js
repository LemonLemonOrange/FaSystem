import { useQuery } from 'react-query';
import axios from 'axios';

const API_BASE_URL = process.env.REACT_APP_API_BASE_URL || 'http://localhost:65326';

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
export const fetchCity = async (params) => {
  const response = await axios.get(`${API_BASE_URL}/api/watergov/basic/city`, { params });
  return response.data;
};

/**
 * 取得指定縣市的鄉鎮清單
 * @param {string} city - 縣市名稱（例如：臺北市）
 * @param {object} [params]
 * @returns {Promise<Array<Town>>} 包含該縣市所有鄉鎮的資料陣列
 */
export const fetchTown = async (city, params) => {
  const response = await axios.get(`${API_BASE_URL}/api/watergov/basic/${city}/town`, { params });
  return response.data;
};

export const useCity = (params, options) => useQuery({ queryKey: ['wraCity', params], queryFn: () => fetchCity(params), ...options });
export const useTown = (city, params, options) => useQuery({ queryKey: ['wraTown', city, params], queryFn: () => fetchTown(city, params), enabled: !!city, ...options });
