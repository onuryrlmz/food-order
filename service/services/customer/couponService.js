import BaseService from '../base.js';
import { ValidateCouponRequestSchema, AvailableCouponsQuerySchema } from '../../schema/customer/coupon.js';

export default class CouponService extends BaseService {
  validate(data) {
    const parsed = ValidateCouponRequestSchema.parse(data);
    return this.post('/customer/coupon/validate', parsed);
  }

  getAvailable(data) {
    const { restaurantId, orderAmount } = AvailableCouponsQuerySchema.parse(data);
    return this.get(`/customer/coupon/available/${restaurantId}`, { orderAmount });
  }
}
