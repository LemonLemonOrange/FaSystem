import { useQuery } from "react-query";
import request from "libs/api/request";

const asArray = (value) => {
  if (Array.isArray(value)) return value;
  if (Array.isArray(value?.data)) return value.data;
  if (Array.isArray(value?.value)) return value.value;
  return [];
};

const useReservoirRealTimeInfo = () => {
  return useQuery(["ReservoirRealTimeInfo"], async ({ signal }) => {
    const response = await request({
      method: "GET",
      url: "/api/watergov/reservoir/real-time-info",
      signal,
    });

    return asArray(response);
  });
};

export default useReservoirRealTimeInfo;
