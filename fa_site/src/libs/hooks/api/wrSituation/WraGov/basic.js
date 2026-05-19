import { useQuery } from 'react-query';
import axios from 'axios';

const API_BASE_URL = process.env.REACT_APP_API_BASE_URL || 'http://localhost:65326';

/**
 * @typedef {Object} City
 * @property {string} CityCode - ç¸??ä»?¢¼
 * @property {string} CityName_Ch - ç¸???ç¨±ï¼ˆä¸­?‡ï?
 * @property {string} CityName_En - ç¸???ç¨±ï¼ˆè‹±?‡ï?
 */

/**
 * @typedef {Object} Town
 * @property {string} TownCode - ?‰é®ä»?¢¼
 * @property {string} TownName - ?‰é®?ç¨±
 */

/**
 * ?–å?ç¸??æ¸…å–®
 * @param {object} [params]
 * @returns {Promise<Array<City>>} ?…å«?€?‰ç¸£å¸‚ç?è³‡æ????
 */
export const fetchCity = async (params) => {
  const response = await axios.get(`${API_BASE_URL}/api/watergov/basic/city`, { params });
  return response.data;
};

/**
 * ?–å??‡å?ç¸???„é??®æ??? * @param {string} city - ç¸???ç¨±ï¼ˆä?å¦‚ï??ºå?å¸‚ï?
 * @param {object} [params]
 * @returns {Promise<Array<Town>>} ?…å«è©²ç¸£å¸‚æ??‰é??®ç?è³‡æ????
 */
export const fetchTown = async (city, params) => {
  const response = await axios.get(`${API_BASE_URL}/api/watergov/basic/${city}/town`, { params });
  return response.data;
};

export const useCity = (params, options) => useQuery({ queryKey: ['wraCity', params], queryFn: () => fetchCity(params), ...options });
export const useTown = (city, params, options) => useQuery({ queryKey: ['wraTown', city, params], queryFn: () => fetchTown(city, params), enabled: !!city, ...options });
