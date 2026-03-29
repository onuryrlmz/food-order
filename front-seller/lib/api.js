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

// --- Finance ---
export const getSellerFinanceSummary = () => api.seller.finance.getSummary();
export const getSellerPayments = (page = 1) => api.seller.finance.getPayments({ page });

// --- Password Reset ---
export const forgotPassword = (emailOrPhone) => api.auth.forgotPassword({ emailOrPhone });
export const verifyResetCode = (emailOrPhone, code) => api.auth.verifyResetCode({ emailOrPhone, code });
export const resetPassword = (emailOrPhone, code, newPassword) => api.auth.resetPassword({ emailOrPhone, code, newPassword });

// --- Analytics ---
export const getSellerAnalytics = (restaurantId, period = 'month') => {
  const { startDate, endDate } = periodToDates(period);
  return api.seller.analytics.getSummary({ restaurantId, startDate, endDate });
};
export const getSellerOrderTrends = (restaurantId, period = 'month') => {
  const { startDate, endDate } = periodToDates(period);
  return api.seller.analytics.getTrends({ restaurantId, startDate, endDate });
};
export const getSellerTopProducts = (restaurantId, period = 'month', limit = 10) => {
  const { startDate, endDate } = periodToDates(period);
  return api.seller.analytics.getTopProducts({ restaurantId, startDate, endDate, limit });
};

// --- Image Upload ---
export const uploadProductImage = (productId, file) => api.seller.product.uploadImage(productId, file);
export const deleteProductImage = (imageId) => api.seller.product.deleteImage({ imageId });

// --- Commission ---
export const getMyCommissions = () => api.seller.commission.getMy();

// --- Settlement ---
export const getMySettlementPeriods = (page = 1) => api.seller.commission.getSettlementPeriods({ page });
export const getMySettlementPeriodDetail = (periodId) => api.seller.commission.getSettlementPeriodDetail({ periodId });
