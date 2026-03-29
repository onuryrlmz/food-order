import { z } from 'zod';

export const AnalyticsQuerySchema = z.object({
  restaurantId: z.string().uuid(),
  startDate: z.string(),
  endDate: z.string(),
});

export const TopProductsQuerySchema = AnalyticsQuerySchema.extend({
  limit: z.number().int().optional().default(10),
});
