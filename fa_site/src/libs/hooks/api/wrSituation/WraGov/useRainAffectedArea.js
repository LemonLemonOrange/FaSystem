import { useQuery } from "react-query";
import request from "libs/api/request";

const fetchRainAffectedArea = async (params) => {
  const response = await request.get("/v1/Rain/AffectedArea", { params });
  return response;
};

const useRainAffectedArea = (params, options) => useQuery({ queryKey: ["wraRainAffectedArea", params], queryFn: () => fetchRainAffectedArea(params), ...options });

export default useRainAffectedArea;
