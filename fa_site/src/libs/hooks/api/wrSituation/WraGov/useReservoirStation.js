import { useQuery } from "react-query";
import request from "libs/api/request";

const fetchReservoirStation = async (params) => {
  const response = await request.get("/api/watergov/reservoir/station", { params });
  return response;
};

const useReservoirStation = (params, options) => useQuery({ queryKey: ["wraReservoirStation", params], queryFn: () => fetchReservoirStation(params), ...options });

export default useReservoirStation;
