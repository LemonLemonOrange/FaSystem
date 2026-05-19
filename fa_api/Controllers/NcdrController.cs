using System;
using System.Threading.Tasks;
using fa_api.Dtos.Ncdr;
using fa_api.Services.Ncdr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace fa_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "NCDR")]
    [Produces("application/json")]
    public class NcdrController : ControllerBase
    {
        private readonly ILogger<NcdrController> _logger;
        private readonly INcdrDroughtService _droughtService;
        private readonly IMemoryCache _cache;
        
        private const string DroughtCacheKey = "ncdrDroughtAlert";
        private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(10);

        public NcdrController(
            ILogger<NcdrController> logger,
            INcdrDroughtService droughtService,
            IMemoryCache cache)
        {
            _logger = logger;
            _droughtService = droughtService;
            _cache = cache;
        }

        /// <summary>
        /// 取得最新枯旱預警資料及影響縣市（來自 NCDR）
        /// 每 10 分鐘更新一次（枯旱預警不頻繁更新）
        /// </summary>
        /// <returns>最新枯旱預警資訊</returns>
        [HttpGet("drought-alert")]
        public async Task<ActionResult<DroughtAlertDto>> GetDroughtAlert()
        {
            try
            {
                // 檢查快取
                if (_cache.TryGetValue(DroughtCacheKey, out DroughtAlertDto cachedAlert))
                {
                    _logger.LogInformation("從快取取得枯旱預警資料");
                    return Ok(cachedAlert);
                }

                // 取得最新資料
                var alert = await _droughtService.FetchDroughtAlertAsync();

                if (alert == null)
                {
                    return NotFound(new { message = "目前無枯旱預警資料" });
                }

                // 存入快取
                _cache.Set(DroughtCacheKey, alert, CacheExpiration);
                _logger.LogInformation("成功取得並快取枯旱預警資料");

                return Ok(alert);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得枯旱預警資料時發生錯誤");
                return StatusCode(500, new { message = "取得枯旱預警資料失敗", error = ex.Message });
            }
        }
    }
}
