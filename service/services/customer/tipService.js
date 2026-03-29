import BaseService from '../base.js';
import { CreateTipRequestSchema, TipOrderIdSchema } from '../../schema/customer/tip.js';

export default class TipService extends BaseService {
  create(data) {
    const parsed = CreateTipRequestSchema.parse(data);
    return this.post('/customer/tip', parsed);
  }

  getOptions(data) {
    const { orderId } = TipOrderIdSchema.parse(data);
    return this.get(`/customer/tip/options/${orderId}`);
  }

  getByOrder(data) {
    const { orderId } = TipOrderIdSchema.parse(data);
    return this.get(`/customer/tip/order/${orderId}`);
  }
}
