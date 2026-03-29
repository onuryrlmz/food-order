import { z } from 'zod';

export const ValidateCouponRequestSchema = z.object({
  code: z.string().min(1),
  restaurantId: z.string().uuid(),
  orderAmount: z.number(),
  items: z.array(z.object({
    menuId: z.string().uuid(),
    quantity: z.number().int().min(1),
  })).optional(),
});

export const AvailableCouponsQuerySchema = z.object({
  restaurantId: z.string().uuid(),
  orderAmount: z.number().optional(),
});
