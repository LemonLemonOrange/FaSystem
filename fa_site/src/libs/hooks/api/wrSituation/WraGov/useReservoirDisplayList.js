import { useQuery } from "react-query";
import request from "libs/api/request";

const fetchReservoirDisplayList = async (params) => {
  const response = await request.get("/api/watergov/reservoir/display-list", { params });
  if (Array.isArray(response)) {
    return response;
  }

  if (Array.isArray(response?.data)) {
    return response.data;
  }

  if (Array.isArray(response?.value)) {
    return response.value;
  }

  return [];
};

const useReservoirDisplayList = (params, options) =>
  useQuery({
    queryKey: ["wraReservoirDisplayList", params],
    queryFn: () => fetchReservoirDisplayList(params),
    ...options,
  });

export default useReservoirDisplayList;
