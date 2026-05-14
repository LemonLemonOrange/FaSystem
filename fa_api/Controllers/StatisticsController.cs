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
    /// 統計資料控制器（水利署 API）
    /// </summary>
    [ApiController]
    [Route("api/watergov/statistics")]
    [ApiExplorerSettings(GroupName = "WraGov - 統計")]
    [Produces("application/json")]
    public class StatisticsController : ControllerBase
    {
        #region 私有欄位

        private readonly ILogger<StatisticsController> _logger;
        private readonly IWraGovService _wraGovService;
        private readonly IMemoryCache _cache;

        #endregion

        #region 快取鍵常數

        private const string FloodingCacheKeyPrefix = "wraStatisticsFlooding_";
        private const string WaterFacilityCacheKeyPrefix = "wraStatisticsWaterFacility_";
        private const string FloodDefenseMaterialCacheKey = "wraStatisticsFloodDefenseMaterial";

        private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(30);

        #endregion

        #region 建構函式

        public StatisticsController(
            ILogger<StatisticsController> logger,
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
        /// 取得淹水統計資料
        /// GET /api/watergov/statistics/flooding/{eventNo}
        /// </summary>
        /// <param name="eventNo">事件編號</param>
        [HttpGet("flooding/{eventNo}")]
        public async Task<ActionResult<List<DisasterFloodingStatisticsDto>>> GetStatisticsFlooding(string eventNo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(eventNo))
                {
                    return BadRequest(new { message = "事件編號不可為空" });
                }

                var cacheKey = $"{FloodingCacheKeyPrefix}{eventNo}";

                // 檢查快取
                if (_cache.TryGetValue(cacheKey, out List<DisasterFloodingStatisticsDto> cachedData))
                {
                    _logger.LogInformation($"從快取取得淹水統計資料（{eventNo}）");
                    return Ok(cachedData);
                }

                // 取得最新資料
                var data = await _wraGovService.GetStatisticsFloodingAsync(eventNo);

                if (data == null || data.Count == 0)
                {
                    return NotFound(new { message = $"事件 {eventNo} 無淹水統計資料" });
                }

                // 存入快取
                _cache.Set(cacheKey, data, CacheExpiration);
                _logger.LogInformation($"成功取得並快取 {data.Count} 筆淹水統計資料（{eventNo}）");

                return Ok(data);
            }
            catch (ArgumentException argEx)
            {
                _logger.LogWarning(argEx, $"參數錯誤: {eventNo}");
                return BadRequest(new { message = argEx.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"取得淹水統計資料時發生錯誤（{eventNo}）");
                return StatusCode(500, new { message = "取得淹水統計資料失敗", error = ex.Message });
            }
        }

        /// <summary>
        /// 取得水利設施統計資料
        /// GET /api/watergov/statistics/water-facility/{eventNo}
        /// </summary>
        /// <param name="eventNo">事件編號</param>
        [HttpGet("water-facility/{eventNo}")]
        public async Task<ActionResult<List<DisasterWaterFacilityStatisticsDto>>> GetStatisticsWaterFacility(string eventNo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(eventNo))
                {
                    return BadRequest(new { message = "事件編號不可為空" });
                }

                var cacheKey = $"{WaterFacilityCacheKeyPrefix}{eventNo}";

                // 檢查快取
                if (_cache.TryGetValue(cacheKey, out List<DisasterWaterFacilityStatisticsDto> cachedData))
                {
                    _logger.LogInformation($"從快取取得水利設施統計資料（{eventNo}）");
                    return Ok(cachedData);
                }

                // 取得最新資料
                var data = await _wraGovService.GetStatisticsWaterFacilityAsync(eventNo);

                if (data == null || data.Count == 0)
                {
                    return NotFound(new { message = $"事件 {eventNo} 無水利設施統計資料" });
                }

                // 存入快取
                _cache.Set(cacheKey, data, CacheExpiration);
                _logger.LogInformation($"成功取得並快取 {data.Count} 筆水利設施統計資料（{eventNo}）");

                return Ok(data);
            }
            catch (ArgumentException argEx)
            {
                _logger.LogWarning(argEx, $"參數錯誤: {eventNo}");
                return BadRequest(new { message = argEx.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"取得水利設施統計資料時發生錯誤（{eventNo}）");
                return StatusCode(500, new { message = "取得水利設施統計資料失敗", error = ex.Message });
            }
        }

        /// <summary>
        /// 取得防汛資材統計資料
        /// GET /api/watergov/statistics/flood-defense-material
        /// </summary>
        [HttpGet("flood-defense-material")]
        public async Task<ActionResult<List<FloodDefenseOperatorDto>>> GetStatisticsFloodDefenseMaterial()
        {
            try
            {
                // 檢查快取
                if (_cache.TryGetValue(FloodDefenseMaterialCacheKey, out List<FloodDefenseOperatorDto> cachedData))
                {
                    _logger.LogInformation("從快取取得防汛資材統計資料");
                    return Ok(cachedData);
                }

                // 取得最新資料
                var data = await _wraGovService.GetStatisticsFloodDefenseMaterialAsync();

                if (data == null || data.Count == 0)
                {
                    return NotFound(new { message = "目前無防汛資材統計資料" });
                }

                // 存入快取
                _cache.Set(FloodDefenseMaterialCacheKey, data, CacheExpiration);
                _logger.LogInformation($"成功取得並快取 {data.Count} 筆防汛資材統計資料");

                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得防汛資材統計資料時發生錯誤");
                return StatusCode(500, new { message = "取得防汛資材統計資料失敗", error = ex.Message });
            }
        }

        #endregion
    }
}