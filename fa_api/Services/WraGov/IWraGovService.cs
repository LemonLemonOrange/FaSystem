using System.Collections.Generic;
using System.Threading.Tasks;
using fa_api.Dtos.WraGov;

namespace fa_api.Services.WraGov
{
    public interface IWraGovService
    {
        #region 基本資料

        Task<List<CityDto>> GetCityAsync();
        Task<List<TownDto>> GetTownByCityAsync(string cityCode);

        #endregion

        #region 水庫資料

        Task<List<ReservoirStationDto>> GetReservoirStationAsync();
        Task<List<ReservoirRealTimeInfoDto>> GetReservoirRealTimeInfoAsync();
        Task<List<ReservoirDailyDto>> GetReservoirDailyAsync();
        Task<List<ReservoirWarningDto>> GetReservoirWarningAsync();
        Task<List<ReservoirAffectedAreaDto>> GetReservoirAffectedAreaAsync();

        #endregion

        #region 雨量資料

        Task<List<RainStationDto>> GetRainStationAsync();
        Task<List<RainRealTimeInfoDto>> GetRainRealTimeInfoAsync();
        Task<List<RainWarningDto>> GetRainWarningAsync();
        Task<List<RainAffectedAreaDto>> GetRainAffectedAreaAsync();

        #endregion

        #region 事件資料

        Task<List<EventDto>> GetEventByYearAsync(int year);

        #endregion

        #region 統計資料

        Task<List<DisasterFloodingStatisticsDto>> GetStatisticsFloodingAsync(string eventNo);
        Task<List<DisasterWaterFacilityStatisticsDto>> GetStatisticsWaterFacilityAsync(string eventNo);
        Task<List<FloodDefenseOperatorDto>> GetStatisticsFloodDefenseMaterialAsync();

        #endregion

        #region 防汛資材（新增）

        /// <summary>
        /// 取得防汛資材位置資料
        /// API: https://fhy.wra.gov.tw/WraApi/v1/FloodDefense/MaterialLocation
        /// </summary>
        Task<List<FloodDefenseMaterialLocationDto>> GetFloodDefenseMaterialLocationAsync();

        #endregion

        #region 水庫相關（舊 API，保留）

        Task<List<ReservoirDataDto>> GetReservoirStatisticsAsync();
        Task<ReservoirDataDto> GetReservoirByNameAsync(string reservoirName);
        Task<List<ReservoirOperationDto>> GetReservoirOperationAsync();
        Task<List<OverflowAlarmDto>> GetOverflowAlarmAsync();

        #endregion

        #region 供水相關

        Task<List<WaterSupplyConditionDto>> GetWaterSupplyConditionAsync();

        #endregion

        #region 水位站相關

        Task<List<WaterStationDto>> GetWaterStationAsync();
        Task<List<WaterRealTimeInfoDto>> GetWaterRealTimeInfoAsync();
        Task<List<WaterWarningDto>> GetWaterWarningAsync();

        #endregion

        #region 水文相關

        Task<List<WaterLevelDataDto>> GetWaterLevelRealTimeAsync(string stationNo = null);
        Task<List<RainfallDataDto>> GetRainfallRealTimeAsync(string stationNo = null);

        #endregion
    }
}