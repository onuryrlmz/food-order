import { z } from 'zod';

// Backend sözleşmesi: Domain.Dto.Seller.Menu.* / MenuOption* DTO'ları
// Alan adları backend DTO'ları ile birebir eşleşmelidir (camelCase JSON serileştirme).

// CreateMenuRequestDto = { RestaurantId, Name, Description?, Price, OrderIndex }
export const CreateMenuRequestSchema = z.object({
  name: z.string().min(1),
  description: z.string().nullish(),
  price: z.number().min(0),
  restaurantId: z.string().uuid(),
  orderIndex: z.number().int().optional(),
  categoryDetailId: z.string().uuid().nullish(),
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

// CreateMenuOptionRequestDto = { MenuId, OptionTemplateId?, Name?, Description?, MinCount, MaxCount, OrderIndex }
export const CreateMenuOptionRequestSchema = z.object({
  menuId: z.string().uuid(),
  optionTemplateId: z.string().uuid().nullish(),
  name: z.string().min(1).nullish(),
  description: z.string().nullish(),
  minCount: z.number().int().optional(),
  maxCount: z.number().int().optional(),
  orderIndex: z.number().int().optional(),
});

export const UpdateMenuOptionRequestSchema = CreateMenuOptionRequestSchema.extend({
  id: z.string().uuid(),
});

export const DeleteMenuOptionRequestSchema = z.object({
  id: z.string().uuid(),
});

// CreateMenuOptionValueRequestDto = { MenuOptionId, ProductId, Price, OrderIndex }
export const CreateMenuOptionValueRequestSchema = z.object({
  menuOptionId: z.string().uuid(),
  productId: z.string().uuid().nullish(),
  price: z.number().min(0).optional(),
  orderIndex: z.number().int().optional(),
  name: z.string().min(1).nullish(),
});

export const DeleteMenuOptionValueRequestSchema = z.object({
  id: z.string().uuid(),
});

// CreateMenuOptionValueOptionRequestDto = { MenuOptionValueId, Name, Description?, MinCount, MaxCount, OrderIndex }
export const CreateMenuOptionValueOptionRequestSchema = z.object({
  menuOptionValueId: z.string().uuid(),
  name: z.string().min(1),
  description: z.string().nullish(),
  minCount: z.number().int().optional(),
  maxCount: z.number().int().optional(),
  orderIndex: z.number().int().optional(),
});

export const UpdateMenuOptionValueOptionRequestSchema = CreateMenuOptionValueOptionRequestSchema.extend({
  id: z.string().uuid(),
});

export const DeleteMenuOptionValueOptionRequestSchema = z.object({
  id: z.string().uuid(),
});

// CreateMenuOptionValueOptionValueRequestDto = { MenuOptionValueOptionId, ProductId, Price, OrderIndex }
export const CreateMenuOptionValueOptionValueRequestSchema = z.object({
  menuOptionValueOptionId: z.string().uuid(),
  productId: z.string().uuid().nullish(),
  price: z.number().min(0),
  orderIndex: z.number().int().optional(),
});

export const UpdateMenuOptionValueOptionValueRequestSchema = CreateMenuOptionValueOptionValueRequestSchema.extend({
  id: z.string().uuid(),
});

export const DeleteMenuOptionValueOptionValueRequestSchema = z.object({
  id: z.string().uuid(),
});
