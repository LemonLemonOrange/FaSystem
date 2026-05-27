import { useQuery } from "react-query";
import request from "libs/api/request";

const fetchReservoirWarning = async (params) => {
  const response = await request.get("/api/watergov/reservoir/warning", { params });
  return response;
};

const useReservoirWarning = (params, options) => useQuery({ queryKey: ["wraReservoirWarning", params], queryFn: () => fetchReservoirWarning(params), ...options });

export default useReservoirWarning;
