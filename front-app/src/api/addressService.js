import apiClient from './client';

export const addressService = {
  getList: () =>
    apiClient.get('/customer/address'),

  add: (data) =>
    apiClient.post('/customer/address', data),

  update: (data) =>
    apiClient.put('/customer/address', data),

  deleteAddress: (id) =>
    apiClient.delete(`/customer/address/${id}`),

  setDefault: (id) =>
    apiClient.patch(`/customer/address/${id}/set-default`),
};
