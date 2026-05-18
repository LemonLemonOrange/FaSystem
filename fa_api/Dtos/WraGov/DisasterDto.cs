namespace fa_api.Dtos.WraGov
{
    /// <summary>
    /// 淹水災情明細
    /// API: GET /v1/Disaster/Flooding/{EventNo}
    /// </summary>
    public class DisasterFloodingDetailDto
    {
        /// <summary>
        /// 災情序號
        /// </summary>
        public int DisasterFloodingID { get; set; }

        /// <summary>
        /// 通報時間（yyyy-MM-dd HH:mm）
        /// </summary>
        public string Time { get; set; }

        /// <summary>
        /// 災情來源：1 經濟部水利署、2 EMIC、3 電視媒體、4 防汛護水志工、5 消防署、6 CHT、7 CCTV、8 其他
        /// </summary>
        public string SourceCode { get; set; }

        /// <summary>
        /// 來源說明
        /// </summary>
        public string SourceRemarks { get; set; }

        /// <summary>
        /// 資料來源序號
        /// </summary>
        public string SourceNo { get; set; }

        /// <summary>
        /// 災點分區
        /// </summary>
        public string OperatorName { get; set; }

        /// <summary>
        /// 縣市代碼
        /// </summary>
        public string CityCode { get; set; }

        /// <summary>
        /// 鄉鎮代碼
        /// </summary>
        public string TownCode { get; set; }

        /// <summary>
        /// 災情描述
        /// </summary>
        public string Situation { get; set; }

        /// <summary>
        /// 災害地點
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// 淹水深度
        /// </summary>
        public decimal? Depth { get; set; }

        /// <summary>
        /// 災情處置情形
        /// </summary>
        public string Treatment { get; set; }

        /// <summary>
        /// 是否退水
        /// </summary>
        public bool? IsReceded { get; set; }

        /// <summary>
        /// 退水時間（yyyy-MM-dd HH:mm）
        /// </summary>
        public string RecededDate { get; set; }

        /// <summary>
        /// 緯度（WGS84）
        /// </summary>
        public decimal? Latitude { get; set; }

        /// <summary>
        /// 經度（WGS84）
        /// </summary>
        public decimal? Longitude { get; set; }

        /// <summary>
        /// 災害種類：0 住戶、1 工(商)業區、2 農田/漁塭、3 道路、4 其他、5 待查
        /// </summary>
        public string Type { get; set; }
    }

    /// <summary>
    /// 水利設施災情明細
    /// API: GET /v1/Disaster/WaterFacility/{EventNo}
    /// </summary>
    public class DisasterWaterFacilityDetailDto
    {
        /// <summary>
        /// 災情序號
        /// </summary>
        public int WaterFacilityID { get; set; }

        /// <summary>
        /// 通報時間（yyyy-MM-dd HH:mm）
        /// </summary>
        public string Time { get; set; }

        /// <summary>
        /// 災點分區
        /// </summary>
        public string OperatorName { get; set; }

        /// <summary>
        /// 縣市代碼
        /// </summary>
        public string CityCode { get; set; }

        /// <summary>
        /// 鄉鎮代碼
        /// </summary>
        public string TownCode { get; set; }

        /// <summary>
        /// 情況說明
        /// </summary>
        public string Situation { get; set; }

        /// <summary>
        /// 處理說明
        /// </summary>
        public string Treatment { get; set; }

        /// <summary>
        /// 緯度（WGS84）
        /// </summary>
        public decimal? Latitude { get; set; }

        /// <summary>
        /// 經度（WGS84）
        /// </summary>
        public decimal? Longitude { get; set; }

        /// <summary>
        /// 災害類別：1 河堤、2 海堤、3 排水、4 水庫、5 水門、6 抽水站、7 其他
        /// </summary>
        public string Type { get; set; }
    }
}
