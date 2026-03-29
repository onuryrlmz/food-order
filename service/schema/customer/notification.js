import { z } from 'zod';
import { PaginationSchema } from '../base.js';

export const NotificationListQuerySchema = PaginationSchema;

export const NotificationIdSchema = z.object({
  notificationId: z.string().uuid(),
});

export const UpdateNotificationPreferencesSchema = z.object({
  orderUpdates: z.boolean().optional(),
  promotions: z.boolean().optional(),
  reviewReplies: z.boolean().optional(),
  deliveryUpdates: z.boolean().optional(),
});
