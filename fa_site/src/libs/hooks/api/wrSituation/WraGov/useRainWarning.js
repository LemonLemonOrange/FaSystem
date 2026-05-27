import { useQuery } from "react-query";
import request from "libs/api/request";

const fetchRainWarning = async (params) => {
  const response = await request.get("/v1/Rain/Warning", { params });
  return response;
};

const useRainWarning = (params, options) => useQuery({ queryKey: ["wraRainWarning", params], queryFn: () => fetchRainWarning(params), ...options });

export default useRainWarning;
