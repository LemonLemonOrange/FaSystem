# -*- coding: utf-8 -*-
import sys
import json
from ultralytics import YOLO
import os

def main():
    if len(sys.argv) < 2:
        print(json.dumps({"error": "Missing image path parameter"}, ensure_ascii=False), file=sys.stderr)
        sys.exit(1)

    image_path = sys.argv[1]

    # Load YOLO model
    script_dir = os.path.dirname(os.path.abspath(__file__))
    model_path = os.path.join(script_dir, "yolo26n.pt")
    
    # Check if model file exists
    if not os.path.exists(model_path):
        print(json.dumps({"error": f"Model file not found: {model_path}"}, ensure_ascii=False), file=sys.stderr)
        sys.exit(1)

    # Check if image file exists
    if not os.path.exists(image_path):
        print(json.dumps({"error": f"Image file not found: {image_path}"}, ensure_ascii=False), file=sys.stderr)
        sys.exit(1)

    try:
        # Load model
        model = YOLO(model_path)
        
        # Run inference (verbose=False to avoid extra output)
        results = model.predict(source=image_path, verbose=False, save=False)
        
        # Parse results
        detections = []
        
        for result in results:
            # Get bounding box information
            boxes = result.boxes
            
            if boxes is not None and len(boxes) > 0:
                for box in boxes:
                    # Get coordinates (xyxy format: x1, y1, x2, y2)
                    xyxy = box.xyxy[0].cpu().numpy()
                    x1, y1, x2, y2 = float(xyxy[0]), float(xyxy[1]), float(xyxy[2]), float(xyxy[3])
                    
                    # Get confidence score
                    confidence = float(box.conf[0].cpu().numpy())
                    
                    # Get class index and name
                    cls_id = int(box.cls[0].cpu().numpy())
                    label = result.names[cls_id]
                    
                    # Calculate width and height
                    width = int(x2 - x1)
                    height = int(y2 - y1)
                    
                    detections.append({
                        "label": label,
                        "confidence": round(confidence, 4),
                        "x": int(x1),
                        "y": int(y1),
                        "width": width,
                        "height": height
                    })
        
        # Output JSON to stdout
        print(json.dumps(detections, ensure_ascii=False))
    
    except Exception as e:
        print(json.dumps({"error": str(e)}, ensure_ascii=False), file=sys.stderr)
        sys.exit(1)

if __name__ == "__main__":
    main()