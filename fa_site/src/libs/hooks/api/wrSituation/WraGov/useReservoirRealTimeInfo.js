import { useQuery } from "react-query";
import request from "libs/api/request";

const useReservoirRealTimeInfo = () => {
  return useQuery(["ReservoirRealTimeInfo"], async ({ signal }) => {
    const { data } = await request({
      method: "GET",
      url: "/api/WaterGov/reservoir/real-time-info",
      signal,
    });

    return data.data;
  });
};

export default useReservoirRealTimeInfo;