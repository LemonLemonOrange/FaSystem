import { useQuery } from "react-query";
import request from "libs/api/request";

const fetchRainRealTimeInfo = async (params) => {
  const response = await request.get("/v1/Rain/RealTimeInfo", { params });
  return response;
};

const useRainRealTimeInfo = (params, options) => useQuery({ queryKey: ["wraRainRealTimeInfo", params], queryFn: () => fetchRainRealTimeInfo(params), ...options });

export default useRainRealTimeInfo;
