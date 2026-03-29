import BaseService from '../base.js';
import { CreateScheduledOrderRequestSchema, UpdateScheduledOrderRequestSchema, ScheduledOrderIdSchema, ScheduledOrderListQuerySchema } from '../../schema/customer/scheduledOrder.js';

export default class ScheduledOrderService extends BaseService {
  create(data) {
    const parsed = CreateScheduledOrderRequestSchema.parse(data);
    return this.post('/customer/scheduled-order', parsed);
  }

  getList(data) {
    const parsed = ScheduledOrderListQuerySchema.parse(data || {});
    return this.get('/customer/scheduled-order', parsed);
  }

  getById(data) {
    const { id } = ScheduledOrderIdSchema.parse(data);
    return this.get(`/customer/scheduled-order/${id}`);
  }

  update(data) {
    const { id, ...body } = UpdateScheduledOrderRequestSchema.parse(data);
    return this.put(`/customer/scheduled-order/${id}`, body);
  }

  cancel(data) {
    const { id } = ScheduledOrderIdSchema.parse(data);
    return this.post(`/customer/scheduled-order/${id}/cancel`);
  }
}
