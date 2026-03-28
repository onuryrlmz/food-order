import apiClient from './client';

export const restaurantService = {
  getRestaurants: (addressId) =>
    apiClient.get('/customer/restaurant', {params: {addressId}}),

  getRestaurantsByLocation: (latitude, longitude) =>
    apiClient.get('/customer/restaurant', {params: {latitude, longitude}}),

  getRestaurantInfo: (id) =>
    apiClient.get(`/customer/restaurant/${id}`),
};
