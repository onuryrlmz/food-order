import axios from 'axios';
import AsyncStorage from '@react-native-async-storage/async-storage';
import {Platform} from 'react-native';

const API_HOST = Platform.select({
  android: '10.0.2.2',
  ios: 'localhost',
});

const API_BASE_URL = `http://${API_HOST}:3762`;

const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {'Content-Type': 'application/json'},
});

// Request interceptor - token ekle
apiClient.interceptors.request.use(
  async config => {
    const token = await AsyncStorage.getItem('courier_token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  error => Promise.reject(error),
);

let isRefreshing = false;
let failedQueue = [];

const processQueue = (error, token = null) => {
  failedQueue.forEach(prom => {
    if (error) {
      prom.reject(error);
    } else {
      prom.resolve(token);
    }
  });
  failedQueue = [];
};

// Response interceptor with refresh token support
apiClient.interceptors.response.use(
  response => response,
  async error => {
    const originalRequest = error.config;

    if (error.response?.status === 401 && !originalRequest._retry) {
      if (isRefreshing) {
        return new Promise((resolve, reject) => {
          failedQueue.push({resolve, reject});
        }).then(token => {
          originalRequest.headers.Authorization = `Bearer ${token}`;
          return apiClient(originalRequest);
        });
      }

      originalRequest._retry = true;
      isRefreshing = true;

      const refreshToken = await AsyncStorage.getItem('courier_refresh_token');
      if (refreshToken) {
        try {
          const res = await axios.post(`${API_BASE_URL}/v1/auth/refresh`, {refreshToken});
          const {accessToken, refreshToken: newRefresh} = res.data.data;
          await AsyncStorage.setItem('courier_token', accessToken);
          await AsyncStorage.setItem('courier_refresh_token', newRefresh);
          processQueue(null, accessToken);
          originalRequest.headers.Authorization = `Bearer ${accessToken}`;
          return apiClient(originalRequest);
        } catch (refreshError) {
          processQueue(refreshError, null);
          await AsyncStorage.multiRemove(['courier_token', 'courier_refresh_token', 'courier_info']);
          return Promise.reject(refreshError);
        } finally {
          isRefreshing = false;
        }
      } else {
        await AsyncStorage.multiRemove(['courier_token', 'courier_info']);
      }
    }

    return Promise.reject(error);
  },
);

export {API_BASE_URL};
export default apiClient;
