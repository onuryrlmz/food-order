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
          localStorage.removeItem('seller_logged_in');
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

// Courier API functions
export const addCourier = async (restaurantId, email) => {
  const res = await api.post(`/v1/seller/restaurant/${restaurantId}/courier/add`, { email });
  return res.data;
};

export const getRestaurantCouriers = async (restaurantId) => {
  const res = await api.get(`/v1/seller/restaurant/${restaurantId}/couriers`);
  return res.data;
};

export const removeCourier = async (restaurantId, courierId) => {
  const res = await api.delete(`/v1/seller/restaurant/${restaurantId}/couriers/${courierId}`);
  return res.data;
};

export const assignCourierToOrder = async (orderId, courierId) => {
  const res = await api.put(`/v1/seller/order/${orderId}/assign-courier`, { courierId });
  return res.data;
};

export const getCourierLocation = async (orderId) => {
  const res = await api.get(`/v1/seller/order/${orderId}/courier-location`);
  return res.data;
};

export const getCourierOrderHistory = async (courierId, month, year) => {
  const res = await api.get(`/v1/seller/courier/${courierId}/orders?month=${month}&year=${year}`);
  return res.data;
};

// Subscription Usage
export const getSubscriptionUsage = (restaurantId) =>
  api.get(`/v1/subscription/usage?restaurantId=${restaurantId}`).then(r => r.data);

export const getUpgradePreview = (restaurantId, planId) =>
  api.get(`/v1/subscription/upgrade/preview?restaurantId=${restaurantId}&planId=${planId}`).then(r => r.data);

export const upgradeSubscription = (data) =>
  api.post('/v1/subscription/upgrade', data).then(r => r.data);

// Courier Companies
export const getRestaurantCourierCompanies = (restaurantId) =>
  api.get(`/v1/seller/restaurant/${restaurantId}/courier-companies`).then(r => r.data);

export const inviteCourierCompany = (restaurantId, companyId) =>
  api.post(`/v1/seller/restaurant/${restaurantId}/courier-company/add`, { companyId }).then(r => r.data);

export const removeCourierCompany = (restaurantId, companyId) =>
  api.delete(`/v1/seller/restaurant/${restaurantId}/courier-company/${companyId}`).then(r => r.data);

export const searchCourierCompanies = (query) =>
  api.get(`/v1/seller/courier-companies/search?q=${encodeURIComponent(query)}`).then(r => r.data);

// Finance
export const getSellerFinanceSummary = () =>
  api.get('/v1/seller/finance/summary').then(r => r.data);

export const getSellerPayments = (page = 1) =>
  api.get(`/v1/seller/finance/payments?page=${page}`).then(r => r.data);

export const toggleAutoRenew = (restaurantId, enabled) =>
  api.put('/v1/subscription/auto-renew', { restaurantId, enabled }).then(r => r.data);

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

export const getSellerAnalytics = (restaurantId, period = 'month') => {
  const { startDate, endDate } = periodToDates(period);
  return api.get(`/v1/seller/analytics/summary/${restaurantId}?startDate=${startDate}&endDate=${endDate}`).then(r => r.data);
};

export const getSellerOrderTrends = (restaurantId, period = 'month') => {
  const { startDate, endDate } = periodToDates(period);
  return api.get(`/v1/seller/analytics/trends/${restaurantId}?startDate=${startDate}&endDate=${endDate}`).then(r => r.data);
};

export const getSellerTopProducts = (restaurantId, period = 'month', limit = 10) => {
  const { startDate, endDate } = periodToDates(period);
  return api.get(`/v1/seller/analytics/top-products/${restaurantId}?startDate=${startDate}&endDate=${endDate}&limit=${limit}`).then(r => r.data);
};

// Image Upload
export const uploadProductImage = (productId, file) => {
  const formData = new FormData();
  formData.append('file', file);
  formData.append('productId', productId);
  return api.post('/v1/seller/product/image', formData, {
    headers: { 'Content-Type': 'multipart/form-data' },
  }).then(r => r.data);
};

export const deleteProductImage = (imageId) =>
  api.delete(`/v1/seller/product/image/${imageId}`).then(r => r.data);

export default api;
