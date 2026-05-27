namespace fa_api.Services.Yolo
{
    public class YoloSettings
    {
        /// <summary>Python 執行檔路徑（例：python 或 /usr/bin/python3）</summary>
        public string PythonPath { get; set; }

        /// <summary>YOLO Python 腳本路徑</summary>
        public string ScriptPath { get; set; }

        /// <summary>腳本執行逾時秒數（預設 30）</summary>
        public int TimeoutSeconds { get; set; } = 30;
    }
}
