import { z } from 'zod';
import { PaginationSchema } from '../base.js';

export const CreateTicketRequestSchema = z.object({
  topicId: z.number().int(),
  orderId: z.string().uuid().optional(),
  message: z.string().min(1),
});

export const SendMessageRequestSchema = z.object({
  ticketId: z.string().uuid(),
  message: z.string().min(1),
});

export const TicketIdSchema = z.object({
  ticketId: z.string().uuid(),
});

export const TicketListQuerySchema = PaginationSchema.extend({
  statusId: z.number().int().optional(),
});

export const RateTicketRequestSchema = z.object({
  ticketId: z.string().uuid(),
  rating: z.number().int().min(1).max(5),
});
