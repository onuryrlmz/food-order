import apiClient from './client';

export const orderService = {
  placeOrder: (data) =>
    apiClient.post('/customer/order/place', data),

  getActiveOrders: () =>
    apiClient.get('/customer/order/active'),

  getHistory: (page = 1, pageSize = 20) =>
    apiClient.get('/customer/order/history', {params: {page, pageSize}}),

  getOrderById: (orderId) =>
    apiClient.get(`/customer/order/${orderId}`),

  cancelOrder: (orderId, reason = '') =>
    apiClient.post(`/customer/order/${orderId}/cancel`, null, {params: {reason}}),

  initiatePayment: (orderId, paymentData) =>
    apiClient.post(`/customer/order/${orderId}/payment/initiate`, paymentData),

  getCourierLocation: (orderId) =>
    apiClient.get(`/v1/customer/order/${orderId}/courier-location`),
};
