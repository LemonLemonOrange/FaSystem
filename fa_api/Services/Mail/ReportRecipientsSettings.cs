namespace fa_api.Services.Mail
{
    /// <summary>
    /// 報告收件人設定
    /// </summary>
    public class ReportRecipientsSettings
    {
        /// <summary>
        /// 主要收件人
        /// </summary>
        public string MainRecipient { get; set; }

        /// <summary>
        /// 副本收件人
        /// </summary>
        public string CcRecipient { get; set; }
    }
}