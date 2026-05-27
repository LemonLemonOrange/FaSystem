import { useQuery } from "react-query";
import request from "libs/api/request";

const fetchTown = async (city, params) => {
  const response = await request.get(`/api/watergov/basic/${city}/town`, { params });
  return response;
};

const useTown = (city, params, options) => useQuery({ queryKey: ["wraTown", city, params], queryFn: () => fetchTown(city, params), enabled: !!city, ...options });

export default useTown;
