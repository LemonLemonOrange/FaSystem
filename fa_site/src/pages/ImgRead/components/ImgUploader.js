import React, { useEffect, useState } from "react";
import { Upload, message } from "antd";
import PropTypes from "prop-types";

// 驗證常數
const ALLOWED_MIME_TYPES = ["image/jpeg", "image/png", "image/bmp"];
const MAX_FILE_SIZE = 10 * 1024 * 1024; // 10 MB

/**
 * 驗證 MIME type 是否為允許的影像格式
 * @param {string} mimeType
 * @returns {boolean}
 */
export const isValidMimeType = (mimeType) => ALLOWED_MIME_TYPES.includes(mimeType);

/**
 * 驗證檔案大小是否在允許範圍內（<= 10 MB）
 * @param {number} sizeInBytes
 * @returns {boolean}
 */
export const isValidFileSize = (sizeInBytes) => sizeInBytes <= MAX_FILE_SIZE;

/**
 * 格式化信心分數為百分比字串
 * @param {number} confidence - 0 到 1 之間的浮點數
 * @returns {string} 例如 "85.42%"
 */
export const formatConfidence = (confidence) => `${(confidence * 100).toFixed(2)}%`;

/**
 * 格式化辨識耗時為顯示字串
 * @param {number} elapsedMs - 毫秒數
 * @returns {string} 例如 "辨識耗時：123 ms"
 */
export const formatElapsedMs = (elapsedMs) => `辨識耗時：${elapsedMs} ms`;

/**
 * ImgUploader 元件
 * 負責影像選擇、驗證與預覽，使用 Ant Design Upload.Dragger
 */
const ImgUploader = ({ onFileSelect, onFileRemove, disabled, detections }) => {
  const [previewUrl, setPreviewUrl] = useState(null);
  const [annotatedUrl, setAnnotatedUrl] = useState(null);

  const beforeUpload = (file) => {
    if (!isValidMimeType(file.type)) {
      message.warning("僅支援 JPEG、PNG、BMP 格式");
      setPreviewUrl(null);
      onFileRemove();
      return false;
    }

    if (!isValidFileSize(file.size)) {
      message.warning("檔案大小不得超過 10 MB");
      setPreviewUrl(null);
      onFileRemove();
      return false;
    }

    const url = URL.createObjectURL(file);
    setPreviewUrl(url);
    setAnnotatedUrl(null);
    onFileSelect(file);
    return false;
  };

  useEffect(() => {
    return () => {
      if (previewUrl) {
        URL.revokeObjectURL(previewUrl);
      }
    };
  }, [previewUrl]);

  useEffect(() => {
    const renderAnnotatedImage = async () => {
      if (!previewUrl || !Array.isArray(detections) || detections.length === 0) {
        setAnnotatedUrl(null);
        return;
      }

      const image = new Image();
      image.src = previewUrl;

      await new Promise((resolve, reject) => {
        image.onload = resolve;
        image.onerror = reject;
      });

      const canvas = document.createElement("canvas");
      canvas.width = image.naturalWidth;
      canvas.height = image.naturalHeight;

      const context = canvas.getContext("2d");
      if (!context) {
        setAnnotatedUrl(null);
        return;
      }

      context.drawImage(image, 0, 0, canvas.width, canvas.height);
      context.lineWidth = Math.max(2, Math.round(canvas.width / 300));
      context.font = `${Math.max(14, Math.round(canvas.width / 45))}px sans-serif`;

      detections.forEach((item) => {
        const x = item.x;
        const y = item.y;
        const width = item.width;
        const height = item.height;

        context.strokeStyle = "#ff4d4f";
        context.fillStyle = "rgba(255, 77, 79, 0.12)";
        context.strokeRect(x, y, width, height);
        context.fillRect(x, y, width, height);

        const labelText = `${item.label} ${formatConfidence(item.confidence)}`;
        const textWidth = context.measureText(labelText).width;
        const textHeight = Math.max(18, Math.round(canvas.width / 45));
        const textY = Math.max(0, y - textHeight - 4);

        context.fillStyle = "#ff4d4f";
        context.fillRect(x, textY, textWidth + 12, textHeight + 4);
        context.fillStyle = "#ffffff";
        context.fillText(labelText, x + 6, textY + textHeight);
      });

      setAnnotatedUrl(canvas.toDataURL("image/png"));
    };

    renderAnnotatedImage().catch(() => setAnnotatedUrl(null));
  }, [previewUrl, detections]);

  return (
    <div>
      <Upload.Dragger
        multiple={false}
        beforeUpload={beforeUpload}
        customRequest={() => {}}
        showUploadList={false}
        disabled={disabled}
      >
        <p className="ant-upload-drag-icon" />
        <p className="ant-upload-text">點擊或拖曳影像至此區域上傳</p>
        <p className="ant-upload-hint">支援 JPEG、PNG、BMP 格式，檔案大小不得超過 10 MB</p>
      </Upload.Dragger>
      {previewUrl && (
        <div style={{ marginTop: 8 }}>
          <div style={{ marginBottom: 12 }}>
            <div style={{ marginBottom: 6, fontWeight: 500 }}>原始圖片</div>
            <img
              src={previewUrl}
              alt="預覽"
              style={{
                display: "block",
                minWidth: 100,
                minHeight: 100,
                objectFit: "contain",
                maxWidth: "100%",
                width: "100%",
                height: "auto",
              }}
            />
          </div>
          <div>
            <div style={{ marginBottom: 6, fontWeight: 500 }}>辨識結果圖片</div>
            {annotatedUrl ? (
              <img
                src={annotatedUrl}
                alt="辨識結果"
                style={{
                  display: "block",
                  minWidth: 100,
                  minHeight: 100,
                  objectFit: "contain",
                  maxWidth: "100%",
                  width: "100%",
                  height: "auto",
                }}
              />
            ) : (
              <div
                style={{
                  padding: 16,
                  border: "1px dashed #d9d9d9",
                  color: "#8c8c8c",
                  borderRadius: 6,
                }}
              >
                送出辨識後會在這裡顯示加上框線的結果圖
              </div>
            )}
          </div>
        </div>
      )}
    </div>
  );
};

ImgUploader.propTypes = {
  onFileSelect: PropTypes.func.isRequired,
  onFileRemove: PropTypes.func,
  disabled: PropTypes.bool,
  detections: PropTypes.array,
};

ImgUploader.defaultProps = {
  onFileRemove: () => {},
  disabled: false,
};

export default ImgUploader;
