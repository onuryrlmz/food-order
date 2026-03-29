import BaseService from '../base.js';
import { AnalyticsQuerySchema, TopProductsQuerySchema } from '../../schema/seller/analytics.js';

export default class SellerAnalyticsService extends BaseService {
  getSummary(data) {
    const { restaurantId, ...params } = AnalyticsQuerySchema.parse(data);
    return this.get(`/seller/analytics/summary/${restaurantId}`, params);
  }

  getTrends(data) {
    const { restaurantId, ...params } = AnalyticsQuerySchema.parse(data);
    return this.get(`/seller/analytics/trends/${restaurantId}`, params);
  }

  getTopProducts(data) {
    const { restaurantId, ...params } = TopProductsQuerySchema.parse(data);
    return this.get(`/seller/analytics/top-products/${restaurantId}`, params);
  }
}
