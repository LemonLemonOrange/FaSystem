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
    /// 事件資料控制器（水利署 API）
    /// </summary>
    [ApiController]
    [Route("api/watergov/event")]
    [ApiExplorerSettings(GroupName = "WraGov - 事件")]
    [Produces("application/json")]
    public class EventController : ControllerBase
    {
        #region 私有欄位

        private readonly ILogger<EventController> _logger;
        private readonly IWraGovService _wraGovService;
        private readonly IMemoryCache _cache;

        #endregion

        #region 快取鍵常數

        private const string EventCacheKeyPrefix = "wraEvent_";
        private static readonly TimeSpan CacheExpiration = TimeSpan.FromHours(1); // 事件資料快取 1 小時

        #endregion

        #region 建構函式

        public EventController(
            ILogger<EventController> logger,
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
        /// 依年份取得事件資料
        /// GET /api/watergov/event/{year}
        /// </summary>
        /// <param name="year">西元年份（例如：2026）</param>
        [HttpGet("{year}")]
        public async Task<ActionResult<List<EventDto>>> GetEventByYear(int year)
        {
            try
            {
                // 驗證年份
                if (year < 1900 || year > 2100)
                {
                    return BadRequest(new { message = "年份範圍必須在 1900-2100 之間" });
                }

                var cacheKey = $"{EventCacheKeyPrefix}{year}";

                // 檢查快取
                if (_cache.TryGetValue(cacheKey, out List<EventDto> cachedData))
                {
                    _logger.LogInformation($"從快取取得事件資料（{year}年）");
                    return Ok(cachedData);
                }

                // 取得最新資料
                var data = await _wraGovService.GetEventByYearAsync(year);

                if (data == null || data.Count == 0)
                {
                    return NotFound(new { message = $"{year}年無事件資料" });
                }

                // 存入快取
                _cache.Set(cacheKey, data, CacheExpiration);
                _logger.LogInformation($"成功取得並快取 {data.Count} 筆事件資料（{year}年）");

                return Ok(data);
            }
            catch (ArgumentException argEx)
            {
                _logger.LogWarning(argEx, $"參數錯誤: {year}");
                return BadRequest(new { message = argEx.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"取得事件資料時發生錯誤（{year}年）");
                return StatusCode(500, new { message = "取得事件資料失敗", error = ex.Message });
            }
        }

        #endregion
    }
}