import api, { client } from './service';

// Default export: raw axios client (backward-compatible with existing page imports)
export default client;

// --- Helper ---
function periodToDates(period) {
  const end = new Date();
  const start = new Date();
  if (period === 'week') start.setDate(end.getDate() - 7);
  else if (period === 'year') start.setFullYear(end.getFullYear() - 1);
  else start.setMonth(end.getMonth() - 1);
  return { startDate: start.toISOString().split('T')[0], endDate: end.toISOString().split('T')[0] };
}

// --- iyzico ---
export const retryIyzicoRegistration = (sellerId) => api.admin.finance.retryIyzico({ sellerId });

// --- Finance ---
export const getFinanceSummary = () => api.admin.finance.getSummary();
export const getSellerFinance = (sellerId) => api.admin.finance.getSellerFinance({ sellerId });
export const updateCommissionRate = (rate) => api.admin.finance.updateCommissionRate({ rate });

// --- Password Reset ---
export const forgotPassword = (emailOrPhone) => api.auth.forgotPassword({ emailOrPhone });
export const verifyResetCode = (emailOrPhone, code) => api.auth.verifyResetCode({ emailOrPhone, code });
export const resetPassword = (emailOrPhone, code, newPassword) => api.auth.resetPassword({ emailOrPhone, code, newPassword });

// --- Analytics ---
export const getAdminAnalytics = (period = 'month') => {
  const { startDate, endDate } = periodToDates(period);
  return api.admin.analytics.getSummary({ startDate, endDate });
};
export const getAdminOrderTrends = (period = 'month') => {
  const { startDate, endDate } = periodToDates(period);
  return api.admin.analytics.getTrends({ startDate, endDate });
};
export const getAdminTopRestaurants = (period = 'month', limit = 10) => {
  const { startDate, endDate } = periodToDates(period);
  return api.admin.analytics.getTopRestaurants({ startDate, endDate, limit });
};

// --- Overdue Orders ---
export const getOverdueOrders = (page = 1) => api.admin.order.getOverdue({ page });

// --- Reviews ---
export const getReviews = (page = 1, status = '') => api.admin.review.getList({ page, status: status || undefined });
export const deleteReview = (reviewId) => api.admin.review.remove({ reviewId });

// --- Support ---
export const getSupportStats = () => api.admin.support.getStats();
export const getAllTickets = (page = 1, statusId = '', topicId = '') =>
  api.admin.support.getTickets({ page, statusId: statusId ? Number(statusId) : undefined, topicId: topicId ? Number(topicId) : undefined });
export const getEscalatedTickets = (page = 1) => api.admin.support.getEscalated({ page });
export const getTicketDetail = (ticketId) => api.admin.support.getTicketDetail({ ticketId });
export const approveAction = (actionId) => api.admin.support.approveAction({ actionId });
export const rejectAction = (actionId) => api.admin.support.rejectAction({ actionId });

// --- Commission ---
export const getPlatformSchedules = () => api.admin.commission.getSettings();
export const getActivePlatformSchedule = () => api.admin.commission.getActiveSchedule();
export const createPlatformSchedule = (data) => api.admin.commission.createSchedule(data);
export const getRestaurantCommission = (restaurantId) => api.admin.commission.getRestaurantCommission({ restaurantId });
export const getRestaurantCommissionHistory = (restaurantId) => api.admin.commission.getRestaurantHistory({ restaurantId });
export const setRestaurantCommission = (restaurantId, data) => api.admin.commission.setRestaurantCommission({ restaurantId, ...data });

// --- Settlement ---
export const getSettlementPeriods = (page = 1, statusId = '') =>
  api.admin.settlement.getPeriods({ page, statusId: statusId ? Number(statusId) : undefined });
export const getSettlementPeriodDetail = (periodId) => api.admin.settlement.getPeriodDetail({ periodId });
export const approveSettlement = (periodId) => api.admin.settlement.approve({ periodId });
export const paySettlement = (periodId, data) => api.admin.settlement.pay({ periodId, ...data });
export const cancelSettlement = (periodId, reason) => api.admin.settlement.cancel({ periodId, reason });
