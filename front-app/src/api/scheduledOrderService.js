import apiClient from './client';

export const scheduledOrderService = {
  create: (data) =>
    apiClient.post('/customer/scheduled-order', data),

  getList: (page = 1, pageSize = 20) =>
    apiClient.get('/customer/scheduled-order', {params: {page, pageSize}}),

  getDetail: (id) =>
    apiClient.get(`/customer/scheduled-order/${id}`),

  update: (id, data) =>
    apiClient.put(`/customer/scheduled-order/${id}`, data),

  cancel: (id, reason) =>
    apiClient.post(`/customer/scheduled-order/${id}/cancel`, null, {params: {reason}}),
};
