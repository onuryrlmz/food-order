import BaseService from '../base.js';
import { PlaceOrderRequestSchema, OrderIdSchema, CancelOrderRequestSchema, InitiatePaymentRequestSchema, OrderHistoryQuerySchema } from '../../schema/customer/order.js';

export default class OrderService extends BaseService {
  place(data) {
    const parsed = PlaceOrderRequestSchema.parse(data);
    return this.post('/customer/order/place', parsed);
  }

  getActive() {
    return this.get('/customer/order/active');
  }

  getHistory(data) {
    const parsed = OrderHistoryQuerySchema.parse(data || {});
    return this.get('/customer/order/history', parsed);
  }

  getById(data) {
    const { orderId } = OrderIdSchema.parse(data);
    return this.get(`/customer/order/${orderId}`);
  }

  cancel(data) {
    const { orderId, reason } = CancelOrderRequestSchema.parse(data);
    return this.post(`/customer/order/${orderId}/cancel`, { reason });
  }

  initiatePayment(data) {
    const { orderId, ...body } = InitiatePaymentRequestSchema.parse(data);
    return this.post(`/customer/order/${orderId}/payment/initiate`, body);
  }

  reorder(data) {
    const { orderId } = OrderIdSchema.parse(data);
    return this.post(`/customer/order/${orderId}/reorder`);
  }

  getTracking(data) {
    const { orderId } = OrderIdSchema.parse(data);
    return this.get(`/customer/order/${orderId}/tracking`);
  }
}
