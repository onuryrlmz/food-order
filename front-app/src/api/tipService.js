import apiClient from './client';

export const tipService = {
  addTip: (data) =>
    apiClient.post('/customer/tip', data),

  getTipOptions: (orderId) =>
    apiClient.get(`/customer/tip/options/${orderId}`),

  getTipByOrder: (orderId) =>
    apiClient.get(`/customer/tip/order/${orderId}`),
};
