using fa_api.Data;
using fa_api.Dtos.WraGov;
using fa_api.Services.WraGov;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fa_api.Services.WrRecorder
{
    /// <summary>
    /// 水庫服務實作
    /// </summary>
    public class WrRecorderService : IWrRecorderService
    {
        private readonly FaDbContext _context;
        private readonly IWraGovService _wraGovService;
        private readonly ILogger<WrRecorderService> _logger;

        public WrRecorderService(
            FaDbContext context,
            IWraGovService wraGovService,
            ILogger<WrRecorderService> logger)
        {
            _context = context;
            _wraGovService = wraGovService;
            _logger = logger;
        }

        public async Task<List<ReservoirDisplayDto>> GetDisplayReservoirsAsync()
        {
            try
            {
                _logger.LogInformation("開始取得前端顯示用水庫清單");

                // 1. 從資料庫直接取得所有資料（包含經緯度）
                var result = await _context.FaWrReservoirAlert
                    .Where(x => x.Longitude.HasValue && x.Latitude.HasValue)
                    .Select(x => new ReservoirDisplayDto
                    {
                        ReservoirName = x.ReservoirName,
                        Longitude = x.Longitude.Value,
                        Latitude = x.Latitude.Value,
                        LowLevelPercentage = x.LowLevelPercentage,
                        MiddleLevelPercentage = x.MiddleLevelPercentage,
                        ConfigCreateTime = x.CreateTime
                    })
                    .OrderBy(x => x.ReservoirName)
                    .ToListAsync();

                _logger.LogInformation($"成功取得 {result.Count} 個水庫顯示資訊");

                if (result.Count == 0)
                {
                    _logger.LogWarning("FA_WR_ReservoirAlert 表中沒有包含經緯度的水庫資料");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得顯示水庫清單時發生錯誤");
                throw;
            }
        }
    }
}