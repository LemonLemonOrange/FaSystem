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
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "WraGov - 水位站")]
    [Produces("application/json")]
    public class WaterGovController : ControllerBase
    {
        #region 私有欄位

        private readonly ILogger<WaterGovController> _logger;
        private readonly IWraGovService _wraGovService;
        private readonly IMemoryCache _cache;

        #endregion

        #region 快取鍵常數

        private const string ReservoirCacheKey = "wraReservoirData";
        private const string OperationCacheKey = "wraOperationData";
        private const string AlarmCacheKey = "wraAlarmData";
        private const string SupplyCacheKey = "wraSupplyData";
        private const string WaterStationCacheKey = "wraWaterStation";
        private const string WaterRealTimeInfoCacheKey = "wraWaterRealTimeInfo";
        private const string WaterWarningCacheKey = "wraWaterWarning";
        private const string WaterLevelCacheKey = "wraWaterLevelData";
        private const string RainfallCacheKey = "wraRainfallData";

        private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(10);

        #endregion

        #region 建構函式

        public WaterGovController(
            ILogger<WaterGovController> logger,
            IWraGovService wraGovService,
            IMemoryCache cache)
        {
            _logger = logger;
            _wraGovService = wraGovService;
            _cache = cache;
        }

        #endregion

        #region 水庫相關 API

        /// <summary>
        /// 取得所有水庫即時水情
        /// GET /api/watergov/statistics
        /// </summary>
        [HttpGet("statistics")]
        public async Task<ActionResult<List<ReservoirDataDto>>> GetReservoirStatistics()
        {
            try
            {
                if (_cache.TryGetValue(ReservoirCacheKey, out List<ReservoirDataDto> cachedData))
                {
                    _logger.LogInformation("從快取取得水庫資料");
                    return Ok(cachedData);
                }

                var data = await _wraGovService.GetReservoirStatisticsAsync();

                if (data == null || data.Count == 0)
                {
                    return NotFound(new { message = "目前無水庫資料" });
                }

                _cache.Set(ReservoirCacheKey, data, CacheExpiration);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得水庫資料時發生錯誤");
                return StatusCode(500, new { message = "取得水庫資料失敗", error = ex.Message });
            }
        }

        /// <summary>
        /// 取得指定水庫的即時水情
        /// GET /api/watergov/statistics/{name}
        /// </summary>
        [HttpGet("statistics/{name}")]
        public async Task<ActionResult<ReservoirDataDto>> GetReservoirByName(string name)
        {
            try
            {
                var data = await _wraGovService.GetReservoirByNameAsync(name);
                return Ok(data);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = $"找不到水庫: {name}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"取得水庫資料時發生錯誤: {name}");
                return StatusCode(500, new { message = "取得水庫資料失敗", error = ex.Message });
            }
        }

        /// <summary>
        /// 取得水庫每日營運狀況
        /// GET /api/watergov/operation
        /// </summary>
        [HttpGet("operation")]
        public async Task<ActionResult<List<ReservoirOperationDto>>> GetReservoirOperation()
        {
            try
            {
                if (_cache.TryGetValue(OperationCacheKey, out List<ReservoirOperationDto> cachedData))
                {
                    return Ok(cachedData);
                }

                var data = await _wraGovService.GetReservoirOperationAsync();
                _cache.Set(OperationCacheKey, data, CacheExpiration);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得營運資料時發生錯誤");
                return StatusCode(500, new { message = "取得營運資料失敗", error = ex.Message });
            }
        }

        /// <summary>
        /// 取得水庫放水警戒
        /// GET /api/watergov/overflow-alarm
        /// </summary>
        [HttpGet("overflow-alarm")]
        public async Task<ActionResult<List<OverflowAlarmDto>>> GetOverflowAlarm()
        {
            try
            {
                if (_cache.TryGetValue(AlarmCacheKey, out List<OverflowAlarmDto> cachedData))
                {
                    return Ok(cachedData);
                }

                var data = await _wraGovService.GetOverflowAlarmAsync();
                _cache.Set(AlarmCacheKey, data, CacheExpiration);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得放水警戒資料時發生錯誤");
                return StatusCode(500, new { message = "取得放水警戒資料失敗", error = ex.Message });
            }
        }

        #endregion

        #region 供水相關 API

        /// <summary>
        /// 取得各地區供水狀況
        /// GET /api/watergov/supply-condition
        /// </summary>
        [HttpGet("supply-condition")]
        public async Task<ActionResult<List<WaterSupplyConditionDto>>> GetWaterSupplyCondition()
        {
            try
            {
                if (_cache.TryGetValue(SupplyCacheKey, out List<WaterSupplyConditionDto> cachedData))
                {
                    return Ok(cachedData);
                }

                var data = await _wraGovService.GetWaterSupplyConditionAsync();
                _cache.Set(SupplyCacheKey, data, CacheExpiration);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得供水狀況時發生錯誤");
                return StatusCode(500, new { message = "取得供水狀況失敗", error = ex.Message });
            }
        }

        #endregion

        #region 水位站相關 API

        /// <summary>
        /// 取得水位站基本資料
        /// GET /api/watergov/water/station
        /// </summary>
        [HttpGet("water/station")]
        public async Task<ActionResult<List<WaterStationDto>>> GetWaterStation()
        {
            try
            {
                if (_cache.TryGetValue(WaterStationCacheKey, out List<WaterStationDto> cachedData))
                {
                    _logger.LogInformation("從快取取得水位站資料");
                    return Ok(cachedData);
                }

                var data = await _wraGovService.GetWaterStationAsync();

                if (data == null || data.Count == 0)
                {
                    return NotFound(new { message = "目前無水位站資料" });
                }

                _cache.Set(WaterStationCacheKey, data, CacheExpiration);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得水位站資料時發生錯誤");
                return StatusCode(500, new { message = "取得水位站資料失敗", error = ex.Message });
            }
        }

        /// <summary>
        /// 取得水位即時資訊
        /// GET /api/watergov/water/real-time-info
        /// </summary>
        [HttpGet("water/real-time-info")]
        public async Task<ActionResult<List<WaterRealTimeInfoDto>>> GetWaterRealTimeInfo()
        {
            try
            {
                if (_cache.TryGetValue(WaterRealTimeInfoCacheKey, out List<WaterRealTimeInfoDto> cachedData))
                {
                    _logger.LogInformation("從快取取得水位即時資訊");
                    return Ok(cachedData);
                }

                var data = await _wraGovService.GetWaterRealTimeInfoAsync();

                if (data == null || data.Count == 0)
                {
                    return NotFound(new { message = "目前無水位即時資訊" });
                }

                _cache.Set(WaterRealTimeInfoCacheKey, data, CacheExpiration);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得水位即時資訊時發生錯誤");
                return StatusCode(500, new { message = "取得水位即時資訊失敗", error = ex.Message });
            }
        }

        /// <summary>
        /// 取得水位警示資料
        /// GET /api/watergov/water/warning
        /// </summary>
        [HttpGet("water/warning")]
        public async Task<ActionResult<List<WaterWarningDto>>> GetWaterWarning()
        {
            try
            {
                if (_cache.TryGetValue(WaterWarningCacheKey, out List<WaterWarningDto> cachedData))
                {
                    _logger.LogInformation("從快取取得水位警示資料");
                    return Ok(cachedData);
                }

                var data = await _wraGovService.GetWaterWarningAsync();

                if (data == null || data.Count == 0)
                {
                    return NotFound(new { message = "目前無水位警示資料" });
                }

                _cache.Set(WaterWarningCacheKey, data, CacheExpiration);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得水位警示資料時發生錯誤");
                return StatusCode(500, new { message = "取得水位警示資料失敗", error = ex.Message });
            }
        }

        #endregion

        #region 水文相關 API

        /// <summary>
        /// 取得即時水位資料
        /// GET /api/watergov/water-level?stationNo={編號}
        /// </summary>
        [HttpGet("water-level")]
        public async Task<ActionResult<List<WaterLevelDataDto>>> GetWaterLevel([FromQuery] string stationNo = null)
        {
            try
            {
                var cacheKey = $"{WaterLevelCacheKey}_{stationNo ?? "all"}";

                if (_cache.TryGetValue(cacheKey, out List<WaterLevelDataDto> cachedData))
                {
                    return Ok(cachedData);
                }

                var data = await _wraGovService.GetWaterLevelRealTimeAsync(stationNo);
                _cache.Set(cacheKey, data, CacheExpiration);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得水位資料時發生錯誤");
                return StatusCode(500, new { message = "取得水位資料失敗", error = ex.Message });
            }
        }

        /// <summary>
        /// 取得即時雨量資料
        /// GET /api/watergov/rainfall?stationNo={編號}
        /// </summary>
        [HttpGet("rainfall")]
        public async Task<ActionResult<List<RainfallDataDto>>> GetRainfall([FromQuery] string stationNo = null)
        {
            try
            {
                var cacheKey = $"{RainfallCacheKey}_{stationNo ?? "all"}";

                if (_cache.TryGetValue(cacheKey, out List<RainfallDataDto> cachedData))
                {
                    return Ok(cachedData);
                }

                var data = await _wraGovService.GetRainfallRealTimeAsync(stationNo);
                _cache.Set(cacheKey, data, CacheExpiration);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得雨量資料時發生錯誤");
                return StatusCode(500, new { message = "取得雨量資料失敗", error = ex.Message });
            }
        }

        #endregion
    }
}