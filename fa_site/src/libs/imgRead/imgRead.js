import axios from 'axios';
import { useMutation } from 'react-query';

/**
 * @typedef {Object} BoundingBox
 * @property {number} x      - 左上角 x 座標（像素整數）
 * @property {number} y      - 左上角 y 座標（像素整數）
 * @property {number} width  - 寬度（像素整數）
 * @property {number} height - 高度（像素整數）
 */

/**
 * @typedef {Object} DetectionResult
 * @property {string} label      - 類別標籤
 * @property {number} confidence - 信心分數（0~1 之間的浮點數）
 * @property {BoundingBox} bbox  - 邊界框座標
 */

/**
 * @typedef {Object} YoloResponseDto
 * @property {DetectionResult[]} detections - 偵測結果陣列（可為空陣列）
 * @property {number} elapsedMs             - 執行時間（毫秒）
 */

/**
 * 呼叫 YOLO 物件偵測 API
 * @param {File} file - 要偵測的影像檔案
 * @returns {Promise<YoloResponseDto>}
 */
const detectImage = async (file) => {
  const formData = new FormData();
  formData.append('image', file);

  const baseURL = process.env.REACT_APP_API_BASE_URL || 'http://localhost:65326';
  const response = await axios.post(
    `${baseURL}/api/yolo/detect`,
    formData,
    { headers: { 'Content-Type': 'multipart/form-data' } }
  );
  return response.data;
};

/**
 * useDetectImage — 封裝 YOLO API 呼叫的 React Query mutation hook
 *
 * @returns {{ mutate: Function, isLoading: boolean, data: YoloResponseDto|undefined, error: import('axios').AxiosError|null, reset: Function }}
 */
export const useDetectImage = () => useMutation(detectImage);
