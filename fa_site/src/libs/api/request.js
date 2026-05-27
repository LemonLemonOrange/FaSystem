import axios from "axios";

export const baseURL = window.env?.REACT_APP_API_BASE_URL || "http://localhost:65326";

/** 共用 axios instance */
export const apiClient = axios.create({
  baseURL,
  timeout: 15000,
  headers: {
    "Content-Type": "application/json",
    Accept: "application/json",
  },
});

// 1. Request interceptor — 預留 auth token 注入點
apiClient.interceptors.request.use(
  (config) => {
    // const token = localStorage.getItem('token');
    // if (token) config.headers.Authorization = `Bearer ${token}`;
    return config;
  },
  (error) => Promise.reject(error)
);

// 2. Response interceptor — 自動 unwrap data，集中處理錯誤
apiClient.interceptors.response.use(
  (response) => response.data,
  (error) => {
    const status = error.response?.status;
    const message = error.response?.data?.message || error.message;

    if (status === 401) {
      console.warn("[apiClient] 401 未授權");
      // 可在此 redirect 至登入頁
    } else if (status === 403) {
      console.warn("[apiClient] 403 禁止存取");
    } else if (status >= 500) {
      console.error(`[apiClient] 伺服器錯誤 ${status}:`, message);
    } else if (!error.response) {
      console.error("[apiClient] 網路錯誤或請求逾時:", message);
    }

    return Promise.reject(error);
  }
);

export default apiClient;