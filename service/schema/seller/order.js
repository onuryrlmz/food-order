import { z } from 'zod';

export const UpdateOrderStatusRequestSchema = z.object({
  orderId: z.string().uuid(),
  statusId: z.number().int(),
});
