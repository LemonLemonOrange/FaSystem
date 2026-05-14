using System.Threading.Tasks;
using fa_api.Dtos.Ncdr;

namespace fa_api.Services.Ncdr
{
    /// <summary>
    /// NCDR 枯旱預警服務介面
    /// </summary>
    public interface INcdrDroughtService
    {
        /// <summary>
        /// 從 NCDR 警示訂閱頻道取得枯旱預警 CAP .cap 檔案內容
        /// 並解析成完整結構化資料（含 severity 與影響區域）
        /// </summary>
        /// <returns>最新一筆枯旱預警，若無警示則回傳 null</returns>
        Task<DroughtAlertDto> FetchDroughtAlertAsync();
    }
}