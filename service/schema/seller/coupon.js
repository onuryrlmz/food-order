import { z } from 'zod';

export const CreateSellerCouponRequestSchema = z.object({
  code: z.string().min(1),
  discountType: z.number().int(),
  discountValue: z.number().min(0),
  minimumOrderAmount: z.number().min(0).optional(),
  maxUsageCount: z.number().int().optional(),
  startDate: z.string().optional(),
  endDate: z.string().optional(),
  restaurantId: z.string().uuid(),
  menuIds: z.array(z.string().uuid()).optional(),
  categoryIds: z.array(z.string().uuid()).optional(),
});

export const UpdateSellerCouponRequestSchema = CreateSellerCouponRequestSchema.extend({
  id: z.string().uuid(),
});

export const DeleteSellerCouponSchema = z.object({
  id: z.string().uuid(),
});
