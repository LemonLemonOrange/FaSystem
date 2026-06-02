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
    /// 早晨警報彙整任務 - 每天早上 8:00
    /// </summary>
    public class MorningAlertSummaryJob
    {
        private readonly ILogger<MorningAlertSummaryJob> _logger;
        private readonly IMailService _mailService;
        private readonly ReportRecipientsSettings _recipients;

        public MorningAlertSummaryJob(
            ILogger<MorningAlertSummaryJob> logger,
            IMailService mailService,
            IOptions<ReportRecipientsSettings> recipients)
        {
            _logger = logger;
            _mailService = mailService;
            _recipients = recipients.Value;
        }

        public async Task ExecuteAsync()
        {
            _logger.LogInformation("開始執行早晨警報彙整 - {Time}", DateTime.Now);

            try
            {
                var reportDate = DateTime.Now.ToString("yyyy年MM月dd日");
                var subject = $"災害警報彙整 - {reportDate}";
                var body = GenerateAlertSummaryHtml(reportDate);

                var mailRequest = new MailRequest
                {
                    To = _recipients.MainRecipient,
                    Subject = subject,
                    Body = body,
                    IsHtml = true,
                    Cc = new List<string> { _recipients.CcRecipient }
                };

                await _mailService.SendAsync(mailRequest);
                
                _logger.LogInformation("早晨警報彙整發送成功");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "早晨警報彙整執行失敗");
                throw;
            }
        }

        private string GenerateAlertSummaryHtml(string reportDate)
        {
            var html = new StringBuilder();
            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html>");
            html.AppendLine("<head><meta charset='utf-8'>");
            html.AppendLine("<style>");
            html.AppendLine("body { font-family: Arial, '微軟正黑體', sans-serif; line-height: 1.6; color: #333; }");
            html.AppendLine(".header { background-color: #FF9800; color: white; padding: 20px; text-align: center; }");
            html.AppendLine(".content { padding: 20px; }");
            html.AppendLine("table { width: 100%; border-collapse: collapse; margin-top: 10px; }");
            html.AppendLine("th, td { padding: 12px; text-align: left; border-bottom: 1px solid #ddd; }");
            html.AppendLine("th { background-color: #FF9800; color: white; }");
            html.AppendLine("</style></head><body>");
            
            html.AppendLine("<div class='header'>");
            html.AppendLine($"<h1>災害警報彙整</h1><p>{reportDate}</p>");
            html.AppendLine("</div>");
            
            html.AppendLine("<div class='content'>");
            html.AppendLine("<h2>⚠️ 前日警報統計</h2>");
            html.AppendLine("<table>");
            html.AppendLine("<thead><tr><th>警報類型</th><th>數量</th><th>狀態</th></tr></thead>");
            html.AppendLine("<tbody>");
            html.AppendLine("<tr><td>淹水警報</td><td>5</td><td>已解除</td></tr>");
            html.AppendLine("<tr><td>乾旱警報</td><td>2</td><td>持續中</td></tr>");
            html.AppendLine("<tr><td>水庫警報</td><td>0</td><td>正常</td></tr>");
            html.AppendLine("</tbody></table>");
            html.AppendLine("</div>");
            
            html.AppendLine("</body></html>");
            
            return html.ToString();
        }
    }
}