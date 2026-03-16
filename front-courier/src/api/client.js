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

// Response interceptor
apiClient.interceptors.response.use(
  response => response,
  async error => {
    if (error.response?.status === 401) {
      await AsyncStorage.removeItem('courier_token');
      await AsyncStorage.removeItem('courier_info');
    }
    return Promise.reject(error);
  },
);

export default apiClient;
