using System.Collections.Generic;

namespace fa_api.Dtos.WraGov
{
    /// <summary>
    /// 水庫測站基本資料
    /// </summary>
    public class ReservoirStationDto
    {
        /// <summary>
        /// 測站代碼
        /// </summary>
        public string StationNo { get; set; }

        /// <summary>
        /// 測站名稱
        /// </summary>
        public string StationName { get; set; }

        /// <summary>
        /// 縣市代碼
        /// </summary>
        public string CityCode { get; set; }

        /// <summary>
        /// 流域代碼
        /// </summary>
        public string BasinNo { get; set; }

        /// <summary>
        /// 流域名稱
        /// </summary>
        public string BasinName { get; set; }

        /// <summary>
        /// 有效容量
        /// </summary>
        public decimal? EffectiveCapacity { get; set; }

        /// <summary>
        /// 滿水位（公尺）
        /// </summary>
        public decimal? FullWaterHeight { get; set; }

        /// <summary>
        /// 呆水位（公尺）
        /// </summary>
        public decimal? DeadWaterHeight { get; set; }

        /// <summary>
        /// 總蓄水量
        /// </summary>
        public decimal Storage { get; set; }

        /// <summary>
        /// 緯度 (WGS84)
        /// </summary>
        public decimal? Latitude { get; set; }

        /// <summary>
        /// 經度 (WGS84)
        /// </summary>
        public decimal? Longitude { get; set; }

        /// <summary>
        /// 是否具防洪功能（0:否, 1:是）
        /// </summary>
        public int ProtectionFlood { get; set; }

        /// <summary>
        /// 水工構造類型（1:水庫, 2:攔河堰）
        /// </summary>
        public int HydraulicConstruction { get; set; }

        /// <summary>
        /// 重要性（1:主要, 0:次要）
        /// </summary>
        public int Importance { get; set; }
    }

    /// <summary>
    /// 水庫即時資訊
    /// </summary>
    public class ReservoirRealTimeInfoDto
    {
        /// <summary>
        /// 測站代碼
        /// </summary>
        public string StationNo { get; set; }

        /// <summary>
        /// 資料時間（yyyy-MM-dd HH:mm）
        /// </summary>
        public string Time { get; set; }

        /// <summary>
        /// 水位（公尺）
        /// </summary>
        public decimal WaterHeight { get; set; }

        /// <summary>
        /// 有效容量
        /// </summary>
        public decimal? EffectiveCapacity { get; set; }

        /// <summary>
        /// 有效蓄水量
        /// </summary>
        public decimal? EffectiveStorage { get; set; }

        /// <summary>
        /// 蓄水率
        /// </summary>
        public decimal? PercentageOfStorage { get; set; }

        /// <summary>
        /// 可用水量
        /// </summary>
        public decimal? OperationalStorage { get; set; }

        /// <summary>
        /// 當日累積雨量(mm)
        /// </summary>
        public decimal? AccumulatedRainfall { get; set; }

        /// <summary>
        /// 入流量(cms)
        /// </summary>
        public decimal? Inflow { get; set; }

        /// <summary>
        /// 出流量(cms)
        /// </summary>
        public decimal? Outflow { get; set; }

        /// <summary>
        /// 放流量(cms)
        /// </summary>
        public decimal? Discharge { get; set; }

        /// <summary>
        /// 防洪放流量(cms)
        /// </summary>
        public decimal? DischargeOfProtectionFlood { get; set; }

        /// <summary>
        /// 排砂放流量(cms)
        /// </summary>
        public decimal? DischargeOfEscapeSand { get; set; }

        /// <summary>
        /// 發電放流量(cms)
        /// </summary>
        public decimal? DischargeOfHydroelectric { get; set; }

        /// <summary>
        /// 其他放流量(cms)
        /// </summary>
        public decimal? DischargeOfOthers { get; set; }

        /// <summary>
        /// 狀態代碼（'0':蓄水, '1':洩水, '-1':排放）
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 預計洩洪時間（yyyy-MM-dd HH:mm）
        /// </summary>
        public string NextSpillTime { get; set; }
    }

    /// <summary>
    /// 水庫每日資料
    /// </summary>
    public class ReservoirDailyDto
    {
        /// <summary>
        /// 測站代碼
        /// </summary>
        public string StationNo { get; set; }

        /// <summary>
        /// 資料時間（yyyy-MM-dd HH:mm）
        /// </summary>
        public string Time { get; set; }

        /// <summary>
        /// 有效容量
        /// </summary>
        public decimal? EffectiveCapacity { get; set; }

        /// <summary>
        /// 呆水位（公尺）
        /// </summary>
        public decimal? DeadWaterHeight { get; set; }

        /// <summary>
        /// 滿水位（公尺）
        /// </summary>
        public decimal? FullWaterHeight { get; set; }

        /// <summary>
        /// 當日累積雨量(mm)
        /// </summary>
        public decimal? AccumulatedRainfall { get; set; }

        /// <summary>
        /// 當日總進水量
        /// </summary>
        public decimal? InflowTotal { get; set; }

