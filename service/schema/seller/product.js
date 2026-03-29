import { z } from 'zod';

export const CreateProductRequestSchema = z.object({
  name: z.string().min(1),
  description: z.string().optional(),
  price: z.number().min(0),
  restaurantId: z.string().uuid(),
  categoryDetailId: z.string().uuid().optional(),
  isActive: z.boolean().optional(),
});

export const UpdateProductRequestSchema = CreateProductRequestSchema.extend({
  id: z.string().uuid(),
});

export const DeleteProductRequestSchema = z.object({
  id: z.string().uuid(),
  restaurantId: z.string().uuid(),
});

export const ProductImageSchema = z.object({
  productId: z.string().uuid(),
});

export const DeleteProductImageSchema = z.object({
  imageId: z.string().uuid(),
});
