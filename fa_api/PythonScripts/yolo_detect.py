import sys
import json
from ultralytics import YOLO
import os

def main():
    if len(sys.argv) < 2:
        print(json.dumps({"error": "缺少影像路徑參數"}), file=sys.stderr)
        sys.exit(1)

    image_path = sys.argv[1]

    # 載入 YOLO 模型
    script_dir = os.path.dirname(os.path.abspath(__file__))
    model_path = os.path.join(script_dir, "yolo26n.pt")
    
    # 檢查模型檔案是否存在
    if not os.path.exists(model_path):
        print(json.dumps({"error": f"模型檔案不存在: {model_path}"}), file=sys.stderr)
        sys.exit(1)

    try:
        model = YOLO(model_path)
        
        # 執行推論 (verbose=False 避免額外輸出干擾 JSON)
        results = model(image_path, verbose=False)
        
        # 解析結果
        detections = []
        for result in results:
            boxes = result.boxes
            if boxes is not None:
                for box in boxes:
                    x1, y1, x2, y2 = box.xyxy[0].tolist()
                    detections.append({
                        "label": result.names[int(box.cls[0])],
                        "confidence": float(box.conf[0]),
                        "x": int(x1),
                        "y": int(y1),
                        "width": int(x2 - x1),
                        "height": int(y2 - y1)
                    })
        
        # 輸出 JSON 到 stdout
        print(json.dumps(detections))
    
    except Exception as e:
        print(json.dumps({"error": str(e)}), file=sys.stderr)
        sys.exit(1)

if __name__ == "__main__":
    main()