        /// <summary>
        /// 當日總出水量
        /// </summary>
        public decimal? OutflowTotal { get; set; }
    }

    /// <summary>
    /// 水庫警示資料
    /// </summary>
    public class ReservoirWarningDto
    {
        /// <summary>
        /// 測站代碼
        /// </summary>
        public string StationNo { get; set; }

        /// <summary>
        /// 縣市代碼
        /// </summary>
        public string CityCode { get; set; }

        /// <summary>
        /// 鄉鎮代碼
        /// </summary>
        public string TownCode { get; set; }

        /// <summary>
        /// 資料時間（yyyy-MM-dd HH:mm）
        /// </summary>
        public string Time { get; set; }

        /// <summary>
        /// 水位（公尺）
        /// </summary>
        public decimal? WaterHeight { get; set; }

        /// <summary>
        /// 防洪放流量(cms)
        /// </summary>
        public decimal? DischargeOfProtectionFlood { get; set; }

        /// <summary>
        /// 放流量(cms)
        /// </summary>
        public decimal? Discharge { get; set; }

        /// <summary>
        /// 預計放流時間（yyyy-MM-dd HH:mm）
        /// </summary>
        public string NextSpillTime { get; set; }

        /// <summary>
        /// 狀態代碼（'0':蓄水, '1':洩水, '-1':排放）
        /// </summary>
        public string Status { get; set; }
    }

    /// <summary>
    /// 水庫影響區域
    /// </summary>
    public class ReservoirAffectedAreaDto
    {
        /// <summary>
        /// 水庫代碼
        /// </summary>
        public string StationNo { get; set; }

        /// <summary>
        /// 縣市代碼
        /// </summary>
        public string CityCode { get; set; }

        /// <summary>
        /// 鄉鎮代碼
        /// </summary>
        public string TownCode { get; set; }
    }

    #region 原有的 DTO（保留）

    /// <summary>
    /// 水庫即時水情統計資料
    /// </summary>
    public class ReservoirDataDto
    {
        /// <summary>
        /// 水庫代碼
        /// </summary>
        public string ReservoirIdentifier { get; set; }

        /// <summary>
        /// 水庫名稱
        /// </summary>
        public string ReservoirName { get; set; }

        /// <summary>
        /// 有效蓄水量 (萬立方公尺)
        /// </summary>
        public decimal EffectiveCapacity { get; set; }

        /// <summary>
        /// 有效容量 (萬立方公尺)
        /// </summary>
        public decimal EffectiveWaterStorageCapacity { get; set; }

        /// <summary>
        /// 蓄水百分比 (%)
        /// </summary>
        public decimal Percentage { get; set; }

        /// <summary>
        /// 進水量 (立方公尺/秒)
        /// </summary>
        public decimal Inflow { get; set; }

        /// <summary>
        /// 出水量 (立方公尺/秒)
        /// </summary>
        public decimal Outflow { get; set; }

        /// <summary>
        /// 水位 (公尺)
        /// </summary>
        public decimal WaterLevel { get; set; }

        /// <summary>
        /// 滿水位 (公尺)
        /// </summary>
        public decimal FullWaterLevel { get; set; }

        /// <summary>
        /// 有效水深 (公尺)
        /// </summary>
        public decimal EffectiveWaterDepth { get; set; }

        /// <summary>
        /// 更新時間
        /// </summary>
        public string UpdateTime { get; set; }
    }

    /// <summary>
    /// 水庫每日營運狀況
    /// </summary>
    public class ReservoirOperationDto
    {
        /// <summary>
        /// 水庫名稱
        /// </summary>
        public string ReservoirName { get; set; }

        /// <summary>
        /// 營運日期
        /// </summary>
        public string OperationDate { get; set; }

        /// <summary>
        /// 集水區累積雨量 (毫米)
        /// </summary>
        public decimal AccumulatedRainfall { get; set; }

        /// <summary>
        /// 放水量 (萬立方公尺)
        /// </summary>
        public decimal Discharge { get; set; }

        /// <summary>
        /// 溢洪道放水量 (萬立方公尺)
        /// </summary>
        public decimal SpillwayDischarge { get; set; }

        /// <summary>
        /// 發電量 (萬度)
        /// </summary>
        public decimal PowerGeneration { get; set; }
    }

    /// <summary>
    /// 水庫壩頂溢洪道放水警戒
    /// </summary>
    public class OverflowAlarmDto
    {
        /// <summary>
        /// 水庫名稱
        /// </summary>
        public string ReservoirName { get; set; }

        /// <summary>
        /// 警戒狀態 (0:正常, 1:警戒)
        /// </summary>
        public int AlarmStatus { get; set; }

        /// <summary>
        /// 放水量 (立方公尺/秒)
        /// </summary>
        public decimal DischargeRate { get; set; }

        /// <summary>
        /// 警戒說明
        /// </summary>
        public string AlarmDescription { get; set; }

        /// <summary>
        /// 更新時間
        /// </summary>
        public string UpdateTime { get; set; }
    }

    #endregion
}