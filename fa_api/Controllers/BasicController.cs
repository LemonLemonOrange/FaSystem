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
    /// 基本資料控制器（縣市、鄉鎮等）
    /// </summary>
    [ApiController]
    [Route("api/watergov/basic")]
    [ApiExplorerSettings(GroupName = "WraGov - 基本資料")]
    [Produces("application/json")]
    public class BasicController : ControllerBase
    {
        #region 私有欄位

        private readonly ILogger<BasicController> _logger;
        private readonly IWraGovService _wraGovService;
        private readonly IMemoryCache _cache;

        #endregion

        #region 快取鍵常數

        private const string CityCacheKey = "wraCity";
        private const string TownCacheKeyPrefix = "wraTown_";
        private static readonly TimeSpan CacheExpiration = TimeSpan.FromHours(24); // 基本資料快取 24 小時

        #endregion

        #region 建構函式

        public BasicController(
            ILogger<BasicController> logger,
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
        /// 取得縣市資料
        /// GET /api/watergov/basic/city
        /// </summary>
        [HttpGet("city")]
        public async Task<ActionResult<List<CityDto>>> GetCity()
        {
            try
            {
                // 檢查快取
                if (_cache.TryGetValue(CityCacheKey, out List<CityDto> cachedData))
                {
                    _logger.LogInformation("從快取取得縣市資料");
                    return Ok(cachedData);
                }

                // 取得最新資料
                var data = await _wraGovService.GetCityAsync();

                if (data == null || data.Count == 0)
                {
                    return NotFound(new { message = "目前無縣市資料" });
                }

                // 存入快取（基本資料快取時間較長）
                _cache.Set(CityCacheKey, data, CacheExpiration);
                _logger.LogInformation($"成功取得並快取 {data.Count} 筆縣市資料");

                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得縣市資料時發生錯誤");
                return StatusCode(500, new { message = "取得縣市資料失敗", error = ex.Message });
            }
        }

        /// <summary>
        /// 取得指定縣市的鄉鎮資料
        /// GET /api/watergov/basic/{cityCode}/town
        /// </summary>
        /// <param name="cityCode">縣市代碼（例如：63000 代表臺北市）</param>
        [HttpGet("{cityCode}/town")]
        public async Task<ActionResult<List<TownDto>>> GetTownByCity(string cityCode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cityCode))
                {
                    return BadRequest(new { message = "縣市代碼不可為空" });
                }

                var cacheKey = $"{TownCacheKeyPrefix}{cityCode}";

                // 檢查快取
                if (_cache.TryGetValue(cacheKey, out List<TownDto> cachedData))
                {
                    _logger.LogInformation($"從快取取得鄉鎮資料（{cityCode}）");
                    return Ok(cachedData);
                }

                // 取得最新資料
                var data = await _wraGovService.GetTownByCityAsync(cityCode);

                if (data == null || data.Count == 0)
                {
                    return NotFound(new { message = $"找不到縣市代碼 {cityCode} 的鄉鎮資料" });
                }

                // 存入快取
                _cache.Set(cacheKey, data, CacheExpiration);
                _logger.LogInformation($"成功取得並快取 {data.Count} 筆鄉鎮資料（{cityCode}）");

                return Ok(data);
            }
            catch (ArgumentException argEx)
            {
                _logger.LogWarning(argEx, $"參數錯誤: {cityCode}");
                return BadRequest(new { message = argEx.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"取得鄉鎮資料時發生錯誤（{cityCode}）");
                return StatusCode(500, new { message = "取得鄉鎮資料失敗", error = ex.Message });
            }
        }

        #endregion
    }
}