import apiClient from './client';

export const couponService = {
  validateCoupon: (code, restaurantId, orderAmount, items) =>
    apiClient.post('/customer/coupon/validate', {
      code,
      restaurantId,
      orderAmount,
      items,
    }),

  getAvailableCoupons: (restaurantId, orderAmount) =>
    apiClient.get(`/customer/coupon/available/${restaurantId}`, {
      params: { orderAmount },
    }),
};
