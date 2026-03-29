import { z } from 'zod';

export const CreateOptionTemplateRequestSchema = z.object({
  name: z.string().min(1),
  restaurantId: z.string().uuid(),
});

export const UpdateOptionTemplateRequestSchema = CreateOptionTemplateRequestSchema.extend({
  id: z.string().uuid(),
});

export const DeleteOptionTemplateRequestSchema = z.object({
  id: z.string().uuid(),
  restaurantId: z.string().uuid(),
});

export const CreateTemplateValueRequestSchema = z.object({
  optionTemplateId: z.string().uuid().optional(),
  name: z.string().min(1).optional(),
});

export const DeleteTemplateValueRequestSchema = z.object({
  id: z.string().uuid(),
});

export const CreateTemplateValueOptionRequestSchema = z.object({
  name: z.string().min(1),
  optionTemplateValueId: z.string().uuid().optional(),
});

export const UpdateTemplateValueOptionRequestSchema = CreateTemplateValueOptionRequestSchema.extend({
  id: z.string().uuid(),
});

export const DeleteTemplateValueOptionRequestSchema = z.object({
  id: z.string().uuid(),
});

export const CreateTemplateValueOptionValueRequestSchema = z.object({
  productId: z.string().uuid().optional(),
  price: z.number().min(0),
  optionTemplateValueOptionId: z.string().uuid().optional(),
});

export const UpdateTemplateValueOptionValueRequestSchema = CreateTemplateValueOptionValueRequestSchema.extend({
  id: z.string().uuid(),
});

export const DeleteTemplateValueOptionValueRequestSchema = z.object({
  id: z.string().uuid(),
});
