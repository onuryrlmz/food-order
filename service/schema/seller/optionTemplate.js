import { z } from 'zod';

// CreateOptionTemplateRequestDto = { RestaurantId, Name, Description?, MinCount, MaxCount }
export const CreateOptionTemplateRequestSchema = z.object({
  name: z.string().min(1),
  restaurantId: z.string().uuid(),
  description: z.string().nullish(),
  minCount: z.number().int().optional(),
  maxCount: z.number().int().optional(),
});

export const UpdateOptionTemplateRequestSchema = CreateOptionTemplateRequestSchema.extend({
  id: z.string().uuid(),
});

export const DeleteOptionTemplateRequestSchema = z.object({
  id: z.string().uuid(),
  restaurantId: z.string().uuid(),
});

// AddOptionTemplateValueRequestDto = { OptionTemplateId, ProductId, Price }
export const CreateTemplateValueRequestSchema = z.object({
  optionTemplateId: z.string().uuid(),
  productId: z.string().uuid().nullish(),
  price: z.number().min(0).optional(),
  name: z.string().min(1).nullish(),
});

export const DeleteTemplateValueRequestSchema = z.object({
  id: z.string().uuid(),
});

// AddOptionTemplateValueOptionRequestDto = { OptionTemplateValueId, Name, Description?, MinCount, MaxCount }
export const CreateTemplateValueOptionRequestSchema = z.object({
  name: z.string().min(1),
  optionTemplateValueId: z.string().uuid(),
  description: z.string().nullish(),
  minCount: z.number().int().optional(),
  maxCount: z.number().int().optional(),
});

export const UpdateTemplateValueOptionRequestSchema = CreateTemplateValueOptionRequestSchema.extend({
  id: z.string().uuid(),
});

export const DeleteTemplateValueOptionRequestSchema = z.object({
  id: z.string().uuid(),
});

// AddOptionTemplateValueOptionValueRequestDto = { OptionTemplateValueOptionId, ProductId, Price }
export const CreateTemplateValueOptionValueRequestSchema = z.object({
  productId: z.string().uuid().nullish(),
  price: z.number().min(0),
  optionTemplateValueOptionId: z.string().uuid(),
});

export const UpdateTemplateValueOptionValueRequestSchema = CreateTemplateValueOptionValueRequestSchema.extend({
  id: z.string().uuid(),
});

export const DeleteTemplateValueOptionValueRequestSchema = z.object({
  id: z.string().uuid(),
});
