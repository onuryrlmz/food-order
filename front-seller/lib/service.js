import axios from 'axios';
import Service from '../../service/services/index.js';

const BASE_URL = process.env.NEXT_PUBLIC_API_BASE_URL || 'http://localhost:3762';

// --- Interceptor setup (shared logic for both clients) ---

function attachInterceptors(instance) {
  let isRefreshing = false;
  let failedQueue = [];

  const processQueue = (error) => {
    failedQueue.forEach((prom) => {
      if (error) prom.reject(error);
      else prom.resolve();
    });
    failedQueue = [];
  };

  instance.interceptors.response.use(
    (res) => {
      const body = res.data;
      if (body && body.hasFailed === true) {
        const msg = body.messages?.[0]?.description || 'Bir hata olustu';
        const error = new Error(msg);
        error.response = res;
        error.isBusinessError = true;
        return Promise.reject(error);
      }
      return res;
    },
    async (err) => {
      const originalRequest = err.config;

      if (err.response?.status === 401 && !originalRequest._retry) {
        if (isRefreshing) {
          return new Promise((resolve, reject) => {
            failedQueue.push({ resolve, reject });
          }).then(() => instance(originalRequest));
        }

        originalRequest._retry = true;
        isRefreshing = true;

        try {
          await instance.post('/v1/auth/refresh');
          processQueue(null);
          return instance(originalRequest);
        } catch (refreshError) {
          processQueue(refreshError);
          if (typeof window !== 'undefined') {
            localStorage.removeItem('seller_logged_in');
            window.location.href = '/login';
          }
          return Promise.reject(refreshError);
        } finally {
          isRefreshing = false;
        }
      }

      return Promise.reject(err);
    },
  );
}

// --- Raw axios client (baseURL without /v1) for SWR fetcher and direct URL calls ---
const client = axios.create({
  baseURL: BASE_URL,
  headers: { 'Content-Type': 'application/json' },
  withCredentials: true,
});
attachInterceptors(client);

// --- Service client (baseURL with /v1) for centralized service methods ---
const serviceClient = axios.create({
  baseURL: `${BASE_URL}/v1`,
  headers: { 'Content-Type': 'application/json' },
  withCredentials: true,
});
attachInterceptors(serviceClient);

// Service instance using centralized service classes
const api = new Service(serviceClient);

export default api;
export { client };
