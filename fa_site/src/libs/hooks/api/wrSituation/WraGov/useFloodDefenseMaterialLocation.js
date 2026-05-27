import { useQuery } from "react-query";
import request from "libs/api/request";

const fetchFloodDefenseMaterialLocation = async (params) => {
  const response = await request.get("/api/watergov/flood-defense/material-location", { params });
  return response;
};

const useFloodDefenseMaterialLocation = (params, options) => useQuery({ queryKey: ["wraFloodDefenseMaterialLocation", params], queryFn: () => fetchFloodDefenseMaterialLocation(params), ...options });

export default useFloodDefenseMaterialLocation;
