using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using fa_api.Data;
using fa_api.Models;
using fa_api.Services.WraGov;
using Microsoft.Extensions.Logging;

namespace fa_api.Services.WaterSignalRecorder
{
    public class WaterSignalRecorderService : IWaterSignalRecorderService
    {
        private readonly IWraGovService _wraGovService;
        private readonly FaDbContext _dbContext;
        private readonly ILogger<WaterSignalRecorderService> _logger;

        public WaterSignalRecorderService(
            IWraGovService wraGovService,
            FaDbContext dbContext,
            ILogger<WaterSignalRecorderService> logger)
        {
            _wraGovService = wraGovService;
            _dbContext = dbContext;
            _logger = logger;
        }

        /// <summary>
        /// 將 SupplyStatus 對應至燈號字串；無效值回傳 null
        /// </summary>
        internal static string MapToSignalLevel(int supplyStatus)
        {
            return supplyStatus switch
            {
                1 => "綠燈",
                2 => "黃燈",
                3 => "橙燈",
                4 => "紅燈",
                _ => null
            };
        }

        /// <summary>
        /// 抓取供水狀況、轉換燈號並批次寫入 FA_WR_WaterSignalSnapshot
        /// </summary>
        public async Task RecordWaterSupplySignalsAsync()
        {
            var batchId = Guid.NewGuid().ToString();
            var stopwatch = Stopwatch.StartNew();

            // 1. 呼叫 WRA API 取得供水狀況
            List<Dtos.WraGov.WaterSupplyConditionDto> conditions;
            try
            {
                conditions = await _wraGovService.GetWaterSupplyConditionAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[WaterSignalRecorder] 呼叫 GetWaterSupplyConditionAsync() 失敗");
                throw;
            }

            // 2. 逐筆轉換燈號，略過無效值
            var snapshots = new List<FaWrSignalSnapshot>();
            foreach (var item in conditions)
            {
                var signalLevel = MapToSignalLevel(item.SupplyStatus);
                if (signalLevel == null)
                {
                    _logger.LogWarning(
                        "[WaterSignalRecorder] 略過無效 SupplyStatus={SupplyStatus}，AreaName={AreaName}",
                        item.SupplyStatus, item.AreaName);
                    continue;
                }

                snapshots.Add(new FaWrSignalSnapshot
                {
                    BatchId = batchId,
                    AreaName = item.AreaName,
                    SignalLevel = signalLevel,
                    SupplyStatus = item.SupplyStatus,
                    StatusDescription = item.StatusDescription
                });
            }

            // 3. 批次寫入資料庫
            try
            {
                _dbContext.FaWrSignalSnapshot.AddRange(snapshots);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "[WaterSignalRecorder] SaveChangesAsync() 失敗，BatchId={BatchId}，預計寫入筆數={Count}",
                    batchId, snapshots.Count);
                throw;
            }

            stopwatch.Stop();
            _logger.LogInformation(
                "[WaterSignalRecorder] BatchId={BatchId}, 寫入筆數={Count}, 執行時間={ElapsedMs}ms",
                batchId, snapshots.Count, stopwatch.ElapsedMilliseconds);
        }
    }
}
