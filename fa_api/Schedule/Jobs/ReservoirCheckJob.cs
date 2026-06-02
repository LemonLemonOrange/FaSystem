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
    /// 水庫水位檢查任務 - 每 6 小時
    /// </summary>
    public class ReservoirCheckJob
    {
        private readonly ILogger<ReservoirCheckJob> _logger;
        private readonly IMailService _mailService;
        private readonly ReportRecipientsSettings _recipients;

        public ReservoirCheckJob(
            ILogger<ReservoirCheckJob> logger,
            IMailService mailService,
            IOptions<ReportRecipientsSettings> recipients)
        {
            _logger = logger;
            _mailService = mailService;
            _recipients = recipients.Value;
        }

        public async Task ExecuteAsync()
        {
            _logger.LogInformation("開始執行水庫水位檢查 - {Time}", DateTime.Now);

            try
            {
                var checkTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm");
                var subject = $"水庫水位檢查報告 - {checkTime}";
                var body = GenerateReservoirCheckHtml(checkTime);

                var mailRequest = new MailRequest
                {
                    To = _recipients.MainRecipient,
                    Subject = subject,
                    Body = body,
                    IsHtml = true,
                    Cc = new List<string>() // 這個不需要 CC
                };

                await _mailService.SendAsync(mailRequest);
                
                _logger.LogInformation("水庫水位檢查報告發送成功");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "水庫水位檢查執行失敗");
                throw;
            }
        }

        private string GenerateReservoirCheckHtml(string checkTime)
        {
            var html = new StringBuilder();
            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html>");
            html.AppendLine("<head><meta charset='utf-8'>");
            html.AppendLine("<style>");
            html.AppendLine("body { font-family: Arial, '微軟正黑體', sans-serif; line-height: 1.6; color: #333; }");
            html.AppendLine(".header { background-color: #00BCD4; color: white; padding: 20px; text-align: center; }");
            html.AppendLine(".content { padding: 20px; }");
            html.AppendLine(".info-item { padding: 10px; background-color: #f5f5f5; margin: 5px 0; border-left: 4px solid #00BCD4; }");
            html.AppendLine("</style></head><body>");
            
            html.AppendLine("<div class='header'>");
            html.AppendLine($"<h1>水庫水位檢查</h1><p>{checkTime}</p>");
            html.AppendLine("</div>");
            
            html.AppendLine("<div class='content'>");
            html.AppendLine("<div class='info-item'><strong>水位正常：</strong> 45 座水庫</div>");
            html.AppendLine("<div class='info-item'><strong>水位偏低：</strong> 3 座水庫</div>");
            html.AppendLine("<div class='info-item'><strong>需注意水庫：</strong> 石門水庫、曾文水庫</div>");
            html.AppendLine("</div>");
            
            html.AppendLine("</body></html>");
            
            return html.ToString();
        }
    }
}