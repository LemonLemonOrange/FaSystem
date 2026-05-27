import { useQuery } from "react-query";
import request from "libs/api/request";

const fetchWaterStation = async (params) => {
  const response = await request.get("/api/watergov/water/station", { params });
  return response;
};

const useWaterStation = (params, options) => useQuery({ queryKey: ["wraWaterStation", params], queryFn: () => fetchWaterStation(params), ...options });

export default useWaterStation;
