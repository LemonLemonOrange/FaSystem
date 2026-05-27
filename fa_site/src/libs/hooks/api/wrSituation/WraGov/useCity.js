import { useQuery } from "react-query";
import request from "libs/api/request";

const fetchCity = async (params) => {
  const response = await request.get("/api/watergov/basic/city", { params });
  return response;
};

const useCity = (params, options) => useQuery({ queryKey: ["wraCity", params], queryFn: () => fetchCity(params), ...options });

export default useCity;
