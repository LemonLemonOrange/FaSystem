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
    /// 防汛資材控制器（水利署 API）
    /// </summary>
    [ApiController]
    [Route("api/watergov/flood-defense")]
    [ApiExplorerSettings(GroupName = "WraGov - 防汛")]
    [Produces("application/json")]
    public class FloodDefenseController : ControllerBase
    {
        #region 私有欄位

        private readonly ILogger<FloodDefenseController> _logger;
        private readonly IWraGovService _wraGovService;
        private readonly IMemoryCache _cache;

        #endregion

        #region 快取鍵常數

        private const string MaterialLocationCacheKey = "wraFloodDefenseMaterialLocation";
        private static readonly TimeSpan CacheExpiration = TimeSpan.FromHours(24); // 防汛資材位置較少變動，快取 24 小時

        #endregion

        #region 建構函式

        public FloodDefenseController(
            ILogger<FloodDefenseController> logger,
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
        /// 取得防汛資材位置資料
        /// GET /api/watergov/flood-defense/material-location
        /// </summary>
        [HttpGet("material-location")]
        public async Task<ActionResult<List<FloodDefenseMaterialLocationDto>>> GetMaterialLocation()
        {
            try
            {
                // 檢查快取
                if (_cache.TryGetValue(MaterialLocationCacheKey, out List<FloodDefenseMaterialLocationDto> cachedData))
                {
                    _logger.LogInformation("從快取取得防汛資材位置資料");
                    return Ok(cachedData);
                }

                // 取得最新資料
                var data = await _wraGovService.GetFloodDefenseMaterialLocationAsync();

                if (data == null || data.Count == 0)
                {
                    return NotFound(new { message = "目前無防汛資材位置資料" });
                }

                // 存入快取
                _cache.Set(MaterialLocationCacheKey, data, CacheExpiration);
                _logger.LogInformation($"成功取得並快取 {data.Count} 筆防汛資材位置資料");

                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得防汛資材位置資料時發生錯誤");
                return StatusCode(500, new { message = "取得防汛資材位置資料失敗", error = ex.Message });
            }
        }

        #endregion
    }
}