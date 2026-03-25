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
  (res) => {
    // Backend returns HTTP 200 with hasFailed=true for business logic errors
    const body = res.data;
    if (body && body.hasFailed === true) {
      const msg = body.messages?.[0]?.description || 'Bir hata oluştu';
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

// Analytics — period → startDate/endDate conversion
function periodToDates(period) {
  const end = new Date();
  const start = new Date();
  if (period === 'week') start.setDate(end.getDate() - 7);
  else if (period === 'year') start.setFullYear(end.getFullYear() - 1);
  else start.setMonth(end.getMonth() - 1);
  return { startDate: start.toISOString().split('T')[0], endDate: end.toISOString().split('T')[0] };
}

export const getAdminAnalytics = (period = 'month') => {
  const { startDate, endDate } = periodToDates(period);
  return api.get(`/v1/admin/analytics/summary?startDate=${startDate}&endDate=${endDate}`).then(r => r.data);
};

export const getAdminOrderTrends = (period = 'month') => {
  const { startDate, endDate } = periodToDates(period);
  return api.get(`/v1/admin/analytics/trends?startDate=${startDate}&endDate=${endDate}`).then(r => r.data);
};

export const getAdminTopRestaurants = (period = 'month', limit = 10) => {
  const { startDate, endDate } = periodToDates(period);
  return api.get(`/v1/admin/analytics/top-restaurants?startDate=${startDate}&endDate=${endDate}&limit=${limit}`).then(r => r.data);
};

// Overdue Orders
export const getOverdueOrders = (page = 1) =>
  api.get(`/v1/admin/order/overdue?page=${page}&pageSize=20`).then(r => r.data);

// Reviews
export const getReviews = (page = 1, status = '') =>
  api.get(`/v1/admin/reviews?page=${page}&pageSize=20${status ? `&status=${status}` : ''}`).then(r => r.data);

export const deleteReview = (reviewId) =>
  api.delete(`/v1/admin/reviews/${reviewId}`).then(r => r.data);

export default api;
