import { z } from 'zod';

export const RecordSearchRequestSchema = z.object({
  query: z.string().min(1),
  restaurantId: z.string().uuid().optional(),
});

export const RecentSearchQuerySchema = z.object({
  limit: z.number().int().optional().default(10),
});

export const PopularSearchQuerySchema = z.object({
  limit: z.number().int().optional().default(10),
});

export const SuggestionsQuerySchema = z.object({
  q: z.string().min(1),
  limit: z.number().int().optional().default(10),
});
