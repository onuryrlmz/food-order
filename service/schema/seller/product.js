import { z } from 'zod';

// AddProductDto = { RestaurantId, CuisineId?, Name, ProductType, Description?, Price, OrderIndex }
export const CreateProductRequestSchema = z.object({
  name: z.string().min(1),
  description: z.string().nullish(),
  price: z.number().min(0),
  restaurantId: z.string().uuid(),
  productType: z.number().int().optional(),
  orderIndex: z.number().int().optional(),
  cuisineId: z.string().uuid().nullish(),
  categoryDetailId: z.string().uuid().nullish(),
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
