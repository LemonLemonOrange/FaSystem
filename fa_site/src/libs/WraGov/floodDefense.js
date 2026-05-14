import { useQuery } from 'react-query';
import axios from 'axios';

const API_BASE_URL = process.env.REACT_APP_API_BASE_URL || 'http://localhost:65326';

/**
 * @typedef {Object} MaterialLocation
 * @property {string} OperatorName - 河川分署
 * @property {string} [CityCode] - 縣市代碼
 * @property {string} Type - 防汛資材類型 = ['1: 防汛倉庫', '2: 防汛設置點']
 * @property {string} [Watershed] - 水系
 * @property {string} [River] - 河川
 * @property {string} [Remarks] - 備註
 * @property {string} [SrcUpdateTime] - 來源資料更新時間
 */

/**
 * 取得防汛資材位置資料
 * @param {object} [params]
 * @returns {Promise<Array<MaterialLocation>>} 防汛資材位置資料陣列
 */
export const fetchFloodDefenseMaterialLocation = async (params) => {
  const response = await axios.get(`${API_BASE_URL}/api/watergov/flood-defense/material-location`, { params });
  return response.data;
};

export const useFloodDefenseMaterialLocation = (params, options) => useQuery({ queryKey: ['wraFloodDefenseMaterialLocation', params], queryFn: () => fetchFloodDefenseMaterialLocation(params), ...options });
