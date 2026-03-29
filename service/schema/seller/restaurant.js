import { z } from 'zod';

export const AddRestaurantRequestSchema = z.object({
  name: z.string().min(1),
  description: z.string().optional(),
  phone: z.string().optional(),
  address: z.string().optional(),
  latitude: z.number().optional(),
  longitude: z.number().optional(),
  minimumOrderAmount: z.number().optional(),
  deliveryFee: z.number().optional(),
  estimatedDeliveryTime: z.number().int().optional(),
  cuisineIds: z.array(z.string().uuid()).optional(),
});

export const UpdateRestaurantRequestSchema = AddRestaurantRequestSchema.extend({
  id: z.string().uuid(),
});

export const RestaurantIdSchema = z.object({
  restaurantId: z.string().uuid(),
});

export const UpdateWorkingHoursRequestSchema = z.object({
  restaurantId: z.string().uuid(),
  dayOfWeek: z.number().int().min(0).max(6),
  openTime: z.string().optional(),
  closeTime: z.string().optional(),
  isClosed: z.boolean().optional(),
});
