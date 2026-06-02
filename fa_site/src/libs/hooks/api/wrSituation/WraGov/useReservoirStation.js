import { useQuery } from "react-query";
import request from "libs/api/request";

const asArray = (value) => {
  if (Array.isArray(value)) return value;
  if (Array.isArray(value?.data)) return value.data;
  if (Array.isArray(value?.value)) return value.value;
  return [];
};

const fetchReservoirStation = async (params) => {
  const response = await request.get("/api/watergov/reservoir/station", { params });
  return asArray(response);
};

const useReservoirStation = (params, options) => useQuery({ queryKey: ["wraReservoirStation", params], queryFn: () => fetchReservoirStation(params), ...options });

export default useReservoirStation;
