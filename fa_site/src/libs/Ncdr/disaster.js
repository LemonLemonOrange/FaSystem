import { useQuery } from "react-query";
import axios from "axios";

const API_BASE_URL = globalThis?.process?.env?.REACT_APP_API_BASE_URL || "http://localhost:65326";

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
 * React Query Hook - ?��??�?�枯?��?警�?影響�??
 * �?10 ?��??�新一次�?後端已�?快�?�? *
 * @param {object} [options] - React Query 額�??��?
 * @returns React Query 結�?，data ??DroughtAlert | null
 */
export const useDroughtAlert = (options) =>
  useQuery({
    queryKey: ["droughtAlert"],
    queryFn: fetchDroughtAlert,
    staleTime: 1000 * 60 * 10, // 10 ?��?
    ...options,
  });
