import apiClient from './client';

export const restaurantService = {
  getRestaurants: (addressId) =>
    apiClient.get('/customer/restaurant', {params: {addressId}}),

  getRestaurantInfo: (id) =>
    apiClient.get(`/customer/restaurant/${id}`),
};
