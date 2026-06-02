using System.Collections.Generic;

namespace fa_api.Services.DailyReport
{
    /// <summary>
    /// 每日報告設定
    /// </summary>
    public class DailyReportSettings
    {
        /// <summary>
        /// 主要收件人清單
        /// </summary>
        public List<string> Recipients { get; set; } = new List<string>();

        /// <summary>
        /// 副本收件人清單
        /// </summary>
        public List<string> CcRecipients { get; set; } = new List<string>();

        /// <summary>
        /// 是否啟用每日報告
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Cron 表達式（格式: 分 時 日 月 星期）
        /// 預設: "0 22 * * *" (每天晚上 10:00)
        /// </summary>
        public string CronExpression { get; set; } = "0 22 * * *";

        /// <summary>
        /// 時區設定（預設: 台北標準時間）
        /// </summary>
        public string TimeZone { get; set; } = "Taipei Standard Time";
    }
}