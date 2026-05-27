using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using fa_api.Dtos.Yolo;
using fa_api.Services.Yolo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace fa_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "Yolo")]
    [Produces("application/json")]
    public class YoloController : ControllerBase
    {
        private readonly ILogger<YoloController> _logger;
        private readonly IYoloService _yoloService;

        public YoloController(ILogger<YoloController> logger, IYoloService yoloService)
        {
            _logger = logger;
            _yoloService = yoloService;
        }

        /// <summary>
        /// 上傳影像並執行 YOLO 物件偵測
        /// </summary>
        /// <param name="image">影像檔案（JPEG、PNG、BMP，最大 10 MB）</param>
        /// <returns>偵測結果清單與執行時間</returns>
        [HttpPost("detect")]
        public async Task<ActionResult<YoloResponseDto>> Detect([FromForm] IFormFile image)
        {
            // 1. 空檔案驗證（Requirement 1.5）
            if (image == null || image.Length == 0)
                return BadRequest(new { message = "影像檔案不得為空" });

            // 2. MIME 類型驗證（Requirement 1.2）
            var allowedMimeTypes = new HashSet<string> { "image/jpeg", "image/png", "image/bmp" };
            if (!allowedMimeTypes.Contains(image.ContentType))
                return BadRequest(new { message = "不支援的檔案格式，僅接受 JPEG、PNG、BMP" });

            // 3. 檔案大小驗證（Requirement 1.3）
            if (image.Length > 10 * 1024 * 1024)
                return BadRequest(new { message = "檔案大小超過限制（最大 10 MB）" });

            try
            {
                // 讀取影像為 byte[]（Requirement 1.4）
                byte[] imageBytes;
                using (var ms = new System.IO.MemoryStream())
                {
                    await image.CopyToAsync(ms);
                    imageBytes = ms.ToArray();
                }

                // 呼叫 YOLO 服務
                var result = await _yoloService.DetectAsync(imageBytes);
                return Ok(result);
            }
            catch (TimeoutException)
            {
                // HTTP 504（Requirement 5.2）
                return StatusCode(504, new { message = "影像辨識逾時，請稍後再試" });
            }
            catch (Exception ex)
            {
                // HTTP 500（Requirement 5.1）
                _logger.LogError(ex, "YOLO 影像辨識發生錯誤");
                return StatusCode(500, new { message = "影像辨識失敗", error = ex.Message });
            }
        }
    }
}
