import apiClient from './client';

export const basketService = {
  getBasket: () =>
    apiClient.get('/customer/basket'),

  updateBasket: (data) =>
    apiClient.put('/customer/basket', data),

  clearBasket: () =>
    apiClient.delete('/customer/basket'),
};
