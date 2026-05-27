import { useQuery } from "react-query";
import request from "libs/api/request";

const fetchStatisticsFlooding = async (eventNo) => {
  const response = await request.get(`/api/watergov/statistics/flooding/${eventNo}`);
  return response;
};

const useStatisticsFlooding = (eventNo, options) => useQuery({ queryKey: ["wraStatisticsFlooding", eventNo], queryFn: () => fetchStatisticsFlooding(eventNo), enabled: !!eventNo, ...options });

export default useStatisticsFlooding;
