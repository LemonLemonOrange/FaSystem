namespace fa_api.Dtos.WraGov
{
    /// <summary>
    /// 雨量站基本資料
    /// </summary>
    public class RainStationDto
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
        /// 測站所在地
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 緯度 (WGS84)
        /// </summary>
        public decimal Latitude { get; set; }

        /// <summary>
        /// 經度 (WGS84)
        /// </summary>
        public decimal Longitude { get; set; }
    }

    /// <summary>
    /// 雨量即時資訊
    /// </summary>
    public class RainRealTimeInfoDto
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
        /// 10 分鐘累積雨量(mm)
        /// </summary>
        public decimal M10 { get; set; }

        /// <summary>
        /// 1 小時累積雨量(mm)
        /// </summary>
        public decimal H1 { get; set; }

        /// <summary>
        /// 3 小時累積雨量(mm)
        /// </summary>
        public decimal H3 { get; set; }

        /// <summary>
        /// 6 小時累積雨量(mm)
        /// </summary>
        public decimal H6 { get; set; }

        /// <summary>
        /// 12 小時累積雨量(mm)
        /// </summary>
        public decimal H12 { get; set; }

        /// <summary>
        /// 24 小時累積雨量(mm)
        /// </summary>
        public decimal H24 { get; set; }
    }

    /// <summary>
    /// 雨量警示資料
    /// </summary>
    public class RainWarningDto
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
        /// 10 分鐘累積雨量(mm)
        /// </summary>
        public decimal M10 { get; set; }

        /// <summary>
        /// 1 小時累積雨量(mm)
        /// </summary>
        public decimal H1 { get; set; }

        /// <summary>
        /// 3 小時累積雨量(mm)
        /// </summary>
        public decimal H3 { get; set; }

        /// <summary>
        /// 6 小時累積雨量(mm)
        /// </summary>
        public decimal H6 { get; set; }

        /// <summary>
        /// 12 小時累積雨量(mm)
        /// </summary>
        public decimal H12 { get; set; }

        /// <summary>
        /// 24 小時累積雨量(mm)
        /// </summary>
        public decimal H24 { get; set; }

        /// <summary>
        /// 警戒級別
        /// </summary>
        public int WarningLevel { get; set; }

        /// <summary>
        /// 影響範圍
        /// </summary>
        public string AffectedArea { get; set; }
    }

    /// <summary>
    /// 雨量影響區域
    /// </summary>
    public class RainAffectedAreaDto
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
        /// 二級警戒 1 小時雨量門檻(mm)
        /// </summary>
        public decimal AlertLevel2_H1 { get; set; }

        /// <summary>
        /// 二級警戒 3 小時雨量門檻(mm)
        /// </summary>
        public decimal AlertLevel2_H3 { get; set; }

        /// <summary>
        /// 二級警戒 6 小時雨量門檻(mm)
        /// </summary>
        public decimal AlertLevel2_H6 { get; set; }

        /// <summary>
        /// 一級警戒 1 小時雨量門檻(mm)
        /// </summary>
        public decimal AlertLevel1_H1 { get; set; }

        /// <summary>
        /// 一級警戒 3 小時雨量門檻(mm)
        /// </summary>
        public decimal AlertLevel1_H3 { get; set; }

        /// <summary>
        /// 一級警戒 6 小時雨量門檻(mm)
        /// </summary>
        public decimal AlertLevel1_H6 { get; set; }

        /// <summary>
        /// 影響範圍
        /// </summary>
        public string AffectedArea { get; set; }
    }
}