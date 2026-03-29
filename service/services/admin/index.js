import BaseService from '../base.js';
import {
  SellerIdSchema, UpdateCommissionRateSchema,
  AdminAnalyticsQuerySchema, AdminTopRestaurantsQuerySchema,
  OverdueOrdersQuerySchema,
  AdminReviewsQuerySchema, AdminReviewIdSchema,
  AdminTicketsQuerySchema, AdminTicketIdSchema, AdminActionIdSchema,
  AdminRestaurantIdSchema, SetRestaurantCommissionSchema,
  AdminSettlementPeriodIdSchema, AdminSettlementPeriodsQuerySchema, PaySettlementSchema, CancelSettlementSchema,
  ApproveCourierSchema, CreatePlatformScheduleSchema,
} from '../../schema/admin/index.js';

class AdminFinanceService extends BaseService {
  getSummary() { return this.get('/admin/finance/summary'); }
  getSellerFinance(data) {
    const { sellerId } = SellerIdSchema.parse(data);
    return this.get(`/admin/finance/sellers/${sellerId}`);
  }
  updateCommissionRate(data) {
    const parsed = UpdateCommissionRateSchema.parse(data);
    return this.put('/admin/settings/commission-rate', parsed);
  }
  retryIyzico(data) {
    const { sellerId } = SellerIdSchema.parse(data);
    return this.post(`/admin/seller/${sellerId}/retry-iyzico`);
  }
}

class AdminAnalyticsService extends BaseService {
  getSummary(data) {
    const parsed = AdminAnalyticsQuerySchema.parse(data);
    return this.get('/admin/analytics/summary', parsed);
  }
  getTrends(data) {
    const parsed = AdminAnalyticsQuerySchema.parse(data);
    return this.get('/admin/analytics/trends', parsed);
  }
  getTopRestaurants(data) {
    const parsed = AdminTopRestaurantsQuerySchema.parse(data);
    return this.get('/admin/analytics/top-restaurants', parsed);
  }
}

class AdminOrderService extends BaseService {
  getOverdue(data) {
    const parsed = OverdueOrdersQuerySchema.parse(data || {});
    return this.get('/admin/order/overdue', parsed);
  }
}

class AdminReviewService extends BaseService {
  getList(data) {
    const parsed = AdminReviewsQuerySchema.parse(data || {});
    return this.get('/admin/reviews', parsed);
  }
  remove(data) {
    const { reviewId } = AdminReviewIdSchema.parse(data);
    return this.delete(`/admin/reviews/${reviewId}`);
  }
}

class AdminSupportService extends BaseService {
  getStats() { return this.get('/admin/support/stats'); }
  getTickets(data) {
    const parsed = AdminTicketsQuerySchema.parse(data || {});
    return this.get('/admin/support/tickets', parsed);
  }
  getEscalated(data) {
    const parsed = OverdueOrdersQuerySchema.parse(data || {});
    return this.get('/admin/support/escalated', parsed);
  }
  getTicketDetail(data) {
    const { ticketId } = AdminTicketIdSchema.parse(data);
    return this.get(`/admin/support/ticket/${ticketId}`);
  }
  approveAction(data) {
    const { actionId } = AdminActionIdSchema.parse(data);
    return this.post(`/admin/support/action/${actionId}/approve`);
  }
  rejectAction(data) {
    const { actionId } = AdminActionIdSchema.parse(data);
    return this.post(`/admin/support/action/${actionId}/reject`);
  }
}

class AdminCommissionService extends BaseService {
  getSettings() { return this.get('/admin/commission/settings'); }
  getActiveSchedule() { return this.get('/admin/commission/settings/active'); }
  createSchedule(data) {
    const parsed = CreatePlatformScheduleSchema.parse(data);
    return this.post('/admin/commission/settings', parsed);
  }
  getRestaurantCommission(data) {
    const { restaurantId } = AdminRestaurantIdSchema.parse(data);
    return this.get(`/admin/commission/restaurant/${restaurantId}`);
  }
  getRestaurantHistory(data) {
    const { restaurantId } = AdminRestaurantIdSchema.parse(data);
    return this.get(`/admin/commission/restaurant/${restaurantId}/history`);
  }
  setRestaurantCommission(data) {
    const { restaurantId, ...body } = SetRestaurantCommissionSchema.parse(data);
    return this.put(`/admin/commission/restaurant/${restaurantId}`, body);
  }
}

class AdminSettlementService extends BaseService {
  getPeriods(data) {
    const parsed = AdminSettlementPeriodsQuerySchema.parse(data || {});
    return this.get('/admin/settlement/periods', parsed);
  }
  getPeriodDetail(data) {
    const { periodId } = AdminSettlementPeriodIdSchema.parse(data);
    return this.get(`/admin/settlement/period/${periodId}`);
  }
  approve(data) {
    const { periodId } = AdminSettlementPeriodIdSchema.parse(data);
    return this.post(`/admin/settlement/period/${periodId}/approve`);
  }
  pay(data) {
    const { periodId, ...body } = PaySettlementSchema.parse(data);
    return this.post(`/admin/settlement/period/${periodId}/pay`, body);
  }
  cancel(data) {
    const { periodId, reason } = CancelSettlementSchema.parse(data);
    return this.post(`/admin/settlement/period/${periodId}/cancel`, null, { reason });
  }
}

class AdminCourierService extends BaseService {
  approve(data) {
    const { courierId } = ApproveCourierSchema.parse(data);
    return this.put(`/admin/courier/couriers/${courierId}/approve`);
  }
}

export default class AdminServices {
  constructor(client) {
    this.finance = new AdminFinanceService(client);
    this.analytics = new AdminAnalyticsService(client);
    this.order = new AdminOrderService(client);
    this.review = new AdminReviewService(client);
    this.support = new AdminSupportService(client);
    this.commission = new AdminCommissionService(client);
    this.settlement = new AdminSettlementService(client);
    this.courier = new AdminCourierService(client);
  }
}
