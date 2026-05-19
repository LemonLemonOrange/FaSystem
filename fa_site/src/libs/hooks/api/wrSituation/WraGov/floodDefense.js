import { useQuery } from 'react-query';
import axios from 'axios';

const API_BASE_URL = process.env.REACT_APP_API_BASE_URL || 'http://localhost:65326';

/**
 * @typedef {Object} MaterialLocation
 * @property {string} OperatorName - æ²³å??†ç½²
 * @property {string} [CityCode] - ç¸??ä»?¢¼
 * @property {string} Type - ?²æ?è³‡æ?é¡å? = ['1: ?²æ??‰åº«', '2: ?²æ?è¨­ç½®é»?]
 * @property {string} [Watershed] - æ°´ç³»
 * @property {string} [River] - æ²³å?
 * @property {string} [Remarks] - ?™è¨»
 * @property {string} [SrcUpdateTime] - ä¾†æ?è³‡æ??´æ–°?‚é?
 */

/**
 * ?–å??²æ?è³‡æ?ä½ç½®è³‡æ?
 * @param {object} [params]
 * @returns {Promise<Array<MaterialLocation>>} ?²æ?è³‡æ?ä½ç½®è³‡æ????
 */
export const fetchFloodDefenseMaterialLocation = async (params) => {
  const response = await axios.get(`${API_BASE_URL}/api/watergov/flood-defense/material-location`, { params });
  return response.data;
};

export const useFloodDefenseMaterialLocation = (params, options) => useQuery({ queryKey: ['wraFloodDefenseMaterialLocation', params], queryFn: () => fetchFloodDefenseMaterialLocation(params), ...options });
