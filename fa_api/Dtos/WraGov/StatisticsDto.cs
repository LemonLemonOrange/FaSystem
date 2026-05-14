using System.Collections.Generic;

namespace fa_api.Dtos.WraGov
{
    /// <summary>
    /// 睺参璸戈
    /// </summary>
    public class DisasterFloodingStatisticsDto
    {
        /// <summary>
        /// 郡カ絏
        /// </summary>
        public string CityCode { get; set; }

        /// <summary>
        /// 紇臫秏马计
        /// </summary>
        public int? TownCount { get; set; }

        /// <summary>
        /// 癶计
        /// </summary>
        public int? RecededCount { get; set; }

        /// <summary>
        /// ゼ癶计
        /// </summary>
        public int? FloodingCount { get; set; }

        /// <summary>
        /// ╝甡羆计
        /// </summary>
        public int? Total { get; set; }
    }

    /// <summary>
    /// 砞琁参璸戈
    /// </summary>
    public class DisasterWaterFacilityStatisticsDto
    {
        /// <summary>
        /// 郡カ絏
        /// </summary>
        public string CityCode { get; set; }

        /// <summary>
        /// 確计秖
        /// </summary>
        public int? RepairedCount { get; set; }

        /// <summary>
        /// 穖い计秖
        /// </summary>
        public int? RepairingCount { get; set; }

        /// <summary>
        /// 羆计秖
        /// </summary>
        public int? Total { get; set; }
    }

    /// <summary>
    /// ňδ戈兜ヘ
    /// </summary>
    public class FloodDefenseMaterialItem
    {
        /// <summary>
        /// 戈嘿
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 羆计秖
        /// </summary>
        public int Total { get; set; }
    }

    /// <summary>
    /// ňδ戈参璸ㄌ猠だ竝
    /// </summary>
    public class FloodDefenseOperatorDto
    {
        /// <summary>
        /// 猠だ竝
        /// </summary>
        public string OperatorName { get; set; }

        /// <summary>
        /// ňδ戈睲虫
        /// </summary>
        public List<FloodDefenseMaterialItem> Material { get; set; }
    }
}