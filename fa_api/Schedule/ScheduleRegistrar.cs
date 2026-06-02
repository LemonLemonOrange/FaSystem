using System;
using fa_api.Schedule.Jobs;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace fa_api.Schedule
{
    /// <summary>
    /// 排程任務註冊器 - 使用 RecurringJob.AddOrUpdate 精確定義執行時間
    /// </summary>
    public class ScheduleRegistrar
    {
        private readonly ILogger<ScheduleRegistrar> _logger;
        private static readonly TimeZoneInfo TaipeiTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Taipei Standard Time");

        public ScheduleRegistrar(ILogger<ScheduleRegistrar> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 註冊所有排程任務
        /// </summary>
        public void RegisterAllJobs()
        {
            try
            {
                // 1. 每日水情報告 - 每天晚上 10:00
                RecurringJob.AddOrUpdate<DailyWaterReportJob>(
                    "daily-water-report",
                    job => job.ExecuteAsync(),
                    "0 22 * * *",  // 每天 22:00
                    TaipeiTimeZone
                );
                _logger.LogInformation("✅ 已註冊排程: daily-water-report (每天 22:00)");

                // 2. 早晨警報彙整 - 每天早上 8:00
                RecurringJob.AddOrUpdate<MorningAlertSummaryJob>(
                    "morning-alert-summary",
                    job => job.ExecuteAsync(),
                    "0 8 * * *",   // 每天 08:00
                    TaipeiTimeZone
                );
                _logger.LogInformation("✅ 已註冊排程: morning-alert-summary (每天 08:00)");

                // 3. 週報統計 - 每週一早上 9:00
                RecurringJob.AddOrUpdate<WeeklyStatisticsJob>(
                    "weekly-statistics",
                    job => job.ExecuteAsync(),
                    "0 9 * * 1",   // 每週一 09:00
                    TaipeiTimeZone
                );
                _logger.LogInformation("✅ 已註冊排程: weekly-statistics (每週一 09:00)");

                // 4. 水庫檢查 - 每 6 小時
                RecurringJob.AddOrUpdate<ReservoirCheckJob>(
                    "reservoir-check",
                    job => job.ExecuteAsync(),
                    "0 */6 * * *", // 每 6 小時 (00:00, 06:00, 12:00, 18:00)
                    TaipeiTimeZone
                );
                _logger.LogInformation("✅ 已註冊排程: reservoir-check (每 6 小時)");

                // 5. 可以繼續新增更多任務...
                // RecurringJob.AddOrUpdate<YourNewJob>(
                //     "your-new-job-id",
                //     job => job.ExecuteAsync(),
                //     "0 12 * * *",  // 每天中午 12:00
                //     TaipeiTimeZone
                // );

                _logger.LogInformation("🎉 所有排程任務已註冊完成");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ 註冊排程任務時發生錯誤");
                throw;
            }
        }

        /// <summary>
        /// 移除所有排程任務
        /// </summary>
        public void RemoveAllJobs()
        {
            RecurringJob.RemoveIfExists("daily-water-report");
            RecurringJob.RemoveIfExists("morning-alert-summary");
            RecurringJob.RemoveIfExists("weekly-statistics");
            RecurringJob.RemoveIfExists("reservoir-check");
            
            _logger.LogInformation("已移除所有排程任務");
        }
    }
}