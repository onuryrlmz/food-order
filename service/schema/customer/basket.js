import { z } from 'zod';

// Backend sözleşmesi: Domain.Dto.Buyer.UpdateBasketDto
// Alan adları backend DTO'su ile birebir eşleşmelidir (camelCase JSON serileştirme).
export const UpdateBasketRequestSchema = z.object({
  id: z.string().uuid().optional(),
  restaurantId: z.string().uuid(),
  sellerId: z.string().uuid().optional(),
  userShippingAddressId: z.string().uuid().optional(),
  userInvoiceAddressId: z.string().uuid().optional(),
  paymentOptionId: z.number().int().optional(),
  basketItems: z.array(z.object({
    menuId: z.string().uuid(),
    quantity: z.number().int().min(1),
    basketItemValues: z.array(z.object({
      menuOptionId: z.string().uuid(),
      menuOptionValueId: z.string().uuid(),
      productId: z.string().uuid().optional(),
      quantity: z.number().int().min(1).optional(),
      basketItemValueItemValues: z.array(z.object({
        menuOptionValueOptionId: z.string().uuid(),
        menuOptionValueOptionValueId: z.string().uuid(),
        productId: z.string().uuid().optional(),
        quantity: z.number().int().min(1).optional(),
      })).optional(),
    })).optional(),
  })),
});
