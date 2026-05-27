import { useQuery } from "react-query";
import request from "libs/api/request";

const fetchWaterWarning = async (params) => {
  const response = await request.get("/api/watergov/water/warning", { params });
  return response;
};

const useWaterWarning = (params, options) => useQuery({ queryKey: ["wraWaterWarning", params], queryFn: () => fetchWaterWarning(params), ...options });

export default useWaterWarning;
