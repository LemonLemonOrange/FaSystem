import { useQuery } from "react-query";
import request from "libs/api/request";

const fetchStatisticsWaterFacility = async (eventNo) => {
  const response = await request.get(`/api/watergov/statistics/water-facility/${eventNo}`);
  return response;
};

const useStatisticsWaterFacility = (eventNo, options) => useQuery({ queryKey: ["wraStatisticsWaterFacility", eventNo], queryFn: () => fetchStatisticsWaterFacility(eventNo), enabled: !!eventNo, ...options });

export default useStatisticsWaterFacility;
