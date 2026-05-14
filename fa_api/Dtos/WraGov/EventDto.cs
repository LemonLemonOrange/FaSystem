namespace fa_api.Dtos.WraGov
{
    /// <summary>
    /// 事件資料
    /// </summary>
    public class EventDto
    {
        /// <summary>
        /// 事件編號
        /// </summary>
        public string EventNo { get; set; }

        /// <summary>
        /// 事件名稱
        /// </summary>
        public string EventName { get; set; }

        /// <summary>
        /// 開始時間
        /// </summary>
        public string BeginTime { get; set; }

        /// <summary>
        /// 結束時間
        /// </summary>
        public string EndTime { get; set; }

        /// <summary>
        /// 是否進行中（1:是, 0:否）
        /// </summary>
        public int IsActive { get; set; }
    }
}