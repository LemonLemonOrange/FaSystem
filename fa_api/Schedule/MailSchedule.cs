using System;
using fa_api.Services.Mail;
using Hangfire;

namespace fa_api.Schedule
{
    public class MailSchedule
    {
        private readonly IMailService _mailService;

        public MailSchedule(IMailService mailService)
        {
            _mailService = mailService;
        }

        /// <summary>
        /// 立即排入背景寄信（Fire-and-forget）
        /// </summary>
        public void EnqueueSend(MailRequest request)
        {
            BackgroundJob.Enqueue<IMailService>(svc => svc.SendAsync(request));
        }

        /// <summary>
        /// 延遲指定時間後寄信
        /// </summary>
        public void ScheduleSend(MailRequest request, TimeSpan delay)
        {
            BackgroundJob.Schedule<IMailService>(
                svc => svc.SendAsync(request),
                delay
            );
        }

        /// <summary>
        /// 固定 Cron 週期寄信（例如每天早上 8 點）
        /// </summary>
        public void AddRecurringMail(string jobId, MailRequest request, string cronExpression)
        {
            RecurringJob.AddOrUpdate<IMailService>(
                jobId,
                svc => svc.SendAsync(request),
                cronExpression
            );
        }

        /// <summary>
        /// 移除週期性排程
        /// </summary>
        public void RemoveRecurringMail(string jobId)
        {
            RecurringJob.RemoveIfExists(jobId);
        }
    }
}
