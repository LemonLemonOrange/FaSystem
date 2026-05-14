import axios from 'axios';
import createAuthRefreshInterceptor from 'axios-auth-refresh';

// 建立 Axios 實例
const API_BASE_URL = process.env.REACT_APP_API_BASE_URL || 'http://localhost:65326';
const apiClient = axios.create({
  baseURL: API_BASE_URL,
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Request 攔截器 (可在此加入 JWT Token)
apiClient.interceptors.request.use(
  (config) => {
    // const token = localStorage.getItem('token');
    // if (token) {
    //   config.headers.Authorization = `Bearer ${token}`;
    // }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Response 攔截器 (統一處理錯誤)
apiClient.interceptors.response.use(
  (response) => {
    return response.data;
  },
  (error) => {
    if (error.response) {
      // 有收到伺服器回應，但狀態碼非 2xx
      switch (error.response.status) {
        case 400:
          console.error('API Error (400): 請求參數錯誤', error.response.data);
          break;
        case 401:
          console.error('API Error (401): 尚未驗證或 Token 準備刷新', error.response.data);
          // 這裡 axios-auth-refresh 會接手處理刷新，如果刷新失敗才會拋出錯誤
          break;
        case 403:
          console.error('API Error (403): 拒絕存取，權限不足', error.response.data);
          break;
        case 404:
          console.error('API Error (404): 找不到請求的資源', error.response.data);
          break;
        case 500:
          console.error('API Error (500): 伺服器內部錯誤', error.response.data);
          break;
        default:
          console.error(`API Error (${error.response.status}):`, error.response.data);
      }
    } else if (error.request) {
      // 請求已發出，但沒收到回應 (如網斷了、伺服器掛了)
      console.error('API Error: 伺服器無回應或網路異常', error.request);
    } else {
      // 其他建構請求時發生的錯誤
      console.error('API Error: 請求發生未知錯誤', error.message);
    }
    
    return Promise.reject(error);
  }
);

// 刷新 Token 的邏輯
const refreshAuthLogic = async (failedRequest) => {
  try {
    // 這裡通常會發送請求去取得新的 token，請替換成實際的 API URL 與邏輯
    // const refreshToken = localStorage.getItem('refreshToken');
    // const tokenRefreshResponse = await axios.post('http://localhost:5299/api/auth/refresh', { refreshToken });
    
    // 將新 token 存入 localStorage (或適當的地方)
    // localStorage.setItem('token', tokenRefreshResponse.data.token);
    
    // 更新原請求的 Authorization header
    // failedRequest.response.config.headers['Authorization'] = 'Bearer ' + tokenRefreshResponse.data.token;
    
    return Promise.resolve();
  } catch (error) {
    // Refresh token 也失效時，通常會需要登出並導向登入頁
    // localStorage.removeItem('token');
    // localStorage.removeItem('refreshToken');
    // window.location.href = '/login';
    return Promise.reject(error);
  }
};

// 綁定 axios-auth-refresh 攔截器
// 預設會在 response 狀態碼為 401 時觸發 refreshAuthLogic
createAuthRefreshInterceptor(apiClient, refreshAuthLogic);

export default apiClient;
