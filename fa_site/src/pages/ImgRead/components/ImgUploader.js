import React, { useState } from "react";
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
const ImgUploader = ({ onFileSelect, onFileRemove, disabled }) => {
  const [previewUrl, setPreviewUrl] = useState(null);

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
    onFileSelect(file);
    return false;
  };

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
        <img
          src={previewUrl}
          alt="預覽"
          style={{
            minWidth: 100,
            minHeight: 100,
            objectFit: "contain",
            maxWidth: "100%",
            marginTop: 8,
          }}
        />
      )}
    </div>
  );
};

ImgUploader.propTypes = {
  onFileSelect: PropTypes.func.isRequired,
  onFileRemove: PropTypes.func,
  disabled: PropTypes.bool,
};

ImgUploader.defaultProps = {
  onFileRemove: () => {},
  disabled: false,
};

export default ImgUploader;
