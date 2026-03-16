import apiClient from './client';

export const favoriteService = {
  addFavorite: (restaurantId) =>
    apiClient.post('/customer/favorite', {restaurantId}),

  removeFavorite: (restaurantId) =>
    apiClient.delete(`/customer/favorite/${restaurantId}`),

  getFavorites: (page = 1, pageSize = 20) =>
    apiClient.get('/customer/favorite', {params: {page, pageSize}}),
};
