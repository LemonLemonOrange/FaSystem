namespace fa_api.Dtos.WraGov
{
    /// <summary>
    /// 水位站基本資料
    /// </summary>
    public class WaterStationDto
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
        /// 水位站所在地
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 一級警戒水位（公尺）
        /// </summary>
        public decimal? WarningLevel1 { get; set; }

        /// <summary>
        /// 二級警戒水位（公尺）
        /// </summary>
        public decimal? WarningLevel2 { get; set; }

        /// <summary>
        /// 三級警戒水位（公尺）
        /// </summary>
        public decimal? WarningLevel3 { get; set; }

        /// <summary>
        /// 最高水位（公尺）
        /// </summary>
        public decimal? TopLevel { get; set; }

        /// <summary>
        /// 計畫洪水位（公尺）
        /// </summary>
        public decimal? PlanFloodLevel { get; set; }

        /// <summary>
        /// 緯度(WGS84)
        /// </summary>
        public decimal? Latitude { get; set; }

        /// <summary>
        /// 經度(WGS84)
        /// </summary>
        public decimal? Longitude { get; set; }
    }

    /// <summary>
    /// 水位即時資訊
    /// </summary>
    public class WaterRealTimeInfoDto
    {
        /// <summary>
        /// 測站代碼
        /// </summary>
        public string StationNo { get; set; }

        /// <summary>
        /// 資料時間（格式：yyyy-MM-dd HH:mm）
        /// </summary>
        public string Time { get; set; }

        /// <summary>
        /// 水位（公尺）
        /// </summary>
        public decimal WaterLevel { get; set; }
    }

    /// <summary>
    /// 水位警示資料
    /// </summary>
    public class WaterWarningDto
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
        /// 資料時間（格式：yyyy-MM-dd HH:mm）
        /// </summary>
        public string Time { get; set; }

        /// <summary>
        /// 水位（公尺）
        /// </summary>
        public decimal WaterLevel { get; set; }

        /// <summary>
        /// 警戒級別
        /// </summary>
        public int WarningLevel { get; set; }
    }

    /// <summary>
    /// 水位測站即時資料
    /// </summary>
    public class WaterLevelDataDto
    {
        /// <summary>
        /// 測站編號
        /// </summary>
        public string StationNo { get; set; }

        /// <summary>
        /// 測站名稱
        /// </summary>
        public string StationName { get; set; }

        /// <summary>
        /// 縣市
        /// </summary>
        public string County { get; set; }

        /// <summary>
        /// 河川名稱
        /// </summary>
        public string RiverName { get; set; }

        /// <summary>
        /// 水位 (公尺)
        /// </summary>
        public decimal WaterLevel { get; set; }

        /// <summary>
        /// 警戒水位 (公尺)
        /// </summary>
        public decimal AlertLevel { get; set; }

        /// <summary>
        /// 觀測時間
        /// </summary>
        public string ObservationTime { get; set; }
    }

    /// <summary>
    /// 雨量測站即時資料
    /// </summary>
    public class RainfallDataDto
    {
        /// <summary>
        /// 測站編號
        /// </summary>
        public string StationNo { get; set; }

        /// <summary>
        /// 測站名稱
        /// </summary>
        public string StationName { get; set; }

        /// <summary>
        /// 縣市
        /// </summary>
        public string County { get; set; }

        /// <summary>
        /// 累積雨量 (毫米)
        /// </summary>
        public decimal Rainfall { get; set; }

        /// <summary>
        /// 時雨量 (毫米)
        /// </summary>
        public decimal HourlyRainfall { get; set; }

        /// <summary>
        /// 觀測時間
        /// </summary>
        public string ObservationTime { get; set; }
    }
}