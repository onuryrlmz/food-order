import { z } from 'zod';

export const GetRestaurantsByAddressSchema = z.object({
  addressId: z.string().uuid(),
});

export const GetRestaurantsByLocationSchema = z.object({
  latitude: z.number(),
  longitude: z.number(),
});

export const GetRestaurantInfoSchema = z.object({
  id: z.string().uuid(),
});
