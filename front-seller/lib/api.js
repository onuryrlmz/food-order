import axios from 'axios';

const BASE_URL = process.env.NEXT_PUBLIC_API_BASE_URL || 'http://localhost:3762';

const api = axios.create({
  baseURL: BASE_URL,
  headers: { 'Content-Type': 'application/json' },
  withCredentials: true,
});

api.interceptors.response.use(
  (res) => res,
  (err) => {
    if (err.response?.status === 401) {
      if (typeof window !== 'undefined') window.location.href = '/login';
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

export default api;
