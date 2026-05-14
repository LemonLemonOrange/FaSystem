using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using fa_api.Dtos.WraGov;
using fa_api.Services.WraGov;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace fa_api.Controllers
{
    /// <summary>
    /// 水庫資料控制器（水利署 API）
    /// </summary>
    [ApiController]
    [Route("api/watergov/reservoir")]
    [ApiExplorerSettings(GroupName = "WraGov - 水庫")]
    [Produces("application/json")]
    public class ReservoirController : ControllerBase
    {
        #region 私有欄位

        private readonly ILogger<ReservoirController> _logger;
        private readonly IWraGovService _wraGovService;
        private readonly IMemoryCache _cache;

        #endregion

        #region 快取鍵常數

        private const string StationCacheKey = "wraReservoirStation";
        private const string RealTimeInfoCacheKey = "wraReservoirRealTimeInfo";
        private const string DailyCacheKey = "wraReservoirDaily";
        private const string WarningCacheKey = "wraReservoirWarning";
        private const string AffectedAreaCacheKey = "wraReservoirAffectedArea";

        private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(10);

        #endregion

        #region 建構函式

        public ReservoirController(
            ILogger<ReservoirController> logger,
            IWraGovService wraGovService,
            IMemoryCache cache)
        {
            _logger = logger;
            _wraGovService = wraGovService;
            _cache = cache;
        }

        #endregion

        #region API 端點

        /// <summary>
        /// 取得水庫測站基本資料
        /// GET /api/watergov/reservoir/station
        /// </summary>
        [HttpGet("station")]
        public async Task<ActionResult<List<ReservoirStationDto>>> GetReservoirStation()
        {
            try
            {
                if (_cache.TryGetValue(StationCacheKey, out List<ReservoirStationDto> cachedData))
                {
                    _logger.LogInformation("從快取取得水庫測站資料");
                    return Ok(cachedData);
                }

                var data = await _wraGovService.GetReservoirStationAsync();

                if (data == null || data.Count == 0)
                {
                    return NotFound(new { message = "目前無水庫測站資料" });
                }

                _cache.Set(StationCacheKey, data, CacheExpiration);
                _logger.LogInformation($"成功取得並快取 {data.Count} 筆水庫測站資料");

                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得水庫測站資料時發生錯誤");
                return StatusCode(500, new { message = "取得水庫測站資料失敗", error = ex.Message });
            }
        }

        /// <summary>
        /// 取得水庫即時資訊
        /// GET /api/watergov/reservoir/real-time-info
        /// </summary>
        [HttpGet("real-time-info")]
        public async Task<ActionResult<List<ReservoirRealTimeInfoDto>>> GetReservoirRealTimeInfo()
        {
            try
            {
                if (_cache.TryGetValue(RealTimeInfoCacheKey, out List<ReservoirRealTimeInfoDto> cachedData))
                {
                    _logger.LogInformation("從快取取得水庫即時資訊");
                    return Ok(cachedData);
                }

                var data = await _wraGovService.GetReservoirRealTimeInfoAsync();

                if (data == null || data.Count == 0)
                {
                    return NotFound(new { message = "目前無水庫即時資訊" });
                }

                _cache.Set(RealTimeInfoCacheKey, data, CacheExpiration);
                _logger.LogInformation($"成功取得並快取 {data.Count} 筆水庫即時資訊");

                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得水庫即時資訊時發生錯誤");
                return StatusCode(500, new { message = "取得水庫即時資訊失敗", error = ex.Message });
            }
        }

        /// <summary>
        /// 取得水庫每日資料
        /// GET /api/watergov/reservoir/daily
        /// </summary>
        [HttpGet("daily")]
        public async Task<ActionResult<List<ReservoirDailyDto>>> GetReservoirDaily()
        {
            try
            {
                if (_cache.TryGetValue(DailyCacheKey, out List<ReservoirDailyDto> cachedData))
                {
                    _logger.LogInformation("從快取取得水庫每日資料");
                    return Ok(cachedData);
                }

                var data = await _wraGovService.GetReservoirDailyAsync();

                if (data == null || data.Count == 0)
                {
                    return NotFound(new { message = "目前無水庫每日資料" });
                }

                _cache.Set(DailyCacheKey, data, CacheExpiration);
                _logger.LogInformation($"成功取得並快取 {data.Count} 筆水庫每日資料");

                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得水庫每日資料時發生錯誤");
                return StatusCode(500, new { message = "取得水庫每日資料失敗", error = ex.Message });
            }
        }

        /// <summary>
        /// 取得水庫警示資料
        /// GET /api/watergov/reservoir/warning
        /// </summary>
        [HttpGet("warning")]
        public async Task<ActionResult<List<ReservoirWarningDto>>> GetReservoirWarning()
        {
            try
            {
                if (_cache.TryGetValue(WarningCacheKey, out List<ReservoirWarningDto> cachedData))
                {
                    _logger.LogInformation("從快取取得水庫警示資料");
                    return Ok(cachedData);
                }

                var data = await _wraGovService.GetReservoirWarningAsync();

                if (data == null || data.Count == 0)
                {
                    return NotFound(new { message = "目前無水庫警示資料" });
                }

                _cache.Set(WarningCacheKey, data, CacheExpiration);
                _logger.LogInformation($"成功取得並快取 {data.Count} 筆水庫警示資料");

                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得水庫警示資料時發生錯誤");
                return StatusCode(500, new { message = "取得水庫警示資料失敗", error = ex.Message });
            }
        }

        /// <summary>
        /// 取得水庫影響區域
        /// GET /api/watergov/reservoir/affected-area
        /// </summary>
        [HttpGet("affected-area")]
        public async Task<ActionResult<List<ReservoirAffectedAreaDto>>> GetReservoirAffectedArea()
        {
            try
            {
                if (_cache.TryGetValue(AffectedAreaCacheKey, out List<ReservoirAffectedAreaDto> cachedData))
                {
                    _logger.LogInformation("從快取取得水庫影響區域資料");
                    return Ok(cachedData);
                }

                var data = await _wraGovService.GetReservoirAffectedAreaAsync();

                if (data == null || data.Count == 0)
                {
                    return NotFound(new { message = "目前無水庫影響區域資料" });
                }

                _cache.Set(AffectedAreaCacheKey, data, CacheExpiration);
                _logger.LogInformation($"成功取得並快取 {data.Count} 筆水庫影響區域資料");

                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得水庫影響區域資料時發生錯誤");
                return StatusCode(500, new { message = "取得水庫影響區域資料失敗", error = ex.Message });
            }
        }

        #endregion
    }
}