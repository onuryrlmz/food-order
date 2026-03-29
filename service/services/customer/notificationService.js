import BaseService from '../base.js';
import { NotificationListQuerySchema, NotificationIdSchema, UpdateNotificationPreferencesSchema } from '../../schema/customer/notification.js';

export default class NotificationService extends BaseService {
  getList(data) {
    const parsed = NotificationListQuerySchema.parse(data || {});
    return this.get('/customer/notification', parsed);
  }

  getUnreadCount() {
    return this.get('/customer/notification/unread-count');
  }

  markAsRead(data) {
    const { notificationId } = NotificationIdSchema.parse(data);
    return this.put(`/customer/notification/${notificationId}/read`);
  }

  markAllAsRead() {
    return this.put('/customer/notification/read-all');
  }

  getPreferences() {
    return this.get('/customer/notification/preferences');
  }

  updatePreferences(data) {
    const parsed = UpdateNotificationPreferencesSchema.parse(data);
    return this.put('/customer/notification/preferences', parsed);
  }
}
