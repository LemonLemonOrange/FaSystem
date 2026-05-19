import { useQuery } from 'react-query';
import { apiClient } from '../api/request';


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
export const fetchFloodDefenseMaterialLocation = (params) => apiClient.get(`/api/watergov/flood-defense/material-location`, { params });

export const useFloodDefenseMaterialLocation = (params, options) => useQuery({ queryKey: ['wraFloodDefenseMaterialLocation', params], queryFn: () => fetchFloodDefenseMaterialLocation(params), ...options });
