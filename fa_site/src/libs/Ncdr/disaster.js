import { useQuery } from 'react-query';
import axios from 'axios';

const API_BASE_URL = process.env.REACT_APP_API_BASE_URL || 'http://localhost:65326';

const fetchDroughtAlert = async () => {
  try {
    const response = await axios.get(`${API_BASE_URL}/api/ncdr/drought-alert`);
    return response.data;
  } catch (error) {
    if (error.response?.status === 404) {
      return null;
    }
    throw error;
  }
};

/**
 * React Query Hook - 取得最新枯旱預警與影響縣市
 * 每 10 分鐘更新一次（後端已有快取）
 *
 * @param {object} [options] - React Query 額外選項
 * @returns React Query 結果，data 為 DroughtAlert | null
 */
export const useDroughtAlert = (options) =>
  useQuery({
    queryKey: ['droughtAlert'],
    queryFn: fetchDroughtAlert,
    staleTime: 1000 * 60 * 10, // 10 分鐘
    ...options,
  });
