import { z } from 'zod';
import { PaginationSchema } from '../base.js';

export const ToggleFavoriteRequestSchema = z.object({
  restaurantId: z.string().uuid(),
});

export const FavoritesQuerySchema = PaginationSchema;
