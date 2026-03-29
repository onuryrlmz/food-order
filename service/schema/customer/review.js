import { z } from 'zod';
import { PaginationSchema } from '../base.js';

export const CreateReviewRequestSchema = z.object({
  orderId: z.string().uuid(),
  rating: z.number().int().min(1).max(5),
  comment: z.string().optional(),
});

export const GetReviewsQuerySchema = PaginationSchema.extend({
  restaurantId: z.string().uuid(),
});

export const DeleteReviewSchema = z.object({
  reviewId: z.string().uuid(),
});
