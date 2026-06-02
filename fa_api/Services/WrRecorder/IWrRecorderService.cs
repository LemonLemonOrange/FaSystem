using fa_api.Dtos.WraGov;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace fa_api.Services.WrRecorder
{
    /// <summary>
    /// 水庫服務
    /// </summary>
    public interface IWrRecorderService
    {
        /// <summary>
        /// 取得前端顯示用的水庫清單（只顯示有設定門檻的水庫）
        /// </summary>
        Task<List<ReservoirDisplayDto>> GetDisplayReservoirsAsync();
    }
}