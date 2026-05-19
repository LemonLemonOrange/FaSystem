import axios from 'axios';
import createAuthRefreshInterceptor from 'axios-auth-refresh';

// å»ºç? Axios å¯¦ä?
const API_BASE_URL = process.env.REACT_APP_API_BASE_URL || 'http://localhost:65326';
const apiClient = axios.create({
  baseURL: API_BASE_URL,
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Request ?”æˆª??(?¯åœ¨æ­¤å???JWT Token)
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

// Response ?”æˆª??(çµ±ä??•ç??¯èª¤)
apiClient.interceptors.response.use(
  (response) => {
    return response.data;
  },
  (error) => {
    if (error.response) {
      // ?‰æ”¶?°ä¼º?å™¨?æ?ï¼Œä??€?‹ç¢¼??2xx
      switch (error.response.status) {
        case 400:
          console.error('API Error (400): è«‹æ??ƒæ•¸?¯èª¤', error.response.data);
          break;
        case 401:
          console.error('API Error (401): å°šæœªé©—è???Token æº–å??·æ–°', error.response.data);
          // ?™è£¡ axios-auth-refresh ?ƒæ¥?‹è??†åˆ·?°ï?å¦‚æ??·æ–°å¤±æ??æ??‹å‡º?¯èª¤
          break;
        case 403:
          console.error('API Error (403): ?’ç?å­˜å?ï¼Œæ??ä?è¶?, error.response.data);
          break;
        case 404:
          console.error('API Error (404): ?¾ä??°è?æ±‚ç?è³‡æ?', error.response.data);
          break;
        case 500:
          console.error('API Error (500): ä¼ºæ??¨å…§?¨éŒ¯èª?, error.response.data);
          break;
        default:
          console.error(`API Error (${error.response.status}):`, error.response.data);
      }
    } else if (error.request) {
      // è«‹æ?å·²ç™¼?ºï?ä½†æ??¶åˆ°?æ? (å¦‚ç¶²?·ä??ä¼º?å™¨?›ä?)
      console.error('API Error: ä¼ºæ??¨ç„¡?æ??–ç¶²è·¯ç•°å¸?, error.request);
    } else {
      // ?¶ä?å»ºæ?è«‹æ??‚ç™¼?Ÿç??¯èª¤
      console.error('API Error: è«‹æ??¼ç??ªçŸ¥?¯èª¤', error.message);
    }
    
    return Promise.reject(error);
  }
);

// ?·æ–° Token ?„é?è¼?const refreshAuthLogic = async (failedRequest) => {
  try {
    // ?™è£¡?šå¸¸?ƒç™¼?è?æ±‚å»?–å??°ç? tokenï¼Œè??¿æ??å¯¦?›ç? API URL ?‡é?è¼?    // const refreshToken = localStorage.getItem('refreshToken');
    // const tokenRefreshResponse = await axios.post('http://localhost:5299/api/auth/refresh', { refreshToken });
    
    // å°‡æ–° token å­˜å…¥ localStorage (?–é©?¶ç??°æ–¹)
    // localStorage.setItem('token', tokenRefreshResponse.data.token);
    
    // ?´æ–°?Ÿè?æ±‚ç? Authorization header
    // failedRequest.response.config.headers['Authorization'] = 'Bearer ' + tokenRefreshResponse.data.token;
    
    return Promise.resolve();
  } catch (error) {
    // Refresh token ä¹Ÿå¤±?ˆæ?ï¼Œé€šå¸¸?ƒé?è¦ç™»?ºä¸¦å°å??»å…¥??    // localStorage.removeItem('token');
    // localStorage.removeItem('refreshToken');
    // window.location.href = '/login';
    return Promise.reject(error);
  }
};

// ç¶å? axios-auth-refresh ?”æˆª??// ?è¨­?ƒåœ¨ response ?€?‹ç¢¼??401 ?‚è§¸??refreshAuthLogic
createAuthRefreshInterceptor(apiClient, refreshAuthLogic);

export default apiClient;
