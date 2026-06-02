using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using fa_api.Services.Mail;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace fa_api.Schedule.Jobs
{
    /// <summary>
    /// 週報統計任務 - 每週一早上 9:00
    /// </summary>
    public class WeeklyStatisticsJob
    {
        private readonly ILogger<WeeklyStatisticsJob> _logger;
        private readonly IMailService _mailService;
        private readonly ReportRecipientsSettings _recipients;

        public WeeklyStatisticsJob(
            ILogger<WeeklyStatisticsJob> logger,
            IMailService mailService,
            IOptions<ReportRecipientsSettings> recipients)
        {
            _logger = logger;
            _mailService = mailService;
            _recipients = recipients.Value;
        }

        public async Task ExecuteAsync()
        {
            _logger.LogInformation("開始執行週報統計 - {Time}", DateTime.Now);

            try
            {
                var weekStart = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek + 1);
                var weekEnd = weekStart.AddDays(6);
                var subject = $"水利署週報 - {weekStart:MM/dd} ~ {weekEnd:MM/dd}";
                var body = GenerateWeeklyReportHtml(weekStart, weekEnd);

                var mailRequest = new MailRequest
                {
                    To = _recipients.MainRecipient,
                    Subject = subject,
                    Body = body,
                    IsHtml = true,
                    Cc = new List<string> { _recipients.CcRecipient }
                };

                await _mailService.SendAsync(mailRequest);
                
                _logger.LogInformation("週報統計發送成功");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "週報統計執行失敗");
                throw;
            }
        }

        private string GenerateWeeklyReportHtml(DateTime weekStart, DateTime weekEnd)
        {
            var html = new StringBuilder();
            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html>");
            html.AppendLine("<head><meta charset='utf-8'>");
            html.AppendLine("<style>");
            html.AppendLine("body { font-family: Arial, '微軟正黑體', sans-serif; line-height: 1.6; color: #333; }");
            html.AppendLine(".header { background-color: #4CAF50; color: white; padding: 20px; text-align: center; }");
            html.AppendLine(".content { padding: 20px; }");
            html.AppendLine(".info-item { padding: 10px; background-color: #f5f5f5; margin: 5px 0; border-left: 4px solid #4CAF50; }");
            html.AppendLine("</style></head><body>");
            
            html.AppendLine("<div class='header'>");
            html.AppendLine($"<h1>水利署週報統計</h1>");
            html.AppendLine($"<p>{weekStart:yyyy/MM/dd} ~ {weekEnd:yyyy/MM/dd}</p>");
            html.AppendLine("</div>");
            
            html.AppendLine("<div class='content'>");
            html.AppendLine("<h2>📊 本週統計</h2>");
            html.AppendLine("<div class='info-item'><strong>本週總降雨量：</strong> 125.5 mm</div>");
            html.AppendLine("<div class='info-item'><strong>發布警報數：</strong> 12 件</div>");
            html.AppendLine("<div class='info-item'><strong>已解除警報：</strong> 10 件</div>");
            html.AppendLine("</div>");
            
            html.AppendLine("</body></html>");
            
            return html.ToString();
        }
    }
}