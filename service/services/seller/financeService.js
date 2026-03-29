import BaseService from '../base.js';
import { PaymentsQuerySchema } from '../../schema/seller/finance.js';

export default class SellerFinanceService extends BaseService {
  getSummary() {
    return this.get('/seller/finance/summary');
  }

  getPayments(data) {
    const parsed = PaymentsQuerySchema.parse(data || {});
    return this.get('/seller/finance/payments', parsed);
  }
}
