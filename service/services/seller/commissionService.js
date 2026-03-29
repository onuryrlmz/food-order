import BaseService from '../base.js';
import { SettlementPeriodsQuerySchema, SettlementPeriodIdSchema } from '../../schema/seller/commission.js';

export default class SellerCommissionService extends BaseService {
  getMy() {
    return this.get('/seller/commission/my');
  }

  getSettlementPeriods(data) {
    const parsed = SettlementPeriodsQuerySchema.parse(data || {});
    return this.get('/seller/commission/settlement/periods', parsed);
  }

  getSettlementPeriodDetail(data) {
    const { periodId } = SettlementPeriodIdSchema.parse(data);
    return this.get(`/seller/commission/settlement/period/${periodId}`);
  }
}
