import axios from 'axios';
import * as SecureStore from 'expo-secure-store';
import { API_BASE_URL } from '../utils/constants';

const apiClient = axios.create({
  baseURL: API_BASE_URL,
  timeout: 15000,
  headers: {
    'Content-Type': 'application/json',
  },
  withCredentials: true,
});

apiClient.interceptors.request.use(
  async config => {
    const token = await SecureStore.getItemAsync('auth_token');
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

apiClient.interceptors.response.use(
  async response => {
    const token = response.data?.token;
    if (token) {
      await SecureStore.setItemAsync('auth_token', token);
    }
    if (response.data?.refreshToken) {
      await SecureStore.setItemAsync('refresh_token', response.data.refreshToken);
    }
    return response;
  },
  async error => {
    const originalRequest = error.config;

    if (error.response?.status === 401 && !originalRequest._retry) {
      if (isRefreshing) {
        return new Promise((resolve, reject) => {
          failedQueue.push({ resolve, reject });
        }).then(token => {
          originalRequest.headers.Authorization = `Bearer ${token}`;
          return apiClient(originalRequest);
        });
      }

      originalRequest._retry = true;
      isRefreshing = true;

      const refreshToken = await SecureStore.getItemAsync('refresh_token');
      if (refreshToken) {
        try {
          const res = await axios.post(`${API_BASE_URL}/auth/refresh`, { refreshToken });
          const { accessToken, refreshToken: newRefresh } = res.data.data;
          await SecureStore.setItemAsync('auth_token', accessToken);
          await SecureStore.setItemAsync('refresh_token', newRefresh);
          processQueue(null, accessToken);
          originalRequest.headers.Authorization = `Bearer ${accessToken}`;
          return apiClient(originalRequest);
        } catch (refreshError) {
          processQueue(refreshError, null);
          await SecureStore.deleteItemAsync('auth_token');
          await SecureStore.deleteItemAsync('refresh_token');
          return Promise.reject(refreshError);
        } finally {
          isRefreshing = false;
        }
      }
    }

    return Promise.reject(error);
  },
);

export default apiClient;
