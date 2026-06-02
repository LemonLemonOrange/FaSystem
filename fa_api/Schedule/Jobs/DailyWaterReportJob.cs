using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using fa_api.Services.Mail;
using Hangfire;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace fa_api.Schedule.Jobs
{
    /// <summary>
    /// 每日水情報告任務 - 每天晚上 10:00
    /// </summary>
    public class DailyWaterReportJob
    {
        private readonly ILogger<DailyWaterReportJob> _logger;
        private readonly IMailService _mailService;
        private readonly ReportRecipientsSettings _recipients;

        public DailyWaterReportJob(
            ILogger<DailyWaterReportJob> logger,
            IMailService mailService,
            IOptions<ReportRecipientsSettings> recipients)
        {
            _logger = logger;
            _mailService = mailService;
            _recipients = recipients.Value;
        }

        /// <summary>
        /// 執行每日水情報告任務
        /// </summary>
        public async Task ExecuteAsync()
        {
            _logger.LogInformation("開始執行每日水情報告 - {Time}", DateTime.Now);

            try
            {
                var reportDate = DateTime.Now.ToString("yyyy年MM月dd日");
                var subject = $"水利署每日水情報告 - {reportDate}";
                var body = GenerateReportHtml(reportDate);

                var mailRequest = new MailRequest
                {
                    To = _recipients.MainRecipient,
                    Subject = subject,
                    Body = body,
                    IsHtml = true,
                    Cc = new List<string> { _recipients.CcRecipient }
                };

                await _mailService.SendAsync(mailRequest);
                
                _logger.LogInformation("每日水情報告發送成功 - 收件人: {To}, CC: {Cc}", 
                    _recipients.MainRecipient, _recipients.CcRecipient);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "每日水情報告執行失敗");
                throw;
            }
        }

        private string GenerateReportHtml(string reportDate)
        {
            var html = new StringBuilder();
            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html>");
            html.AppendLine("<head><meta charset='utf-8'>");
            html.AppendLine("<style>");
            html.AppendLine("body { font-family: Arial, '微軟正黑體', sans-serif; line-height: 1.6; color: #333; }");
            html.AppendLine(".header { background-color: #2196F3; color: white; padding: 20px; text-align: center; }");
            html.AppendLine(".content { padding: 20px; }");
            html.AppendLine(".info-item { padding: 10px; background-color: #f5f5f5; margin: 5px 0; border-left: 4px solid #2196F3; }");
            html.AppendLine(".footer { text-align: center; color: #666; font-size: 12px; margin-top: 30px; padding-top: 20px; border-top: 1px solid #ddd; }");
            html.AppendLine("</style></head><body>");
            
            html.AppendLine("<div class='header'>");
            html.AppendLine($"<h1>水利署每日水情報告</h1><p>{reportDate}</p>");
            html.AppendLine("</div>");
            
            html.AppendLine("<div class='content'>");
            html.AppendLine("<h2>?? 水情燈號統計</h2>");
            html.AppendLine("<div class='info-item'><strong>綠燈地區：</strong> 15 個縣市</div>");
            html.AppendLine("<div class='info-item'><strong>黃燈地區：</strong> 3 個縣市</div>");
            html.AppendLine("<div class='info-item'><strong>橙燈地區：</strong> 0 個縣市</div>");
            html.AppendLine("<div class='info-item'><strong>紅燈地區：</strong> 0 個縣市</div>");
            html.AppendLine("</div>");
            
            html.AppendLine("<div class='footer'>");
            html.AppendLine($"<p>此郵件由水利署災害預警系統自動產生 - {DateTime.Now:yyyy/MM/dd HH:mm:ss}</p>");
            html.AppendLine("</div>");
            
            html.AppendLine("</body></html>");
            
            return html.ToString();
        }
    }
}