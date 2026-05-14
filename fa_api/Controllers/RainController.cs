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
    /// 雨量資料控制器（水利署 API）
    /// </summary>
    [ApiController]
    [Route("api/watergov/rain")]
    [ApiExplorerSettings(GroupName = "WraGov - 雨量")]
    [Produces("application/json")]
    public class RainController : ControllerBase
    {
        #region 私有欄位

        private readonly ILogger<RainController> _logger;
        private readonly IWraGovService _wraGovService;
        private readonly IMemoryCache _cache;

        #endregion

        #region 快取鍵常數

        private const string StationCacheKey = "wraRainStation";
        private const string RealTimeInfoCacheKey = "wraRainRealTimeInfo";
        private const string WarningCacheKey = "wraRainWarning";
        private const string AffectedAreaCacheKey = "wraRainAffectedArea";

        private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(10);

        #endregion

        #region 建構函式

        public RainController(
            ILogger<RainController> logger,
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
        /// 取得雨量站基本資料
        /// GET /api/watergov/rain/station
        /// </summary>
        [HttpGet("station")]
        public async Task<ActionResult<List<RainStationDto>>> GetRainStation()
        {
            try
            {
                if (_cache.TryGetValue(StationCacheKey, out List<RainStationDto> cachedData))
                {
                    _logger.LogInformation("從快取取得雨量站資料");
                    return Ok(cachedData);
                }

                var data = await _wraGovService.GetRainStationAsync();

                if (data == null || data.Count == 0)
                {
                    return NotFound(new { message = "目前無雨量站資料" });
                }

                _cache.Set(StationCacheKey, data, CacheExpiration);
                _logger.LogInformation($"成功取得並快取 {data.Count} 筆雨量站資料");

                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得雨量站資料時發生錯誤");
                return StatusCode(500, new { message = "取得雨量站資料失敗", error = ex.Message });
            }
        }

        /// <summary>
        /// 取得雨量即時資訊
        /// GET /api/watergov/rain/real-time-info
        /// </summary>
        [HttpGet("real-time-info")]
        public async Task<ActionResult<List<RainRealTimeInfoDto>>> GetRainRealTimeInfo()
        {
            try
            {
                if (_cache.TryGetValue(RealTimeInfoCacheKey, out List<RainRealTimeInfoDto> cachedData))
                {
                    _logger.LogInformation("從快取取得雨量即時資訊");
                    return Ok(cachedData);
                }

                var data = await _wraGovService.GetRainRealTimeInfoAsync();

                if (data == null || data.Count == 0)
                {
                    return NotFound(new { message = "目前無雨量即時資訊" });
                }

                _cache.Set(RealTimeInfoCacheKey, data, CacheExpiration);
                _logger.LogInformation($"成功取得並快取 {data.Count} 筆雨量即時資訊");

                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得雨量即時資訊時發生錯誤");
                return StatusCode(500, new { message = "取得雨量即時資訊失敗", error = ex.Message });
            }
        }

        /// <summary>
        /// 取得雨量警示資料
        /// GET /api/watergov/rain/warning
        /// </summary>
        [HttpGet("warning")]
        public async Task<ActionResult<List<RainWarningDto>>> GetRainWarning()
        {
            try
            {
                if (_cache.TryGetValue(WarningCacheKey, out List<RainWarningDto> cachedData))
                {
                    _logger.LogInformation("從快取取得雨量警示資料");
                    return Ok(cachedData);
                }

                var data = await _wraGovService.GetRainWarningAsync();

                if (data == null || data.Count == 0)
                {
                    return NotFound(new { message = "目前無雨量警示資料" });
                }

                _cache.Set(WarningCacheKey, data, CacheExpiration);
                _logger.LogInformation($"成功取得並快取 {data.Count} 筆雨量警示資料");

                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得雨量警示資料時發生錯誤");
                return StatusCode(500, new { message = "取得雨量警示資料失敗", error = ex.Message });
            }
        }

        /// <summary>
        /// 取得雨量影響區域
        /// GET /api/watergov/rain/affected-area
        /// </summary>
        [HttpGet("affected-area")]
        public async Task<ActionResult<List<RainAffectedAreaDto>>> GetRainAffectedArea()
        {
            try
            {
                if (_cache.TryGetValue(AffectedAreaCacheKey, out List<RainAffectedAreaDto> cachedData))
                {
                    _logger.LogInformation("從快取取得雨量影響區域資料");
                    return Ok(cachedData);
                }

                var data = await _wraGovService.GetRainAffectedAreaAsync();

                if (data == null || data.Count == 0)
                {
                    return NotFound(new { message = "目前無雨量影響區域資料" });
                }

                _cache.Set(AffectedAreaCacheKey, data, CacheExpiration);
                _logger.LogInformation($"成功取得並快取 {data.Count} 筆雨量影響區域資料");

                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得雨量影響區域資料時發生錯誤");
                return StatusCode(500, new { message = "取得雨量影響區域資料失敗", error = ex.Message });
            }
        }

        #endregion
    }
}