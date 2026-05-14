using System;
using System.Collections.Generic;

namespace fa_api.Models
{
    /// <summary>
    /// 乾旱影響區域
    /// </summary>
    public class DroughtArea
    {
        /// <summary>
        /// 受影響縣市名稱（例如：新竹縣、台中市）
        /// </summary>
        public string AreaDesc { get; set; }
    }

    /// <summary>
    /// 乾旱警示詳細資訊
    /// </summary>
    public class DroughtInfo
    {
        /// <summary>
        /// 警示嚴重程度
        /// - "Minor"    藍燈：水情提醒
        /// - "Moderate" 黃燈：減壓供水
        /// - "Severe"   橙燈：減量供水
        /// - "Extreme"  紅燈：分區供水及限水點用水
        /// </summary>
        public string Severity { get; set; }

        /// <summary>
        /// 標題 (例如：115年7月份水利署枯旱預警）通報)
        /// </summary>
        public string Headline { get; set; }

        /// <summary>
        /// 詳細說明
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 警示生效時間
        /// </summary>
        public string Effective { get; set; }

        /// <summary>
        /// 警示過期時間
        /// </summary>
        public string Expires { get; set; }

        /// <summary>
        /// 受影響地區列表
        /// </summary>
        public List<DroughtArea> Area { get; set; }
    }

    /// <summary>
    /// 乾旱警示完整資訊
    /// </summary>
    public class DroughtAlert
    {
        /// <summary>
        /// 警示識別碼(例如：WRA_Drought_20260427193326)
        /// </summary>
        public string Identifier { get; set; }

        /// <summary>
        /// 發送時間
        /// </summary>
        public string Sent { get; set; }

        /// <summary>
        /// 狀態(Actual / Test)
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 警示詳細內容 (含 severity 與影響區域）
        /// </summary>
        public DroughtInfo Info { get; set; }
    }
}