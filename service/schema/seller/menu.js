import { z } from 'zod';

export const CreateMenuRequestSchema = z.object({
  name: z.string().min(1),
  description: z.string().optional(),
  price: z.number().min(0),
  restaurantId: z.string().uuid(),
  categoryDetailId: z.string().uuid().optional(),
});

export const UpdateMenuRequestSchema = CreateMenuRequestSchema.extend({
  id: z.string().uuid(),
});

export const DeleteMenuRequestSchema = z.object({
  id: z.string().uuid(),
  restaurantId: z.string().uuid(),
});

export const GetMenusByRestaurantSchema = z.object({
  restaurantId: z.string().uuid(),
});

export const CreateMenuOptionRequestSchema = z.object({
  name: z.string().min(1),
  menuId: z.string().uuid(),
  isRequired: z.boolean().optional(),
  maxSelection: z.number().int().optional(),
});

export const UpdateMenuOptionRequestSchema = CreateMenuOptionRequestSchema.extend({
  id: z.string().uuid(),
});

export const DeleteMenuOptionRequestSchema = z.object({
  id: z.string().uuid(),
});

export const CreateMenuOptionValueRequestSchema = z.object({
  menuOptionId: z.string().uuid(),
  name: z.string().min(1).optional(),
});

export const DeleteMenuOptionValueRequestSchema = z.object({
  id: z.string().uuid(),
});

export const CreateMenuOptionValueOptionRequestSchema = z.object({
  menuOptionValueId: z.string().uuid().optional(),
  name: z.string().min(1),
});

export const UpdateMenuOptionValueOptionRequestSchema = CreateMenuOptionValueOptionRequestSchema.extend({
  id: z.string().uuid(),
});

export const DeleteMenuOptionValueOptionRequestSchema = z.object({
  id: z.string().uuid(),
});

export const CreateMenuOptionValueOptionValueRequestSchema = z.object({
  menuOptionValueOptionId: z.string().uuid().optional(),
  productId: z.string().uuid().optional(),
  price: z.number().min(0),
  orderIndex: z.number().int().optional(),
});

export const UpdateMenuOptionValueOptionValueRequestSchema = CreateMenuOptionValueOptionValueRequestSchema.extend({
  id: z.string().uuid(),
});

export const DeleteMenuOptionValueOptionValueRequestSchema = z.object({
  id: z.string().uuid(),
});
