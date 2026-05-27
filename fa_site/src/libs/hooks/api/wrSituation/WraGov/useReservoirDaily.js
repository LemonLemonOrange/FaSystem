import { useQuery } from "react-query";
import request from "libs/api/request";

const fetchReservoirDaily = async (params) => {
  const response = await request.get("/api/watergov/reservoir/daily", { params });
  return response;
};

const useReservoirDaily = (params, options) => useQuery({ queryKey: ["wraReservoirDaily", params], queryFn: () => fetchReservoirDaily(params), ...options });

export default useReservoirDaily;
