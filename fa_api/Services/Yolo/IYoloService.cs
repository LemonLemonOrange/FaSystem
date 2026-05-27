using System.Threading.Tasks;
using fa_api.Dtos.Yolo;

namespace fa_api.Services.Yolo
{
    /// <summary>
    /// YOLO 影像辨識服務介面。
    /// </summary>
    public interface IYoloService
    {
        /// <summary>
        /// 執行 YOLO 影像辨識，回傳偵測結果與執行時間。
        /// </summary>
        /// <param name="imageBytes">影像位元組陣列</param>
        /// <returns>YoloResponseDto 包含偵測結果清單與執行時間（毫秒）</returns>
        /// <exception cref="System.TimeoutException">Python 腳本執行逾時</exception>
        /// <exception cref="System.InvalidOperationException">腳本執行失敗或設定不完整</exception>
        Task<YoloResponseDto> DetectAsync(byte[] imageBytes);
    }
}
