using System.Collections.Generic;
using System.Threading.Tasks;
using fa_api.Dtos.WraGov;

namespace fa_api.Services.WraGov
{
    public interface IWraGovService
    {
        #region �򥻸��

        Task<List<CityDto>> GetCityAsync();
        Task<List<TownDto>> GetTownByCityAsync(string cityCode);

        #endregion

        #region ���w���

        Task<List<ReservoirStationDto>> GetReservoirStationAsync();
        Task<List<ReservoirRealTimeInfoDto>> GetReservoirRealTimeInfoAsync();
        Task<List<ReservoirDailyDto>> GetReservoirDailyAsync();
        Task<List<ReservoirWarningDto>> GetReservoirWarningAsync();
        Task<List<ReservoirAffectedAreaDto>> GetReservoirAffectedAreaAsync();

        #endregion

        #region �B�q���

        Task<List<RainStationDto>> GetRainStationAsync();
        Task<List<RainRealTimeInfoDto>> GetRainRealTimeInfoAsync();
        Task<List<RainWarningDto>> GetRainWarningAsync();
        Task<List<RainAffectedAreaDto>> GetRainAffectedAreaAsync();

        #endregion

        #region �ƥ���

        Task<List<EventDto>> GetEventByYearAsync(int year);

        #endregion

        #region �έp���

        Task<List<DisasterFloodingStatisticsDto>> GetStatisticsFloodingAsync(string eventNo);
        Task<List<DisasterWaterFacilityStatisticsDto>> GetStatisticsWaterFacilityAsync(string eventNo);
        Task<List<FloodDefenseOperatorDto>> GetStatisticsFloodDefenseMaterialAsync();

        #endregion

        #region ���ĸ���]�s�W�^

        /// <summary>
        /// ���o���ĸ����m���
        /// API: https://fhy.wra.gov.tw/WraApi/v1/FloodDefense/MaterialLocation
        /// </summary>
        Task<List<FloodDefenseMaterialLocationDto>> GetFloodDefenseMaterialLocationAsync();

        #endregion

        #region 水庫舊版 API（Legacy，保留）

        Task<List<ReservoirDataDto>> GetReservoirStatisticsAsync();
        Task<List<ReservoirDataDto>> GetReservoirDataAsync();  // alias for GetReservoirStatisticsAsync
        Task<ReservoirDataDto> GetReservoirByNameAsync(string reservoirName);
        Task<List<ReservoirOperationDto>> GetReservoirOperationAsync();
        Task<List<OverflowAlarmDto>> GetOverflowAlarmAsync();

        #endregion

        #region �Ѥ�����

        Task<List<WaterSupplyConditionDto>> GetWaterSupplyConditionAsync();

        #endregion

        #region ���쯸����

        Task<List<WaterStationDto>> GetWaterStationAsync();
        Task<List<WaterRealTimeInfoDto>> GetWaterRealTimeInfoAsync();
        Task<List<WaterWarningDto>> GetWaterWarningAsync();

        #endregion

        #region �������

        Task<List<WaterLevelDataDto>> GetWaterLevelRealTimeAsync(string stationNo = null);
        Task<List<RainfallDataDto>> GetRainfallRealTimeAsync(string stationNo = null);

        #endregion
    }
}