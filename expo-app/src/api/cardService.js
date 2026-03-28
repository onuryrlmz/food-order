import apiClient from './client';

export const cardService = {
  getCards: () => apiClient.get('/customer/card/list'),

  createCard: (cardData) => apiClient.post('/customer/card/create', cardData),

  deleteCard: (cardToken) => apiClient.delete(`/customer/card/${cardToken}`),
};
