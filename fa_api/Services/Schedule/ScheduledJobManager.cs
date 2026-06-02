using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fa_api.Services.Mail;
using fa_api.Services.Ncdr;
using fa_api.Services.WraGov;
using Hangfire;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace fa_api.Services.Schedule
{
    /// <summary>
    /// 排程任務管理器
    /// </summary>
    public class ScheduledJobManager
    {
        private readonly ILogger<ScheduledJobManager> _logger;
        private readonly ScheduledJobSettings _settings;
        private readonly IMailService _mailService;
        private readonly INcdrDroughtService _ncdrService;
        private readonly IWraGovService _wraService;

        public ScheduledJobManager(
            ILogger<ScheduledJobManager> logger,
            IOptions<ScheduledJobSettings> settings,
            IMailService mailService,
            INcdrDroughtService ncdrService,
            IWraGovService wraService)
        {
            _logger = logger;
            _settings = settings.Value;
            _mailService = mailService;
            _ncdrService = ncdrService;
            _wraService = wraService;
        }

        /// <summary>
        /// 註冊所有已啟用的排程任務
        /// </summary>
        public void RegisterAllJobs()
        {
            if (_settings.Jobs == null || !_settings.Jobs.Any())
            {
                _logger.LogWarning("沒有設定任何排程任務");
                return;
            }

            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(_settings.TimeZone);

            foreach (var jobConfig in _settings.Jobs.Where(j => j.Enabled))
            {
                try
                {
                    RegisterJob(jobConfig, timeZone);
                    _logger.LogInformation(
                        "已註冊排程任務: {JobName} ({JobId}) - Cron: {Cron}",
                        jobConfig.JobName,
                        jobConfig.JobId,
                        jobConfig.CronExpression
                    );
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "註冊排程任務失敗: {JobId}", jobConfig.JobId);
                }
            }
        }

        /// <summary>
        /// 註冊單一排程任務
        /// </summary>
        private void RegisterJob(JobConfig config, TimeZoneInfo timeZone)
        {
            RecurringJob.AddOrUpdate(
                config.JobId,
                () => ExecuteJobAsync(config),
                config.CronExpression,
                timeZone
            );
        }

        /// <summary>
        /// 移除所有排程任務
        /// </summary>
        public void RemoveAllJobs()
        {
            foreach (var jobConfig in _settings.Jobs)
            {
                RecurringJob.RemoveIfExists(jobConfig.JobId);
                _logger.LogInformation("已移除排程任務: {JobId}", jobConfig.JobId);
            }
        }

        /// <summary>
        /// 移除單一排程任務
        /// </summary>
        public void RemoveJob(string jobId)
        {
            RecurringJob.RemoveIfExists(jobId);
            _logger.LogInformation("已移除排程任務: {JobId}", jobId);
        }

        /// <summary>
        /// 立即執行指定任務
        /// </summary>
        public void TriggerJob(string jobId)
        {
            var config = _settings.Jobs.FirstOrDefault(j => j.JobId == jobId);
            if (config == null)
            {
                throw new ArgumentException($"找不到任務: {jobId}");
            }

            BackgroundJob.Enqueue(() => ExecuteJobAsync(config));
            _logger.LogInformation("已將任務排入佇列: {JobId}", jobId);
        }

        /// <summary>
        /// 執行排程任務
        /// </summary>
        public async Task ExecuteJobAsync(JobConfig config)
        {
            _logger.LogInformation("開始執行任務: {JobName} ({JobId}) - {Time}",
                config.JobName, config.JobId, DateTime.Now);

            try
            {
                string subject = "";
                string body = "";

                // 根據任務類型執行不同的邏輯
                switch (config.JobType)
                {
                    case "WaterReport":
                        (subject, body) = await GenerateWaterReportAsync();
                        break;

                    case "AlertSummary":
                        (subject, body) = await GenerateAlertSummaryAsync();
                        break;

                    case "WeeklyStatistics":
                        (subject, body) = await GenerateWeeklyStatisticsAsync();
                        break;

                    case "ReservoirCheck":
                        (subject, body) = await CheckReservoirStatusAsync();
                        break;

                    case "DroughtCheck":
                        (subject, body) = await CheckDroughtAlertsAsync();
                        break;

                    default:
                        _logger.LogWarning("未知的任務類型: {JobType}", config.JobType);
                        return;
                }

                // 發送郵件給所有收件人
                await SendEmailToRecipientsAsync(config, subject, body);

                _logger.LogInformation("任務執行成功: {JobId}", config.JobId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "任務執行失敗: {JobId}", config.JobId);
                throw;
            }
        }

        /// <summary>
        /// 發送郵件給收件人
        /// </summary>
        private async Task SendEmailToRecipientsAsync(JobConfig config, string subject, string body)
        {
            var recipients = config.Recipients?.Any() == true
                ? config.Recipients
                : _settings.DefaultRecipients;

            var ccRecipients = config.CcRecipients?.Any() == true
                ? config.CcRecipients
                : _settings.DefaultCcRecipients;

            if (!recipients.Any())
            {
                _logger.LogWarning("任務 {JobId} 沒有收件人", config.JobId);
                return;
            }

            foreach (var recipient in recipients)
            {
                var mailRequest = new MailRequest
                {
                    To = recipient,
                    Subject = subject,
                    Body = body,
                    IsHtml = true,
                    Cc = ccRecipients ?? new List<string>()
                };

                try
                {
                    await _mailService.SendAsync(mailRequest);
                    _logger.LogInformation("已發送郵件至: {Recipient} (任務: {JobId})",
                        recipient, config.JobId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "發送郵件失敗 - 收件人: {Recipient}, 任務: {JobId}",
                        recipient, config.JobId);
                }
            }
        }

        #region 各種報告產生方法

        /// <summary>
        /// 產生每日水情報告
        /// </summary>
        private async Task<(string subject, string body)> GenerateWaterReportAsync()
        {
            var date = DateTime.Now.ToString("yyyy/MM/dd");
            var subject = $"水利署每日水情報告 - {date}";

            // TODO: 從資料庫查詢實際數據
            var body = GenerateHtmlReport(
                "每日水情報告",
                new Dictionary<string, object>
                {
                    { "報告日期", date },
                    { "綠燈地區", "15 個縣市" },
                    { "黃燈地區", "3 個縣市" },
                    { "橙燈地區", "0 個縣市" },
                    { "紅燈地區", "0 個縣市" }
                }
            );

            await Task.CompletedTask;
            return (subject, body);
        }

        /// <summary>
        /// 產生警報彙整報告
        /// </summary>
        private async Task<(string subject, string body)> GenerateAlertSummaryAsync()
        {
            var date = DateTime.Now.ToString("yyyy/MM/dd");
            var subject = $"災害警報彙整 - {date}";

            var body = GenerateHtmlReport(
                "災害警報彙整",
                new Dictionary<string, object>
                {
                    { "統計日期", date },
                    { "淹水警報", "5 件 (已解除)" },
                    { "乾旱警報", "2 件 (持續中)" },
                    { "水庫警報", "0 件" }
                }
            );

            await Task.CompletedTask;
            return (subject, body);
        }

        /// <summary>
        /// 產生週報統計
        /// </summary>
        private async Task<(string subject, string body)> GenerateWeeklyStatisticsAsync()
        {
            var weekStart = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek + 1);
            var weekEnd = weekStart.AddDays(6);
            var subject = $"水利署週報 - {weekStart:MM/dd} ~ {weekEnd:MM/dd}";

            var body = GenerateHtmlReport(
                "水利署週報統計",
                new Dictionary<string, object>
                {
                    { "統計週期", $"{weekStart:yyyy/MM/dd} ~ {weekEnd:yyyy/MM/dd}" },
                    { "本週總降雨量", "125.5 mm" },
                    { "發布警報數", "12 件" },
                    { "已解除警報", "10 件" }
                }
            );

            await Task.CompletedTask;
            return (subject, body);
        }

        /// <summary>
        /// 檢查水庫狀態
        /// </summary>
        private async Task<(string subject, string body)> CheckReservoirStatusAsync()
        {
            var date = DateTime.Now.ToString("yyyy/MM/dd HH:mm");
            var subject = $"水庫水位檢查報告 - {date}";

            // TODO: 呼叫 WraGovService 取得實際水庫資料
            var body = GenerateHtmlReport(
                "水庫水位檢查",
                new Dictionary<string, object>
                {
                    { "檢查時間", date },
                    { "水位正常", "45 座水庫" },
                    { "水位偏低", "3 座水庫" },
                    { "需注意水庫", "石門水庫、曾文水庫" }
                }
            );

            await Task.CompletedTask;
            return (subject, body);
        }

        /// <summary>
        /// 檢查乾旱警報
        /// </summary>
        private async Task<(string subject, string body)> CheckDroughtAlertsAsync()
        {
            var date = DateTime.Now.ToString("yyyy/MM/dd HH:mm");
            var subject = $"乾旱警報檢查 - {date}";

            // TODO: 呼叫 NcdrDroughtService 取得實際乾旱資料
            var body = GenerateHtmlReport(
                "乾旱警報檢查",
                new Dictionary<string, object>
                {
                    { "檢查時間", date },
                    { "藍燈地區", "0 個" },
                    { "黃燈地區", "2 個" },
                    { "橙燈地區", "0 個" },
                    { "紅燈地區", "0 個" }
                }
            );

            await Task.CompletedTask;
            return (subject, body);
        }

        /// <summary>
        /// 產生 HTML 格式報告
        /// </summary>
        private string GenerateHtmlReport(string title, Dictionary<string, object> data)
        {
            var html = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <style>
        body {{ font-family: Arial, '微軟正黑體', sans-serif; line-height: 1.6; color: #333; }}
        .header {{ background-color: #2196F3; color: white; padding: 20px; text-align: center; }}
        .content {{ padding: 20px; }}
        .info-item {{ padding: 10px; background-color: #f5f5f5; margin: 5px 0; border-left: 4px solid #2196F3; }}
        .footer {{ text-align: center; color: #666; font-size: 12px; margin-top: 30px; padding-top: 20px; border-top: 1px solid #ddd; }}
    </style>
</head>
<body>
    <div class='header'>
        <h1>{title}</h1>
    </div>
    <div class='content'>";

            foreach (var item in data)
            {
                html += $@"
        <div class='info-item'>
            <strong>{item.Key}：</strong> {item.Value}
        </div>";
            }

            html += $@"
    </div>
    <div class='footer'>
        <p>此郵件由水利署災害預警系統自動產生 - {DateTime.Now:yyyy/MM/dd HH:mm:ss}</p>
        <p>如有問題請聯繫系統管理員</p>
    </div>
</body>
</html>";

            return html;
        }

        #endregion

        /// <summary>
        /// 取得所有任務清單
        /// </summary>
        public List<JobConfig> GetAllJobs()
        {
            return _settings.Jobs;
        }

        /// <summary>
        /// 取得單一任務設定
        /// </summary>
        public JobConfig GetJob(string jobId)
        {
            return _settings.Jobs.FirstOrDefault(j => j.JobId == jobId);
        }
    }
}