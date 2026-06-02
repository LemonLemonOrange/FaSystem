using System;

namespace fa_api.Dtos.WraGov
{
    /// <summary>
    /// 前端顯示用的水庫資訊
    /// </summary>
    public class ReservoirDisplayDto
    {
        /// <summary>
        /// 水庫名稱
        /// </summary>
        public string ReservoirName { get; set; }

        /// <summary>
        /// 經度 (WGS84)
        /// </summary>
        public decimal Longitude { get; set; }

        /// <summary>
        /// 緯度 (WGS84)
        /// </summary>
        public decimal Latitude { get; set; }

        /// <summary>
        /// 低水位百分比門檻 (%)
        /// </summary>
        public decimal? LowLevelPercentage { get; set; }

        /// <summary>
        /// 中水位差異百分比門檻 (%)
        /// </summary>
        public decimal? MiddleLevelPercentage { get; set; }

        /// <summary>
        /// 設定建立時間
        /// </summary>
        public DateTime ConfigCreateTime { get; set; }
    }
}