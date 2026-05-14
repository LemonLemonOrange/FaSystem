namespace fa_api.Dtos.WraGov
{
    /// <summary>
    /// 防汛資材位置資料
    /// </summary>
    public class FloodDefenseMaterialLocationDto
    {
        /// <summary>
        /// 河川分署
        /// </summary>
        public string OperatorName { get; set; }

        /// <summary>
        /// 縣市代碼
        /// </summary>
        public string CityCode { get; set; }

        /// <summary>
        /// 資材類型（'1':防汛倉庫, '2':防汛設置點）
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 水系
        /// </summary>
        public string Watershed { get; set; }

        /// <summary>
        /// 河川
        /// </summary>
        public string River { get; set; }

        /// <summary>
        /// 備註
        /// </summary>
        public string Remarks { get; set; }

        /// <summary>
        /// 來源資料更新時間
        /// </summary>
        public string SrcUpdateTime { get; set; }
    }
}