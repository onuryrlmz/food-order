import { z } from 'zod';

export const CreateTipRequestSchema = z.object({
  orderId: z.string().uuid(),
  amount: z.number().min(0),
});

export const TipOrderIdSchema = z.object({
  orderId: z.string().uuid(),
});
