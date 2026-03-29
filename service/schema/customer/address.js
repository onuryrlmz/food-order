import { z } from 'zod';

export const CreateAddressRequestSchema = z.object({
  title: z.string().min(1),
  address: z.string().min(1),
  latitude: z.number(),
  longitude: z.number(),
  isDefault: z.boolean().optional(),
});

export const UpdateAddressRequestSchema = CreateAddressRequestSchema.extend({
  id: z.string().uuid(),
});

export const AddressIdSchema = z.object({
  id: z.string().uuid(),
});
