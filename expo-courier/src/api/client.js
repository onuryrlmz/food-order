import axios from 'axios';
import * as SecureStore from 'expo-secure-store';
import { API_BASE_URL } from '../utils/constants';

const apiClient = axios.create({
  baseURL: API_BASE_URL,
  timeout: 15000,
  headers: {
    'Content-Type': 'application/json',
  },
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
    if (error.response?.status === 401) {
      await SecureStore.deleteItemAsync('auth_token');
      await SecureStore.deleteItemAsync('refresh_token');
    }
    return Promise.reject(error);
  },
);

export default apiClient;
