import { useQuery } from "react-query";
import request from "libs/api/request";

const fetchRainStation = async (params) => {
  const response = await request.get("/v1/Rain/Station", { params });
  return response;
};

const useRainStation = (params, options) => useQuery({ queryKey: ["wraRainStation", params], queryFn: () => fetchRainStation(params), ...options });

export default useRainStation;
