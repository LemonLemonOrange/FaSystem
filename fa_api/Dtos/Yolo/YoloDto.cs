using System.Collections.Generic;

namespace fa_api.Dtos.Yolo
{
    /// <summary>單一物件偵測結果</summary>
    public class DetectionResultDto
    {
        /// <summary>物件類別名稱（例：person、car）</summary>
        public string Label { get; set; }

        /// <summary>信心分數（0.0 ~ 1.0）</summary>
        public decimal Confidence { get; set; }

        /// <summary>邊界框左上角 X 座標（像素）</summary>
        public int X { get; set; }

        /// <summary>邊界框左上角 Y 座標（像素）</summary>
        public int Y { get; set; }

        /// <summary>邊界框寬度（像素）</summary>
        public int Width { get; set; }

        /// <summary>邊界框高度（像素）</summary>
        public int Height { get; set; }
    }

    /// <summary>YOLO 影像辨識完整回應</summary>
    public class YoloResponseDto
    {
        /// <summary>偵測到的物件清單（可為空陣列）</summary>
        public List<DetectionResultDto> Detections { get; set; }

        /// <summary>本次辨識執行時間（毫秒）</summary>
        public int ElapsedMs { get; set; }
    }
}
