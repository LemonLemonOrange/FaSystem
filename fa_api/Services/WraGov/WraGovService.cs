using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using fa_api.Dtos.WraGov;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace fa_api.Services.WraGov
{
    public class WraGovService : IWraGovService
    {
        private const string BASE_URL = "https://fhy.wra.gov.tw/WraApi/v1";

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<WraGovService> _logger;

        public WraGovService(IHttpClientFactory httpClientFactory, ILogger<WraGovService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        #region 基本資料

        /// <summary>
        /// 取得縣市資料
        /// </summary>
        public async Task<List<CityDto>> GetCityAsync()
        {
            var url = $"{BASE_URL}/Basic/City";
            return await FetchDataAsync<CityDto>(url, "縣市資料");
        }

        /// <summary>
        /// 取得指定縣市的鄉鎮資料
        /// </summary>
        public async Task<List<TownDto>> GetTownByCityAsync(string cityCode)
        {
            if (string.IsNullOrWhiteSpace(cityCode))
            {
                throw new ArgumentException("縣市代碼不可為空", nameof(cityCode));
            }

            // 使用 OData filter 語法
            var url = $"{BASE_URL}/Basic/Town?$filter=CityCode eq '{cityCode}'";
            return await FetchDataAsync<TownDto>(url, $"鄉鎮資料（{cityCode}）");
        }

        #endregion

        #region 水庫資料（新增完整 API）

        public async Task<List<ReservoirStationDto>> GetReservoirStationAsync()
        {
            var url = $"{BASE_URL}/Reservoir/Station";
            return await FetchDataAsync<ReservoirStationDto>(url, "水庫測站基本資料");
        }

        public async Task<List<ReservoirRealTimeInfoDto>> GetReservoirRealTimeInfoAsync()
        {
            var url = $"{BASE_URL}/Reservoir/RealTimeInfo";
            return await FetchDataAsync<ReservoirRealTimeInfoDto>(url, "水庫即時資訊");
        }

        public async Task<List<ReservoirDailyDto>> GetReservoirDailyAsync()
        {
            var url = $"{BASE_URL}/Reservoir/Daily";
            return await FetchDataAsync<ReservoirDailyDto>(url, "水庫每日資料");
        }

        public async Task<List<ReservoirWarningDto>> GetReservoirWarningAsync()
        {
            var url = $"{BASE_URL}/Reservoir/Warning";
            return await FetchDataAsync<ReservoirWarningDto>(url, "水庫警示資料");
        }

        public async Task<List<ReservoirAffectedAreaDto>> GetReservoirAffectedAreaAsync()
        {
            var url = $"{BASE_URL}/Reservoir/AffectedArea";
            return await FetchDataAsync<ReservoirAffectedAreaDto>(url, "水庫影響區域");
        }

        #endregion

        #region 水庫相關（舊 API，保留）

        public async Task<List<ReservoirDataDto>> GetReservoirStatisticsAsync()
        {
            var url = $"{BASE_URL}/Reservoir/Statistics";
            return await FetchDataAsync<ReservoirDataDto>(url, "水庫即時水情");
        }

        /// <summary>
        /// GetReservoirDataAsync — alias for GetReservoirStatisticsAsync（供 WaterGovController Legacy 區塊使用）
        /// </summary>
        public Task<List<ReservoirDataDto>> GetReservoirDataAsync()
            => GetReservoirStatisticsAsync();

        public async Task<ReservoirDataDto> GetReservoirByNameAsync(string reservoirName)
        {
            if (string.IsNullOrWhiteSpace(reservoirName))
            {
                throw new ArgumentException("水庫名稱不可為空", nameof(reservoirName));
            }

            var allReservoirs = await GetReservoirStatisticsAsync();
            var reservoir = allReservoirs.FirstOrDefault(r =>
                r.ReservoirName != null &&
                r.ReservoirName.Contains(reservoirName, StringComparison.OrdinalIgnoreCase));

            if (reservoir == null)
            {
                _logger.LogWarning($"找不到水庫: {reservoirName}");
                throw new KeyNotFoundException($"找不到水庫: {reservoirName}");
            }

            return reservoir;
        }

        public async Task<List<ReservoirOperationDto>> GetReservoirOperationAsync()
        {
            var url = $"{BASE_URL}/Reservoir/Operation";
            return await FetchDataAsync<ReservoirOperationDto>(url, "水庫營運狀況");
        }

        public async Task<List<OverflowAlarmDto>> GetOverflowAlarmAsync()
        {
            var url = $"{BASE_URL}/Reservoir/OverflowAlarm";
            return await FetchDataAsync<OverflowAlarmDto>(url, "放水警戒");
        }

        #endregion

        #region 供水相關

        public async Task<List<WaterSupplyConditionDto>> GetWaterSupplyConditionAsync()
        {
            var url = $"{BASE_URL}/Water/SupplyCondition";
            return await FetchDataAsync<WaterSupplyConditionDto>(url, "供水狀況");
        }

        #endregion

        #region 水位站相關

        public async Task<List<WaterStationDto>> GetWaterStationAsync()
        {
            var url = $"{BASE_URL}/WaterLevel/Station";
            return await FetchDataAsync<WaterStationDto>(url, "水位站基本資料");
        }

        public async Task<List<WaterRealTimeInfoDto>> GetWaterRealTimeInfoAsync()
        {
            var url = $"{BASE_URL}/WaterLevel/RealTimeInfo";
            return await FetchDataAsync<WaterRealTimeInfoDto>(url, "水位即時資訊");
        }

        public async Task<List<WaterWarningDto>> GetWaterWarningAsync()
        {
            var url = $"{BASE_URL}/WaterLevel/Warning";
            return await FetchDataAsync<WaterWarningDto>(url, "水位警示資料");
        }

        #endregion

        #region 水文相關

        public async Task<List<WaterLevelDataDto>> GetWaterLevelRealTimeAsync(string stationNo = null)
        {
            var url = string.IsNullOrEmpty(stationNo)
                ? $"{BASE_URL}/WaterLevel/RealTime"
                : $"{BASE_URL}/WaterLevel/RealTime?StationNo={stationNo}";
            return await FetchDataAsync<WaterLevelDataDto>(url, "即時水位");
        }

        public async Task<List<RainfallDataDto>> GetRainfallRealTimeAsync(string stationNo = null)
        {
            var url = string.IsNullOrEmpty(stationNo)
                ? $"{BASE_URL}/Rainfall/RealTime"
                : $"{BASE_URL}/Rainfall/RealTime?StationNo={stationNo}";
            return await FetchDataAsync<RainfallDataDto>(url, "即時雨量");
        }

        #endregion

        #region 雨量資料

        public async Task<List<RainStationDto>> GetRainStationAsync()
        {
            var url = $"{BASE_URL}/Rainfall/Station";
            return await FetchDataAsync<RainStationDto>(url, "雨量站基本資料");
        }

        public async Task<List<RainRealTimeInfoDto>> GetRainRealTimeInfoAsync()
        {
            var url = $"{BASE_URL}/Rainfall/RealTimeInfo";
            return await FetchDataAsync<RainRealTimeInfoDto>(url, "雨量即時資訊");
        }

        public async Task<List<RainWarningDto>> GetRainWarningAsync()
        {
            var url = $"{BASE_URL}/Rainfall/Warning";
            return await FetchDataAsync<RainWarningDto>(url, "雨量警示資料");
        }

        public async Task<List<RainAffectedAreaDto>> GetRainAffectedAreaAsync()
        {
            var url = $"{BASE_URL}/Rainfall/AffectedArea";
            return await FetchDataAsync<RainAffectedAreaDto>(url, "雨量影響區域");
        }

        #endregion

        #region 事件資料

        public async Task<List<EventDto>> GetEventByYearAsync(int year)
        {
            if (year < 1900 || year > 2100)
            {
                throw new ArgumentException("年份範圍必須在 1900-2100 之間", nameof(year));
            }

            var url = $"{BASE_URL}/Event/{year}";
            return await FetchDataAsync<EventDto>(url, $"事件資料（{year}年）");
        }

        #endregion

        #region 統計資料

        public async Task<List<DisasterFloodingStatisticsDto>> GetStatisticsFloodingAsync(string eventNo)
        {
            if (string.IsNullOrWhiteSpace(eventNo))
            {
                throw new ArgumentException("事件編號不可為空", nameof(eventNo));
            }

            var url = $"{BASE_URL}/Statistics/Flooding/{eventNo}";
            return await FetchDataAsync<DisasterFloodingStatisticsDto>(url, $"淹水統計資料（{eventNo}）");
        }

        public async Task<List<DisasterWaterFacilityStatisticsDto>> GetStatisticsWaterFacilityAsync(string eventNo)
        {
            if (string.IsNullOrWhiteSpace(eventNo))
            {
                throw new ArgumentException("事件編號不可為空", nameof(eventNo));
            }

            var url = $"{BASE_URL}/Statistics/WaterFacility/{eventNo}";
            return await FetchDataAsync<DisasterWaterFacilityStatisticsDto>(url, $"水利設施統計資料（{eventNo}）");
        }

        public async Task<List<FloodDefenseOperatorDto>> GetStatisticsFloodDefenseMaterialAsync()
        {
            var url = $"{BASE_URL}/Statistics/FloodDefenseMaterial";
            return await FetchDataAsync<FloodDefenseOperatorDto>(url, "防汛資材統計資料");
        }

        #endregion

        #region 防汛資材

        public async Task<List<FloodDefenseMaterialLocationDto>> GetFloodDefenseMaterialLocationAsync()
        {
            var url = $"{BASE_URL}/FloodDefense/MaterialLocation";
            return await FetchDataAsync<FloodDefenseMaterialLocationDto>(url, "防汛資材位置資料");
        }

        #endregion

        #region 輔助方法

        /// <summary>
        /// 通用的資料取得方法
        /// </summary>
        private async Task<List<T>> FetchDataAsync<T>(string url, string dataType)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                _logger.LogInformation($"開始取得{dataType}: {url}");

                var response = await client.GetStringAsync(url);
                var jsonData = JArray.Parse(response);
                var results = jsonData.ToObject<List<T>>();

                _logger.LogInformation($"✅ 成功解析 {results.Count} 筆{dataType}");
                return results;
            }
            catch (HttpRequestException httpEx)
            {
                _logger.LogError(httpEx, $"HTTP 請求失敗: {url}");
                throw new InvalidOperationException($"無法連接到水利署 API", httpEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"取得{dataType}時發生錯誤");
                throw;
            }
        }

        #endregion
    }
}