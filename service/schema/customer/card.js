import { z } from 'zod';

export const CreateCardRequestSchema = z.object({
  cardHolderName: z.string().min(1),
  cardNumber: z.string().min(13),
  expireMonth: z.string().length(2),
  expireYear: z.string().length(4),
  cvc: z.string().min(3),
  alias: z.string().optional(),
});

export const DeleteCardSchema = z.object({
  cardToken: z.string().min(1),
});
