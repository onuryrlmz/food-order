import axios from 'axios';
import AsyncStorage from '@react-native-async-storage/async-storage';
import {API_BASE_URL} from '../utils/constants';

const apiClient = axios.create({
  baseURL: API_BASE_URL,
  timeout: 15000,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Request interceptor — inject Bearer token
apiClient.interceptors.request.use(
  async config => {
    const token = await AsyncStorage.getItem('auth_token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  error => Promise.reject(error),
);

// Response interceptor — persist tokens from login/register responses & handle 401
apiClient.interceptors.response.use(
  response => {
    // If the response contains a token (login / register), persist it
    const token = response.data?.token;
    if (token) {
      AsyncStorage.setItem('auth_token', token);
    }
    if (response.data?.refreshToken) {
      AsyncStorage.setItem('refresh_token', response.data.refreshToken);
    }
    return response;
  },
  async error => {
    if (error.response?.status === 401) {
      // Clear stored credentials — the app's auth state listener should
      // detect this and navigate to the login screen.
      await AsyncStorage.multiRemove([
        'auth_token',
        'refresh_token',
        'user_info',
      ]);
    }
    return Promise.reject(error);
  },
);

export default apiClient;
