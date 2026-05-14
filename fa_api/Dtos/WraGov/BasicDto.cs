namespace fa_api.Dtos.WraGov
{
    /// <summary>
    /// 縣市資料
    /// </summary>
    public class CityDto
    {
        /// <summary>
        /// 縣市代碼
        /// </summary>
        public string CityCode { get; set; }

        /// <summary>
        /// 縣市名稱（中文）
        /// </summary>
        public string CityName_Ch { get; set; }

        /// <summary>
        /// 縣市名稱（英文）
        /// </summary>
        public string CityName_En { get; set; }
    }

    /// <summary>
    /// 鄉鎮資料
    /// </summary>
    public class TownDto
    {
        /// <summary>
        /// 鄉鎮代碼
        /// </summary>
        public string TownCode { get; set; }

        /// <summary>
        /// 鄉鎮名稱
        /// </summary>
        public string TownName { get; set; }
    }
}