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
 * React Query Hook - ?–å??€?°æž¯?±é?è­¦è?å½±éŸ¿ç¸??
 * æ¯?10 ?†é??´æ–°ä¸€æ¬¡ï?å¾Œç«¯å·²æ?å¿«å?ï¼? *
 * @param {object} [options] - React Query é¡å??¸é?
 * @returns React Query çµæ?ï¼Œdata ??DroughtAlert | null
 */
export const useDroughtAlert = (options) =>
  useQuery({
    queryKey: ['droughtAlert'],
    queryFn: fetchDroughtAlert,
    staleTime: 1000 * 60 * 10, // 10 ?†é?
    ...options,
  });
