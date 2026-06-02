using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fa_api.Services.DailyReport;
using fa_api.Services.Mail;
using Hangfire;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace fa_api.Schedule
{
    /// <summary>
    /// 每日報告排程任務
    /// </summary>
    public class DailyReportSchedule
    {
        private const string JobId = "daily-report";

        private readonly ILogger<DailyReportSchedule> _logger;
        private readonly IMailService _mailService;
        private readonly DailyReportSettings _settings;

        public DailyReportSchedule(
            ILogger<DailyReportSchedule> logger,
            IMailService mailService,
            IOptions<DailyReportSettings> settings)
        {
            _logger = logger;
            _mailService = mailService;
            _settings = settings.Value;
        }

        /// <summary>
        /// 向 Hangfire 註冊週期性任務
        /// </summary>
        public void RegisterRecurringJob()
        {
            if (!_settings.Enabled)
            {
                _logger.LogWarning("每日報告排程已停用（DailyReportSettings.Enabled = false）");
                return;
            }

            // 驗證 Cron 表達式
            if (string.IsNullOrWhiteSpace(_settings.CronExpression))
            {
                _logger.LogError("Cron 表達式不得為空，請在 appsettings.json 中設定 DailyReportSettings.CronExpression");
                return;
            }

            try
            {
                // 取得時區資訊
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById(_settings.TimeZone);

                RecurringJob.AddOrUpdate(
                    JobId,
                    () => ExecuteDailyReportAsync(),
                    _settings.CronExpression,
                    timeZone
                );

                _logger.LogInformation(
                    "已註冊每日報告排程任務 - Cron: {Cron}, 時區: {TimeZone}, 收件人數: {Count}",
                    _settings.CronExpression,
                    _settings.TimeZone,
                    _settings.Recipients?.Count ?? 0
                );
            }
            catch (TimeZoneNotFoundException ex)
            {
                _logger.LogError(ex, "時區設定錯誤: {TimeZone}", _settings.TimeZone);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "註冊排程任務失敗");
            }
        }

        /// <summary>
        /// 移除週期性任務
        /// </summary>
        public void RemoveRecurringJob()
        {
            RecurringJob.RemoveIfExists(JobId);
            _logger.LogInformation("已移除每日報告排程任務");
        }

        /// <summary>
        /// 立即執行一次任務（測試用）
        /// </summary>
        public void EnqueueNow()
        {
            BackgroundJob.Enqueue(() => ExecuteDailyReportAsync());
            _logger.LogInformation("已將每日報告任務排入佇列立即執行");
        }

        /// <summary>
        /// 每日報告任務的實際執行邏輯
        /// </summary>
        public async Task ExecuteDailyReportAsync()
        {
            _logger.LogInformation("開始執行每日報告任務 - {Time}", DateTime.Now);

            try
            {
                // 檢查是否有收件人
                if (_settings.Recipients == null || !_settings.Recipients.Any())
                {
                    _logger.LogWarning("沒有設定收件人，跳過發送郵件");
                    return;
                }

                // 1. 產生報告內容
                var reportContent = await GenerateReportContentAsync();

                // 2. 發送郵件給每個收件人
                foreach (var recipient in _settings.Recipients)
                {
                    var mailRequest = new MailRequest
                    {
                        To = recipient,
                        Subject = $"水利署每日報告 - {DateTime.Now:yyyy/MM/dd}",
                        Body = reportContent,
                        IsHtml = true,
                        Cc = _settings.CcRecipients ?? new System.Collections.Generic.List<string>()
                    };

                    try
                    {
                        await _mailService.SendAsync(mailRequest);
                        _logger.LogInformation("已發送每日報告郵件至: {Recipient}", recipient);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "發送每日報告郵件失敗 - 收件人: {Recipient}", recipient);
                    }
                }

                _logger.LogInformation("每日報告任務執行成功 - 已發送 {Count} 封郵件", _settings.Recipients.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "每日報告任務執行失敗");
                throw; // 重新拋出例外讓 Hangfire 記錄失敗
            }
        }

        /// <summary>
        /// 產生每日報告內容（HTML 格式）
        /// </summary>
        private async Task<string> GenerateReportContentAsync()
        {
            var now = DateTime.Now;
            var reportDate = now.ToString("yyyy年MM月dd日");

            var html = new StringBuilder();
            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html>");
            html.AppendLine("<head>");
            html.AppendLine("    <meta charset='utf-8'>");
            html.AppendLine("    <style>");
            html.AppendLine("        body { font-family: Arial, '微軟正黑體', sans-serif; line-height: 1.6; color: #333; }");
            html.AppendLine("        .header { background-color: #2196F3; color: white; padding: 20px; text-align: center; }");
            html.AppendLine("        .content { padding: 20px; }");
            html.AppendLine("        .section { margin-bottom: 30px; }");
            html.AppendLine("        .section-title { color: #2196F3; border-bottom: 2px solid #2196F3; padding-bottom: 10px; margin-bottom: 15px; }");
            html.AppendLine("        .info-item { padding: 10px; background-color: #f5f5f5; margin: 5px 0; border-left: 4px solid #2196F3; }");
            html.AppendLine("        .footer { text-align: center; color: #666; font-size: 12px; margin-top: 30px; padding-top: 20px; border-top: 1px solid #ddd; }");
            html.AppendLine("        table { width: 100%; border-collapse: collapse; margin-top: 10px; }");
            html.AppendLine("        th, td { padding: 12px; text-align: left; border-bottom: 1px solid #ddd; }");
            html.AppendLine("        th { background-color: #2196F3; color: white; }");
            html.AppendLine("        tr:hover { background-color: #f5f5f5; }");
            html.AppendLine("    </style>");
            html.AppendLine("</head>");
            html.AppendLine("<body>");

            // Header
            html.AppendLine("    <div class='header'>");
            html.AppendLine($"        <h1>水利署災害預警系統 - 每日報告</h1>");
            html.AppendLine($"        <p>{reportDate}</p>");
            html.AppendLine("    </div>");

            html.AppendLine("    <div class='content'>");

            // Section 1: 水情燈號統計
            html.AppendLine("        <div class='section'>");
            html.AppendLine("            <h2 class='section-title'>📊 水情燈號統計</h2>");
            html.AppendLine("            <div class='info-item'>");
            html.AppendLine("                <strong>綠燈地區：</strong> 15 個縣市");
            html.AppendLine("            </div>");
            html.AppendLine("            <div class='info-item'>");
            html.AppendLine("                <strong>黃燈地區：</strong> 3 個縣市");
            html.AppendLine("            </div>");
            html.AppendLine("            <div class='info-item'>");
            html.AppendLine("                <strong>橙燈地區：</strong> 0 個縣市");
            html.AppendLine("            </div>");
            html.AppendLine("            <div class='info-item'>");
            html.AppendLine("                <strong>紅燈地區：</strong> 0 個縣市");
            html.AppendLine("            </div>");
            html.AppendLine("        </div>");

            // Section 2: 今日警報
            html.AppendLine("        <div class='section'>");
            html.AppendLine("            <h2 class='section-title'>⚠️ 今日警報統計</h2>");
            html.AppendLine("            <table>");
            html.AppendLine("                <thead>");
            html.AppendLine("                    <tr>");
            html.AppendLine("                        <th>警報類型</th>");
            html.AppendLine("                        <th>數量</th>");
            html.AppendLine("                        <th>狀態</th>");
            html.AppendLine("                    </tr>");
            html.AppendLine("                </thead>");
            html.AppendLine("                <tbody>");
            html.AppendLine("                    <tr>");
            html.AppendLine("                        <td>淹水警報</td>");
            html.AppendLine("                        <td>5</td>");
            html.AppendLine("                        <td>已解除</td>");
            html.AppendLine("                    </tr>");
            html.AppendLine("                    <tr>");
            html.AppendLine("                        <td>乾旱警報</td>");
            html.AppendLine("                        <td>2</td>");
            html.AppendLine("                        <td>持續中</td>");
            html.AppendLine("                    </tr>");
            html.AppendLine("                    <tr>");
            html.AppendLine("                        <td>水庫警報</td>");
            html.AppendLine("                        <td>0</td>");
            html.AppendLine("                        <td>正常</td>");
            html.AppendLine("                    </tr>");
            html.AppendLine("                </tbody>");
            html.AppendLine("            </table>");
            html.AppendLine("        </div>");

            // Section 3: 系統資訊
            html.AppendLine("        <div class='section'>");
            html.AppendLine("            <h2 class='section-title'>💻 系統資訊</h2>");
            html.AppendLine("            <div class='info-item'>");
            html.AppendLine($"                <strong>報告產生時間：</strong> {now:yyyy/MM/dd HH:mm:ss}");
            html.AppendLine("            </div>");
            html.AppendLine("            <div class='info-item'>");
            html.AppendLine("                <strong>系統狀態：</strong> 正常運行");
            html.AppendLine("            </div>");
            html.AppendLine("        </div>");

            html.AppendLine("    </div>");

            // Footer
            html.AppendLine("    <div class='footer'>");
            html.AppendLine("        <p>此郵件由水利署災害預警系統自動產生</p>");
            html.AppendLine("        <p>如有問題請聯繫系統管理員</p>");
            html.AppendLine("    </div>");

            html.AppendLine("</body>");
            html.AppendLine("</html>");

            // 模擬非同步操作
            await Task.Delay(100);

            return html.ToString();
        }
    }
}