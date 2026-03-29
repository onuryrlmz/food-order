import { z } from 'zod';
import { PaginationSchema } from '../base.js';

export const PlaceOrderRequestSchema = z.object({
  restaurantId: z.string().uuid(),
  addressId: z.string().uuid(),
  paymentMethod: z.number().int(),
  items: z.array(z.object({
    menuId: z.string().uuid(),
    quantity: z.number().int().min(1),
    note: z.string().optional(),
    values: z.array(z.object({
      menuOptionId: z.string().uuid(),
      menuOptionValueId: z.string().uuid(),
    })).optional(),
  })),
  couponCode: z.string().optional(),
  orderNote: z.string().optional(),
  tipAmount: z.number().optional(),
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
