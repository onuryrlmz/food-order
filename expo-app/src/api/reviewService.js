import apiClient from './client';

export const reviewService = {
  createReview: (data) =>
    apiClient.post('/customer/review', data),

  getRestaurantReviews: (restaurantId, page = 1, pageSize = 10) =>
    apiClient.get(`/customer/review/restaurant/${restaurantId}`, {params: {page, pageSize}}),

  deleteReview: (reviewId) =>
    apiClient.delete(`/customer/review/${reviewId}`),
};
