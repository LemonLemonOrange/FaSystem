using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using fa_api.Dtos.WraGov;
using fa_api.Services.WraGov;
using fa_api.Services.WrRecorder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace fa_api.Controllers
{
    [ApiController]
    [Route("api/watergov")]
    [ApiExplorerSettings(GroupName = "WraGov")]
    [Produces("application/json")]
    public class WaterGovController : ControllerBase
    {
        private readonly ILogger<WaterGovController> _logger;
        private readonly IWraGovService _wraGovService;
        private readonly IMemoryCache _cache;
        private readonly IWrRecorderService _wrRecorderService;

        // Cache keys
        private const string CityCacheKey = "wraCity";
        private const string TownCacheKeyPrefix = "wraTown_";
        private const string EventCacheKeyPrefix = "wraEvent_";
        private const string MaterialLocationCacheKey = "wraFloodDefenseMaterialLocation";
        private const string RainStationCacheKey = "wraRainStation";
        private const string RainRealTimeInfoCacheKey = "wraRainRealTimeInfo";
        private const string RainWarningCacheKey = "wraRainWarning";
        private const string RainAffectedAreaCacheKey = "wraRainAffectedArea";
        private const string ReservoirStationCacheKey = "wraReservoirStation";
        private const string ReservoirRealTimeInfoCacheKey = "wraReservoirRealTimeInfo";
        private const string ReservoirDailyCacheKey = "wraReservoirDaily";
        private const string ReservoirWarningCacheKey = "wraReservoirWarning";
        private const string ReservoirAffectedAreaCacheKey = "wraReservoirAffectedArea";
        private const string StatFloodingCacheKeyPrefix = "wraStatisticsFlooding_";
        private const string StatWaterFacilityCacheKeyPrefix = "wraStatisticsWaterFacility_";
        private const string StatFloodDefenseMaterialCacheKey = "wraStatisticsFloodDefenseMaterial";
        private const string ReservoirDataCacheKey = "wraReservoirData";
        private const string OperationCacheKey = "wraOperationData";
        private const string AlarmCacheKey = "wraAlarmData";
        private const string SupplyCacheKey = "wraSupplyData";
        private const string WaterStationCacheKey = "wraWaterStation";
        private const string WaterRealTimeInfoCacheKey = "wraWaterRealTimeInfo";
        private const string WaterWarningCacheKey = "wraWaterWarning";
        private const string WaterLevelCacheKey = "wraWaterLevelData";
        private const string RainfallCacheKey = "wraRainfallData";

        private static readonly TimeSpan ShortCache = TimeSpan.FromMinutes(10);
        private static readonly TimeSpan MediumCache = TimeSpan.FromHours(1);
        private static readonly TimeSpan LongCache = TimeSpan.FromHours(24);

        public WaterGovController(
            ILogger<WaterGovController> logger,
            IWraGovService wraGovService,
            IMemoryCache cache,
            IWrRecorderService wrRecorderService)
        {
            _logger = logger;
            _wraGovService = wraGovService;
            _cache = cache;
            _wrRecorderService = wrRecorderService;
        }

        #region Basic

        [HttpGet("basic/city")]
        public async Task<ActionResult<List<CityDto>>> GetCity()
        {
            try
            {
                if (_cache.TryGetValue(CityCacheKey, out List<CityDto> cached)) return Ok(cached);
                var data = await _wraGovService.GetCityAsync();
                if (data == null || data.Count == 0) return NotFound(new { message = "目前無縣市資料" });
                _cache.Set(CityCacheKey, data, LongCache);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得縣市資料發生錯誤");
                return StatusCode(500, new { message = "取得縣市資料失敗", error = ex.Message });
            }
        }

        [HttpGet("basic/{cityCode}/town")]
        public async Task<ActionResult<List<TownDto>>> GetTownByCity(string cityCode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cityCode))
                    return BadRequest(new { message = "縣市代碼不可空白" });
                var cacheKey = $"{TownCacheKeyPrefix}{cityCode}";
                if (_cache.TryGetValue(cacheKey, out List<TownDto> cached)) return Ok(cached);
                var data = await _wraGovService.GetTownByCityAsync(cityCode);
                if (data == null || data.Count == 0) return NotFound(new { message = $"縣市代碼 {cityCode} 無鄉鎮資料" });
                _cache.Set(cacheKey, data, LongCache);
                return Ok(data);
            }
            catch (ArgumentException argEx)
            {
                return BadRequest(new { message = argEx.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"取得鄉鎮資料發生錯誤 [{cityCode}]");
                return StatusCode(500, new { message = "取得鄉鎮資料失敗", error = ex.Message });
            }
        }

        #endregion

        #region Event

        [HttpGet("event/{year}")]
        public async Task<ActionResult<List<EventDto>>> GetEventByYear(int year)
        {
            try
            {
                if (year < 1900 || year > 2100)
                    return BadRequest(new { message = "年份範圍需在 1900-2100 之間" });
                var cacheKey = $"{EventCacheKeyPrefix}{year}";
                if (_cache.TryGetValue(cacheKey, out List<EventDto> cached)) return Ok(cached);
                var data = await _wraGovService.GetEventByYearAsync(year);
                if (data == null || data.Count == 0) return NotFound(new { message = $"{year}年無事件資料" });
                _cache.Set(cacheKey, data, MediumCache);
                return Ok(data);
            }
            catch (ArgumentException argEx)
            {
                return BadRequest(new { message = argEx.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"取得事件資料發生錯誤 [{year}年]");
                return StatusCode(500, new { message = "取得事件資料失敗", error = ex.Message });
            }
        }

        #endregion

        #region FloodDefense

        [HttpGet("flood-defense/material-location")]
        public async Task<ActionResult<List<FloodDefenseMaterialLocationDto>>> GetMaterialLocation()
        {
            try
            {
                if (_cache.TryGetValue(MaterialLocationCacheKey, out List<FloodDefenseMaterialLocationDto> cached)) return Ok(cached);
                var data = await _wraGovService.GetFloodDefenseMaterialLocationAsync();
                if (data == null || data.Count == 0) return NotFound(new { message = "目前無防汛資材位置資料" });
                _cache.Set(MaterialLocationCacheKey, data, MediumCache);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得防汛資材位置資料發生錯誤");
                return StatusCode(500, new { message = "取得防汛資材位置資料失敗", error = ex.Message });
            }
        }

        #endregion

        #region Rain

        [HttpGet("rain/station")]
        public async Task<ActionResult<List<RainStationDto>>> GetRainStation()
        {
            try
            {
                if (_cache.TryGetValue(RainStationCacheKey, out List<RainStationDto> cached)) return Ok(cached);
                var data = await _wraGovService.GetRainStationAsync();
                if (data == null || data.Count == 0) return NotFound(new { message = "目前無雨量站資料" });
                _cache.Set(RainStationCacheKey, data, MediumCache);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得雨量站資料發生錯誤");
                return StatusCode(500, new { message = "取得雨量站資料失敗", error = ex.Message });
            }
        }

        [HttpGet("rain/real-time-info")]
        public async Task<ActionResult<List<RainRealTimeInfoDto>>> GetRainRealTimeInfo()
        {
            try
            {
                if (_cache.TryGetValue(RainRealTimeInfoCacheKey, out List<RainRealTimeInfoDto> cached)) return Ok(cached);
                var data = await _wraGovService.GetRainRealTimeInfoAsync();
                if (data == null || data.Count == 0) return NotFound(new { message = "目前無雨量即時資訊" });
                _cache.Set(RainRealTimeInfoCacheKey, data, ShortCache);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得雨量即時資訊發生錯誤");
                return StatusCode(500, new { message = "取得雨量即時資訊失敗", error = ex.Message });
            }
        }

        [HttpGet("rain/warning")]
        public async Task<ActionResult<List<RainWarningDto>>> GetRainWarning()
        {
            try
            {
                if (_cache.TryGetValue(RainWarningCacheKey, out List<RainWarningDto> cached)) return Ok(cached);
                var data = await _wraGovService.GetRainWarningAsync();
                if (data == null || data.Count == 0) return NotFound(new { message = "目前無雨量警戒資料" });
                _cache.Set(RainWarningCacheKey, data, ShortCache);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得雨量警戒資料發生錯誤");
                return StatusCode(500, new { message = "取得雨量警戒資料失敗", error = ex.Message });
            }
        }

        [HttpGet("rain/affected-area")]
        public async Task<ActionResult<List<RainAffectedAreaDto>>> GetRainAffectedArea()
        {
            try
            {
                if (_cache.TryGetValue(RainAffectedAreaCacheKey, out List<RainAffectedAreaDto> cached)) return Ok(cached);
                var data = await _wraGovService.GetRainAffectedAreaAsync();
                if (data == null || data.Count == 0) return NotFound(new { message = "目前無雨量影響範圍資料" });
                _cache.Set(RainAffectedAreaCacheKey, data, ShortCache);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得雨量影響範圍資料發生錯誤");
                return StatusCode(500, new { message = "取得雨量影響範圍資料失敗", error = ex.Message });
            }
        }

        #endregion

        #region Reservoir

        [HttpGet("reservoir/station")]
        public async Task<ActionResult<List<ReservoirStationDto>>> GetReservoirStation()
        {
            try
            {
                if (_cache.TryGetValue(ReservoirStationCacheKey, out List<ReservoirStationDto> cached)) return Ok(cached);
                var data = await _wraGovService.GetReservoirStationAsync();
                if (data == null || data.Count == 0) return NotFound(new { message = "目前無水庫測站資料" });
                _cache.Set(ReservoirStationCacheKey, data, LongCache);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得水庫測站資料發生錯誤");
                return StatusCode(500, new { message = "取得水庫測站資料失敗", error = ex.Message });
            }
        }

        [HttpGet("reservoir/real-time-info")]
        public async Task<ActionResult<List<ReservoirRealTimeInfoDto>>> GetReservoirRealTimeInfo()
        {
            try
            {
                if (_cache.TryGetValue(ReservoirRealTimeInfoCacheKey, out List<ReservoirRealTimeInfoDto> cached)) return Ok(cached);
                var data = await _wraGovService.GetReservoirRealTimeInfoAsync();
                if (data == null || data.Count == 0) return NotFound(new { message = "目前無水庫即時資訊" });
                _cache.Set(ReservoirRealTimeInfoCacheKey, data, ShortCache);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得水庫即時資訊發生錯誤");
                return StatusCode(500, new { message = "取得水庫即時資訊失敗", error = ex.Message });
            }
        }

        [HttpGet("reservoir/daily")]
        public async Task<ActionResult<List<ReservoirDailyDto>>> GetReservoirDaily()
        {
            try
            {
                if (_cache.TryGetValue(ReservoirDailyCacheKey, out List<ReservoirDailyDto> cached)) return Ok(cached);
                var data = await _wraGovService.GetReservoirDailyAsync();
                if (data == null || data.Count == 0) return NotFound(new { message = "目前無水庫每日資料" });
                _cache.Set(ReservoirDailyCacheKey, data, MediumCache);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得水庫每日資料發生錯誤");
                return StatusCode(500, new { message = "取得水庫每日資料失敗", error = ex.Message });
            }
        }

        [HttpGet("reservoir/warning")]
        public async Task<ActionResult<List<ReservoirWarningDto>>> GetReservoirWarning()
        {
            try
            {
                if (_cache.TryGetValue(ReservoirWarningCacheKey, out List<ReservoirWarningDto> cached)) return Ok(cached);
                var data = await _wraGovService.GetReservoirWarningAsync();
                if (data == null || data.Count == 0) return NotFound(new { message = "目前無水庫警戒資料" });
                _cache.Set(ReservoirWarningCacheKey, data, ShortCache);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得水庫警戒資料發生錯誤");
                return StatusCode(500, new { message = "取得水庫警戒資料失敗", error = ex.Message });
            }
        }

        [HttpGet("reservoir/affected-area")]
        public async Task<ActionResult<List<ReservoirAffectedAreaDto>>> GetReservoirAffectedArea()
        {
            try
            {
                if (_cache.TryGetValue(ReservoirAffectedAreaCacheKey, out List<ReservoirAffectedAreaDto> cached)) return Ok(cached);
                var data = await _wraGovService.GetReservoirAffectedAreaAsync();
                if (data == null || data.Count == 0) return NotFound(new { message = "目前無水庫影響範圍資料" });
                _cache.Set(ReservoirAffectedAreaCacheKey, data, ShortCache);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得水庫影響範圍資料發生錯誤");
                return StatusCode(500, new { message = "取得水庫影響範圍資料失敗", error = ex.Message });
            }
        }

        #endregion

        #region Statistics

        [HttpGet("statistics/flooding/{eventNo}")]
        public async Task<ActionResult<List<DisasterFloodingStatisticsDto>>> GetStatisticsFlooding(string eventNo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(eventNo))
                    return BadRequest(new { message = "事件編號不可空白" });
                var cacheKey = $"{StatFloodingCacheKeyPrefix}{eventNo}";
                if (_cache.TryGetValue(cacheKey, out List<DisasterFloodingStatisticsDto> cached)) return Ok(cached);
                var data = await _wraGovService.GetStatisticsFloodingAsync(eventNo);
                if (data == null || data.Count == 0) return NotFound(new { message = $"事件 {eventNo} 無淹水統計資料" });
                _cache.Set(cacheKey, data, MediumCache);
                return Ok(data);
            }
            catch (ArgumentException argEx)
            {
                return BadRequest(new { message = argEx.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"取得淹水統計資料發生錯誤 [{eventNo}]");
                return StatusCode(500, new { message = "取得淹水統計資料失敗", error = ex.Message });
            }
        }

        [HttpGet("statistics/water-facility/{eventNo}")]
        public async Task<ActionResult<List<DisasterWaterFacilityStatisticsDto>>> GetStatisticsWaterFacility(string eventNo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(eventNo))
                    return BadRequest(new { message = "事件編號不可空白" });
                var cacheKey = $"{StatWaterFacilityCacheKeyPrefix}{eventNo}";
                if (_cache.TryGetValue(cacheKey, out List<DisasterWaterFacilityStatisticsDto> cached)) return Ok(cached);
                var data = await _wraGovService.GetStatisticsWaterFacilityAsync(eventNo);
                if (data == null || data.Count == 0) return NotFound(new { message = $"事件 {eventNo} 無水利設施統計資料" });
                _cache.Set(cacheKey, data, MediumCache);
                return Ok(data);
            }
            catch (ArgumentException argEx)
            {
                return BadRequest(new { message = argEx.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"取得水利設施統計資料發生錯誤 [{eventNo}]");
                return StatusCode(500, new { message = "取得水利設施統計資料失敗", error = ex.Message });
            }
        }

        [HttpGet("statistics/flood-defense-material")]
        public async Task<ActionResult<List<FloodDefenseOperatorDto>>> GetStatisticsFloodDefenseMaterial()
        {
            try
            {
                if (_cache.TryGetValue(StatFloodDefenseMaterialCacheKey, out List<FloodDefenseOperatorDto> cached)) return Ok(cached);
                var data = await _wraGovService.GetStatisticsFloodDefenseMaterialAsync();
                if (data == null || data.Count == 0) return NotFound(new { message = "目前無防汛資材統計資料" });
                _cache.Set(StatFloodDefenseMaterialCacheKey, data, MediumCache);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得防汛資材統計資料發生錯誤");
                return StatusCode(500, new { message = "取得防汛資材統計資料失敗", error = ex.Message });
            }
        }

        #endregion

        #region Reservoir (Legacy - Supply Situation)

        [HttpGet("statistics")]
        public async Task<ActionResult<List<ReservoirDataDto>>> GetReservoirStatistics()
        {
            try
            {
                if (_cache.TryGetValue(ReservoirDataCacheKey, out List<ReservoirDataDto> cached)) return Ok(cached);
                var data = await _wraGovService.GetReservoirDataAsync();
                _cache.Set(ReservoirDataCacheKey, data, ShortCache);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得水庫即時統計資料發生錯誤");
                return StatusCode(500, new { message = "取得水庫即時統計資料失敗", error = ex.Message });
            }
        }

        [HttpGet("statistics/{name}")]
        public async Task<ActionResult<ReservoirDataDto>> GetReservoirByName(string name)
        {
            try
            {
                if (_cache.TryGetValue(ReservoirDataCacheKey, out List<ReservoirDataDto> allCached))
                {
                    var matched = allCached.Find(r => r.ReservoirName == name);
                    if (matched != null) return Ok(matched);
                    return NotFound(new { message = $"找不到水庫: {name}" });
                }
                var data = await _wraGovService.GetReservoirDataAsync();
                _cache.Set(ReservoirDataCacheKey, data, ShortCache);
                var result = data.Find(r => r.ReservoirName == name);
                if (result != null) return Ok(result);
                return NotFound(new { message = $"找不到水庫: {name}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"取得水庫資料發生錯誤 [{name}]");
                return StatusCode(500, new { message = "取得水庫資料失敗", error = ex.Message });
            }
        }

        [HttpGet("operation")]
        public async Task<ActionResult<List<ReservoirOperationDto>>> GetReservoirOperation()
        {
            try
            {
                if (_cache.TryGetValue(OperationCacheKey, out List<ReservoirOperationDto> cached)) return Ok(cached);
                var data = await _wraGovService.GetReservoirOperationAsync();
                _cache.Set(OperationCacheKey, data, ShortCache);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得水庫操作資料發生錯誤");
                return StatusCode(500, new { message = "取得水庫操作資料失敗", error = ex.Message });
            }
        }

        [HttpGet("overflow-alarm")]
        public async Task<ActionResult<List<OverflowAlarmDto>>> GetOverflowAlarm()
        {
            try
            {
                if (_cache.TryGetValue(AlarmCacheKey, out List<OverflowAlarmDto> cached)) return Ok(cached);
                var data = await _wraGovService.GetOverflowAlarmAsync();
                _cache.Set(AlarmCacheKey, data, ShortCache);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得溢流警報資料發生錯誤");
                return StatusCode(500, new { message = "取得溢流警報資料失敗", error = ex.Message });
            }
        }

        [HttpGet("supply-condition")]
        public async Task<ActionResult<List<WaterSupplyConditionDto>>> GetWaterSupplyCondition()
        {
            try
            {
                if (_cache.TryGetValue(SupplyCacheKey, out List<WaterSupplyConditionDto> cached)) return Ok(cached);
                var data = await _wraGovService.GetWaterSupplyConditionAsync();
                _cache.Set(SupplyCacheKey, data, ShortCache);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得供水情勢資料發生錯誤");
                return StatusCode(500, new { message = "取得供水情勢資料失敗", error = ex.Message });
            }
        }

        #endregion

        #region Water

        [HttpGet("water/station")]
        public async Task<ActionResult<List<WaterStationDto>>> GetWaterStation()
        {
            try
            {
                if (_cache.TryGetValue(WaterStationCacheKey, out List<WaterStationDto> cached)) return Ok(cached);
                var data = await _wraGovService.GetWaterStationAsync();
                if (data == null || data.Count == 0) return NotFound(new { message = "目前無水位站資料" });
                _cache.Set(WaterStationCacheKey, data, MediumCache);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得水位站資料發生錯誤");
                return StatusCode(500, new { message = "取得水位站資料失敗", error = ex.Message });
            }
        }

        [HttpGet("water/real-time-info")]
        public async Task<ActionResult<List<WaterRealTimeInfoDto>>> GetWaterRealTimeInfo()
        {
            try
            {
                if (_cache.TryGetValue(WaterRealTimeInfoCacheKey, out List<WaterRealTimeInfoDto> cached)) return Ok(cached);
                var data = await _wraGovService.GetWaterRealTimeInfoAsync();
                if (data == null || data.Count == 0) return NotFound(new { message = "目前無水位即時資訊" });
                _cache.Set(WaterRealTimeInfoCacheKey, data, ShortCache);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得水位即時資訊發生錯誤");
                return StatusCode(500, new { message = "取得水位即時資訊失敗", error = ex.Message });
            }
        }

        [HttpGet("water/warning")]
        public async Task<ActionResult<List<WaterWarningDto>>> GetWaterWarning()
        {
            try
            {
                if (_cache.TryGetValue(WaterWarningCacheKey, out List<WaterWarningDto> cached)) return Ok(cached);
                var data = await _wraGovService.GetWaterWarningAsync();
                if (data == null || data.Count == 0) return NotFound(new { message = "目前無水位警戒資料" });
                _cache.Set(WaterWarningCacheKey, data, ShortCache);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得水位警戒資料發生錯誤");
                return StatusCode(500, new { message = "取得水位警戒資料失敗", error = ex.Message });
            }
        }

        [HttpGet("water-level")]
        public async Task<ActionResult<List<WaterLevelDataDto>>> GetWaterLevel([FromQuery] string stationNo = null)
        {
            try
            {
                var cacheKey = $"{WaterLevelCacheKey}_{stationNo ?? "all"}";
                if (_cache.TryGetValue(cacheKey, out List<WaterLevelDataDto> cached)) return Ok(cached);
                var data = await _wraGovService.GetWaterLevelRealTimeAsync(stationNo);
                _cache.Set(cacheKey, data, ShortCache);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得水位資料發生錯誤");
                return StatusCode(500, new { message = "取得水位資料失敗", error = ex.Message });
            }
        }

        [HttpGet("rainfall")]
        public async Task<ActionResult<List<RainfallDataDto>>> GetRainfall([FromQuery] string stationNo = null)
        {
            try
            {
                var cacheKey = $"{RainfallCacheKey}_{stationNo ?? "all"}";
                if (_cache.TryGetValue(cacheKey, out List<RainfallDataDto> cached)) return Ok(cached);
                var data = await _wraGovService.GetRainfallRealTimeAsync(stationNo);
                _cache.Set(cacheKey, data, ShortCache);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得降雨量資料發生錯誤");
                return StatusCode(500, new { message = "取得降雨量資料失敗", error = ex.Message });
            }
        }

        #endregion

        /// <summary>
        /// 取得前端顯示用的水庫清單（只顯示有設定門檻的水庫）
        /// </summary>
        /// <remarks>
        /// 此 API 會從 FA_WR_ReservoirAlert 表讀取已設定警示門檻的水庫清單，
        /// 並結合水利署 API 提供完整的地理位置、流域、蓄水量等資訊。
        /// </remarks>
        [HttpGet("reservoir/display-list")]
        public async Task<ActionResult<List<ReservoirDisplayDto>>> GetReservoirDisplayList()
        {
            try
            {
                var data = await _wrRecorderService.GetDisplayReservoirsAsync();
                
                return Ok(new
                {
                    count = data?.Count ?? 0,
                    data = data ?? new List<ReservoirDisplayDto>(),
                    timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得水庫顯示清單發生錯誤");
                return StatusCode(500, new { message = "取得水庫顯示清單失敗", error = ex.Message });
            }
        }
    }
}