import { useQuery } from "react-query";
import request from "libs/api/request";

const fetchWaterRealTimeInfo = async (params) => {
  const response = await request.get("/api/watergov/water/real-time-info", { params });
  return response;
};

const useWaterRealTimeInfo = (params, options) => useQuery({ queryKey: ["wraWaterRealTimeInfo", params], queryFn: () => fetchWaterRealTimeInfo(params), ...options });

export default useWaterRealTimeInfo;
