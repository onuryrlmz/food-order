import { z } from 'zod';

export const UpdateBasketRequestSchema = z.object({
  restaurantId: z.string().uuid(),
  items: z.array(z.object({
    menuId: z.string().uuid(),
    quantity: z.number().int().min(1),
    note: z.string().optional(),
    values: z.array(z.object({
      menuOptionId: z.string().uuid(),
      menuOptionValueId: z.string().uuid(),
    })).optional(),
  })),
});
