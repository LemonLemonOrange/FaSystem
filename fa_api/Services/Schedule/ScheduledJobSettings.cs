using System.Collections.Generic;

namespace fa_api.Services.Schedule
{
    /// <summary>
    /// 排程任務設定
    /// </summary>
    public class ScheduledJobSettings
    {
        /// <summary>
        /// 全域時區設定
        /// </summary>
        public string TimeZone { get; set; } = "Taipei Standard Time";

        /// <summary>
        /// 預設收件人
        /// </summary>
        public List<string> DefaultRecipients { get; set; } = new List<string>();

        /// <summary>
        /// 預設副本收件人
        /// </summary>
        public List<string> DefaultCcRecipients { get; set; } = new List<string>();

        /// <summary>
        /// 所有排程任務清單
        /// </summary>
        public List<JobConfig> Jobs { get; set; } = new List<JobConfig>();
    }

    /// <summary>
    /// 單一排程任務配置
    /// </summary>
    public class JobConfig
    {
        /// <summary>
        /// 任務 ID（唯一識別碼）
        /// </summary>
        public string JobId { get; set; }

        /// <summary>
        /// 任務名稱
        /// </summary>
        public string JobName { get; set; }

        /// <summary>
        /// 任務類型
        /// </summary>
        public string JobType { get; set; }

        /// <summary>
        /// Cron 表達式
        /// </summary>
        public string CronExpression { get; set; }

        /// <summary>
        /// 是否啟用
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// 收件人清單
        /// </summary>
        public List<string> Recipients { get; set; } = new List<string>();

        /// <summary>
        /// 副本收件人清單
        /// </summary>
        public List<string> CcRecipients { get; set; } = new List<string>();

        /// <summary>
        /// 任務描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 額外參數（JSON 格式）
        /// </summary>
        public Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();
    }
}