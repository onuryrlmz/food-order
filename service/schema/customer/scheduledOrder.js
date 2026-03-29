import { z } from 'zod';
import { PaginationSchema } from '../base.js';

export const CreateScheduledOrderRequestSchema = z.object({
  restaurantId: z.string().uuid(),
  addressId: z.string().uuid(),
  scheduledDate: z.string(),
  scheduledTime: z.string(),
  paymentMethod: z.number().int(),
  items: z.array(z.object({
    menuId: z.string().uuid(),
    quantity: z.number().int().min(1),
    note: z.string().optional(),
    values: z.array(z.object({
      menuOptionId: z.string().uuid(),
      menuOptionValueId: z.string().uuid(),
    })).optional(),
  })),
  repeatType: z.string().optional(),
});

export const UpdateScheduledOrderRequestSchema = CreateScheduledOrderRequestSchema.extend({
  id: z.string().uuid(),
});

export const ScheduledOrderIdSchema = z.object({
  id: z.string().uuid(),
});

export const ScheduledOrderListQuerySchema = PaginationSchema;
