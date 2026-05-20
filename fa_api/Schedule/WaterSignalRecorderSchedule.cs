using fa_api.Services.WaterSignalRecorder;
using Hangfire;

namespace fa_api.Schedule
{
    public class WaterSignalRecorderSchedule
    {
        private const string JobId = "water-signal-recorder";
        private const string CronExpression = "*/10 * * * *";

        private readonly IWaterSignalRecorderService _service;

        public WaterSignalRecorderSchedule(IWaterSignalRecorderService service)
        {
            _service = service;
        }

        /// <summary>
        /// 向 Hangfire 註冊週期性任務（每 10 分鐘執行一次）
        /// </summary>
        public void RegisterRecurringJob()
        {
            RecurringJob.AddOrUpdate<IWaterSignalRecorderService>(
                JobId,
                svc => svc.RecordWaterSupplySignalsAsync(),
                CronExpression
            );
        }

        /// <summary>
        /// 移除週期性任務
        /// </summary>
        public void RemoveRecurringJob()
        {
            RecurringJob.RemoveIfExists(JobId);
        }

        /// <summary>
        /// 立即排入一次執行（Fire-and-forget）
        /// </summary>
        public void EnqueueNow()
        {
            BackgroundJob.Enqueue<IWaterSignalRecorderService>(
                svc => svc.RecordWaterSupplySignalsAsync()
            );
        }
    }
}
