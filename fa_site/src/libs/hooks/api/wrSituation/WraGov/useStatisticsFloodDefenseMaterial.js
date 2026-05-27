import { useQuery } from "react-query";
import request from "libs/api/request";

const fetchStatisticsFloodDefenseMaterial = async (params) => {
  const response = await request.get("/api/watergov/statistics/flood-defense-material", { params });
  return response;
};

const useStatisticsFloodDefenseMaterial = (params, options) => useQuery({ queryKey: ["wraStatisticsFloodDefenseMaterial", params], queryFn: () => fetchStatisticsFloodDefenseMaterial(params), ...options });

export default useStatisticsFloodDefenseMaterial;
