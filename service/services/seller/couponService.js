import BaseService from '../base.js';
import { CreateSellerCouponRequestSchema, UpdateSellerCouponRequestSchema, DeleteSellerCouponSchema } from '../../schema/seller/coupon.js';

export default class SellerCouponService extends BaseService {
  create(data) {
    const parsed = CreateSellerCouponRequestSchema.parse(data);
    return this.post('/seller/coupon/create', parsed);
  }

  update(data) {
    const parsed = UpdateSellerCouponRequestSchema.parse(data);
    return this.put('/seller/coupon/update', parsed);
  }

  remove(data) {
    const { id } = DeleteSellerCouponSchema.parse(data);
    return this.delete(`/seller/coupon/${id}`);
  }
}
