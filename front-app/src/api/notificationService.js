import apiClient from './client';

export const notificationService = {
  getNotifications: (page = 1, pageSize = 20) =>
    apiClient.get('/customer/notification', {params: {page, pageSize}}),

  getUnreadCount: () =>
    apiClient.get('/customer/notification/unread-count'),

  markAsRead: (notificationId) =>
    apiClient.put(`/customer/notification/${notificationId}/read`),

  markAllAsRead: () =>
    apiClient.put('/customer/notification/read-all'),

  getPreferences: () =>
    apiClient.get('/customer/notification/preferences'),

  updatePreference: (data) =>
    apiClient.put('/customer/notification/preferences', data),
};
