import { useQuery } from "react-query";
import request from "libs/api/request";

const fetchEventByYear = async (year, params) => {
  const response = await request.get(`/v1/Event/Year/${year}`, { params });
  return response;
};

const useEventByYear = (year, params, options) => useQuery({ queryKey: ["wraEvent", year, params], queryFn: () => fetchEventByYear(year, params), enabled: !!year, ...options });

export default useEventByYear;
