import { useQuery } from "react-query";
import request from "libs/api/request";

const fetchReservoirAffectedArea = async (params) => {
  const response = await request.get("/api/watergov/reservoir/affected-area", { params });
  return response;
};

const useReservoirAffectedArea = (params, options) => useQuery({ queryKey: ["wraReservoirAffectedArea", params], queryFn: () => fetchReservoirAffectedArea(params), ...options });

export default useReservoirAffectedArea;
