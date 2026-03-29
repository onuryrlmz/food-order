import BaseService from '../base.js';
import { UpdateOrderStatusRequestSchema } from '../../schema/seller/order.js';

export default class SellerOrderService extends BaseService {
  updateStatus(data) {
    const { orderId, statusId } = UpdateOrderStatusRequestSchema.parse(data);
    return this.put(`/order/${orderId}/status`, { statusId });
  }
}
