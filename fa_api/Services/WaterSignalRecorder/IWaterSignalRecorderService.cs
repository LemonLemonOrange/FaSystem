using System.Threading.Tasks;

namespace fa_api.Services.WaterSignalRecorder
{
    public interface IWaterSignalRecorderService
    {
        /// <summary>
        /// 抓取供水狀況、轉換燈號並批次寫入 FA_WR_WaterSignalSnapshot
        /// </summary>
        Task RecordWaterSupplySignalsAsync();
    }
}
