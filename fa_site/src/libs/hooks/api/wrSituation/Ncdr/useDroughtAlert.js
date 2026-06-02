import { useQuery } from "react-query";
import request from "libs/api/request";

const isExpiredByHeader = (expiresHeader) => {
  if (!expiresHeader) {
    return false;
  }

  const expiresTime = Date.parse(expiresHeader);
  if (Number.isNaN(expiresTime)) {
    return false;
  }

  return expiresTime <= Date.now();
};

const fetchDroughtAlert = async () => {
  try {
    const response = await request.get("/api/ncdr/drought-alert", { rawResponse: true });

    if (isExpiredByHeader(response.headers?.expires)) {
      return null;
    }

    return response.data;
  } catch (error) {
    if (error.response?.status === 404) {
      return null;
    }
    throw error;
  }
};

const useDroughtAlert = (options) =>
  useQuery({
    queryKey: ["droughtAlert"],
    queryFn: fetchDroughtAlert,
    staleTime: 1000 * 60 * 10,
    ...options,
  });

export default useDroughtAlert;
