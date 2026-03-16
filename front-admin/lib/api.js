import axios from 'axios';

const BASE_URL = process.env.NEXT_PUBLIC_API_BASE_URL || 'http://localhost:3762';

const api = axios.create({
  baseURL: BASE_URL,
  headers: { 'Content-Type': 'application/json' },
  withCredentials: true,
});

let isRefreshing = false;
let failedQueue = [];

const processQueue = (error, token = null) => {
  failedQueue.forEach(prom => {
    if (error) prom.reject(error);
    else prom.resolve(token);
  });
  failedQueue = [];
};

api.interceptors.response.use(
  (res) => res,
  async (err) => {
    const originalRequest = err.config;

    if (err.response?.status === 401 && !originalRequest._retry) {
      if (isRefreshing) {
        return new Promise((resolve, reject) => {
          failedQueue.push({ resolve, reject });
        }).then(() => api(originalRequest));
      }

      originalRequest._retry = true;
      isRefreshing = true;

      try {
        await api.post('/v1/auth/refresh');
        processQueue(null);
        return api(originalRequest);
      } catch (refreshError) {
        processQueue(refreshError, null);
        if (typeof window !== 'undefined') {
          localStorage.removeItem('logged_in');
          window.location.href = '/login';
        }
        return Promise.reject(refreshError);
      } finally {
        isRefreshing = false;
      }
    }

    return Promise.reject(err);
  }
);

// Courier Companies
export const getCourierCompanies = (page = 1, size = 20) =>
  api.get(`/v1/admin/courier-companies?page=${page}&size=${size}`).then(r => r.data);

export const approveCourierCompany = (id) =>
  api.put(`/v1/admin/courier-companies/${id}/approve`).then(r => r.data);

export const rejectCourierCompany = (id) =>
  api.put(`/v1/admin/courier-companies/${id}/reject`).then(r => r.data);

export const suspendCourierCompany = (id) =>
  api.put(`/v1/admin/courier-companies/${id}/suspend`).then(r => r.data);

export const banCourierCompany = (id) =>
  api.put(`/v1/admin/courier-companies/${id}/ban`).then(r => r.data);

// iyzico
export const retryIyzicoRegistration = (sellerId) =>
  api.post(`/v1/admin/seller/${sellerId}/retry-iyzico`).then(r => r.data);

// Finance
export const getFinanceSummary = () =>
  api.get('/v1/admin/finance/summary').then(r => r.data);

export const getSellerFinance = (sellerId) =>
  api.get(`/v1/admin/finance/sellers/${sellerId}`).then(r => r.data);

export const updateCommissionRate = (rate) =>
  api.put('/v1/admin/settings/commission-rate', { rate }).then(r => r.data);

// Password Reset
export const forgotPassword = (emailOrPhone) =>
  api.post('/v1/auth/forgot-password', { emailOrPhone }).then(r => r.data);

export const verifyResetCode = (emailOrPhone, code) =>
  api.post('/v1/auth/verify-reset-code', { emailOrPhone, code }).then(r => r.data);

export const resetPassword = (emailOrPhone, code, newPassword) =>
  api.post('/v1/auth/reset-password', { emailOrPhone, code, newPassword }).then(r => r.data);

export default api;
