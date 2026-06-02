using System;
using fa_api.Schedule;
using fa_api.Schedule.Jobs;
using Hangfire;
using Microsoft.AspNetCore.Mvc;

namespace fa_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "Schedule")]
    public class ScheduleController : ControllerBase
    {
        private readonly DailyWaterReportJob _dailyWaterReport;
        private readonly MorningAlertSummaryJob _morningAlert;
        private readonly WeeklyStatisticsJob _weeklyStats;
        private readonly ReservoirCheckJob _reservoirCheck;
        private readonly ScheduleRegistrar _registrar;

        public ScheduleController(
            DailyWaterReportJob dailyWaterReport,
            MorningAlertSummaryJob morningAlert,
            WeeklyStatisticsJob weeklyStats,
            ReservoirCheckJob reservoirCheck,
            ScheduleRegistrar registrar)
        {
            _dailyWaterReport = dailyWaterReport;
            _morningAlert = morningAlert;
            _weeklyStats = weeklyStats;
            _reservoirCheck = reservoirCheck;
            _registrar = registrar;
        }

        /// <summary>
        /// 取得所有排程任務資訊
        /// </summary>
        [HttpGet("jobs")]
        public IActionResult GetAllJobs()
        {
            return Ok(new
            {
                timezone = "Taipei Standard Time",
                jobs = new[]
                {
                    new 
                    { 
                        jobId = "daily-water-report", 
                        name = "每日水情報告", 
                        cron = "0 22 * * *", 
                        description = "每天晚上 10:00",
                        recipients = "boy.sun1116@gmail.com",
                        cc = "music.sun1116@gmail.com"
                    },
                    new 
                    { 
                        jobId = "morning-alert-summary", 
                        name = "早晨警報彙整", 
                        cron = "0 8 * * *", 
                        description = "每天早上 8:00",
                        recipients = "boy.sun1116@gmail.com",
                        cc = "music.sun1116@gmail.com"
                    },
                    new 
                    { 
                        jobId = "weekly-statistics", 
                        name = "週報統計", 
                        cron = "0 9 * * 1", 
                        description = "每週一早上 9:00",
                        recipients = "boy.sun1116@gmail.com",
                        cc = "music.sun1116@gmail.com"
                    },
                    new 
                    { 
                        jobId = "reservoir-check", 
                        name = "水庫檢查", 
                        cron = "0 */6 * * *", 
                        description = "每 6 小時 (00:00, 06:00, 12:00, 18:00)",
                        recipients = "boy.sun1116@gmail.com",
                        cc = ""
                    }
                }
            });
        }

        /// <summary>
        /// 觸發每日水情報告（測試用）
        /// </summary>
        [HttpPost("jobs/daily-water-report/trigger")]
        public IActionResult TriggerDailyWaterReport()
        {
            BackgroundJob.Enqueue<DailyWaterReportJob>(job => job.ExecuteAsync());
            return Ok(new 
            { 
                message = "每日水情報告已排入佇列", 
                jobId = "daily-water-report",
                scheduledTime = "每天 22:00"
            });
        }

        /// <summary>
        /// 觸發早晨警報彙整（測試用）
        /// </summary>
        [HttpPost("jobs/morning-alert-summary/trigger")]
        public IActionResult TriggerMorningAlert()
        {
            BackgroundJob.Enqueue<MorningAlertSummaryJob>(job => job.ExecuteAsync());
            return Ok(new 
            { 
                message = "早晨警報彙整已排入佇列", 
                jobId = "morning-alert-summary",
                scheduledTime = "每天 08:00"
            });
        }

        /// <summary>
        /// 觸發週報統計（測試用）
        /// </summary>
        [HttpPost("jobs/weekly-statistics/trigger")]
        public IActionResult TriggerWeeklyStats()
        {
            BackgroundJob.Enqueue<WeeklyStatisticsJob>(job => job.ExecuteAsync());
            return Ok(new 
            { 
                message = "週報統計已排入佇列", 
                jobId = "weekly-statistics",
                scheduledTime = "每週一 09:00"
            });
        }

        /// <summary>
        /// 觸發水庫檢查（測試用）
        /// </summary>
        [HttpPost("jobs/reservoir-check/trigger")]
        public IActionResult TriggerReservoirCheck()
        {
            BackgroundJob.Enqueue<ReservoirCheckJob>(job => job.ExecuteAsync());
            return Ok(new 
            { 
                message = "水庫檢查已排入佇列", 
                jobId = "reservoir-check",
                scheduledTime = "每 6 小時"
            });
        }

        /// <summary>
        /// 重新載入所有排程任務
        /// </summary>
        [HttpPost("reload")]
        public IActionResult ReloadAllJobs()
        {
            _registrar.RemoveAllJobs();
            _registrar.RegisterAllJobs();
            return Ok(new { message = "已重新載入所有排程任務" });
        }
    }
}