import { z } from 'zod';

export const CreateCategoryRequestSchema = z.object({
  name: z.string().min(1),
  restaurantId: z.string().uuid(),
  orderIndex: z.number().int().optional(),
});

export const UpdateCategoryRequestSchema = CreateCategoryRequestSchema.extend({
  id: z.string().uuid(),
});

export const DeleteCategoryRequestSchema = z.object({
  id: z.string().uuid(),
  restaurantId: z.string().uuid(),
});

export const GetCategoriesQuerySchema = z.object({
  restaurantId: z.string().uuid(),
});

// CreateCategoryDetailRequestDto = { CategoryId, MenuId, OrderIndex }
export const CreateCategoryDetailRequestSchema = z.object({
  categoryId: z.string().uuid(),
  menuId: z.string().uuid(),
  orderIndex: z.number().int().optional(),
});

export const DeleteCategoryDetailRequestSchema = z.object({
  id: z.string().uuid(),
  categoryId: z.string().uuid().optional(),
  restaurantId: z.string().uuid().optional(),
});
