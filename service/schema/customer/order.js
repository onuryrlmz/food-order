import { z } from 'zod';
import { PaginationSchema } from '../base.js';

// Backend sözleşmesi: Domain.Dto.Buyer.Order.PlaceOrderRequestDto
// Alan adları backend DTO'su ile birebir eşleşmelidir (camelCase JSON serileştirme).
export const PlaceOrderRequestSchema = z.object({
  restaurantId: z.string().uuid(),
  deliveryAddressId: z.string().uuid().nullish(),
  invoiceAddressId: z.string().uuid().nullish(),
  paymentOptionId: z.number().int(),
  notes: z.string().nullish(),
  couponId: z.string().uuid().nullish(),
  couponCode: z.string().nullish(),
  discountAmount: z.number().nullish(),
  items: z.array(z.object({
    menuId: z.string().uuid(),
    quantity: z.number().int().min(1),
    values: z.array(z.object({
      menuOptionId: z.string().uuid(),
      menuOptionValueId: z.string().uuid(),
      productId: z.string().uuid().nullish(),
      quantity: z.number().int().min(1).optional(),
      options: z.array(z.object({
        menuOptionValueOptionId: z.string().uuid(),
        menuOptionValueOptionValueId: z.string().uuid(),
        productId: z.string().uuid().nullish(),
        quantity: z.number().int().min(1).optional(),
      })).optional(),
    })).optional(),
  })),
  // Online ödeme (kredi kartı) alanları — opsiyonel.
  cardToken: z.string().optional(),
  cardHolderName: z.string().optional(),
  cardNumber: z.string().optional(),
  expireMonth: z.string().optional(),
  expireYear: z.string().optional(),
  cvc: z.string().optional(),
  saveCard: z.boolean().optional(),
  cardAlias: z.string().optional(),
});

export const OrderIdSchema = z.object({
  orderId: z.string().uuid(),
});

export const CancelOrderRequestSchema = z.object({
  orderId: z.string().uuid(),
  reason: z.string().optional(),
});

export const InitiatePaymentRequestSchema = z.object({
  orderId: z.string().uuid(),
  cardToken: z.string().optional(),
  saveCard: z.boolean().optional(),
});

export const OrderHistoryQuerySchema = PaginationSchema;
