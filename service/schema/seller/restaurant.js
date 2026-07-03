import { z } from 'zod';

// Backend sözleşmesi: Domain.Dto.Seller.Restaurant.UpdateRestaurantDto / AddRestaurantDto
// Alan adları backend DTO'su ile birebir eşleşmelidir (camelCase JSON serileştirme).
export const AddRestaurantRequestSchema = z.object({
  name: z.string().min(1),
  description: z.string().nullish(),
  phone: z.string().nullish(),
  email: z.string().nullish(),
  address: z.string().nullish(),
  latitude: z.number().nullish(),
  longitude: z.number().nullish(),
  minimumOrderPrice: z.number().nullish(),
  minDeliveryTime: z.number().int().nullish(),
  maxDeliveryTime: z.number().int().nullish(),
  coverImage: z.string().nullish(),
  cuisineIds: z.array(z.string().uuid()).nullish(),
});

export const UpdateRestaurantRequestSchema = AddRestaurantRequestSchema.extend({
  id: z.string().uuid(),
});

export const RestaurantIdSchema = z.object({
  restaurantId: z.string().uuid(),
});

// Backend: RestaurantWorkingHour.DayOfWeek 1=Pazartesi .. 7=Pazar
export const UpdateWorkingHoursRequestSchema = z.object({
  restaurantId: z.string().uuid(),
  dayOfWeek: z.number().int().min(1).max(7),
  openTime: z.string().nullish(),
  closeTime: z.string().nullish(),
  isClosed: z.boolean().optional(),
});
