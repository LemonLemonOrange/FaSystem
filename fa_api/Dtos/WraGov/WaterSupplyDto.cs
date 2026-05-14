namespace fa_api.Dtos.WraGov
{
    /// <summary>
    /// 各地區供水狀況
    /// </summary>
    public class WaterSupplyConditionDto
    {
        /// <summary>
        /// 地區名稱
        /// </summary>
        public string AreaName { get; set; }

        /// <summary>
        /// 供水狀況代碼
        /// 1: 正常供水 (綠燈)
        /// 2: 減壓供水 (黃燈)
        /// 3: 減量供水 (橙燈)
        /// 4: 分區供水 (紅燈)
        /// </summary>
        public int SupplyStatus { get; set; }

        /// <summary>
        /// 供水狀況說明
        /// </summary>
        public string StatusDescription { get; set; }

        /// <summary>
        /// 限水措施說明
        /// </summary>
        public string RestrictionDescription { get; set; }

        /// <summary>
        /// 更新時間
        /// </summary>
        public string UpdateTime { get; set; }
    }
}