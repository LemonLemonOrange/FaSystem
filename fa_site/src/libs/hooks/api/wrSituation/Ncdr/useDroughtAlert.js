import { useQuery } from "react-query";
import request from "libs/api/request";

const fetchDroughtAlert = async () => {
  try {
    const response = await request.get("/api/ncdr/drought-alert");
    return response;
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
