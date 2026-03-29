import { z } from 'zod';

export const ApiMessageSchema = z.object({
  description: z.string().optional(),
  code: z.string().optional(),
});

export const ApiResponseSchema = (dataSchema) => {
  const base = z.object({
    hasFailed: z.boolean(),
    messages: z.array(ApiMessageSchema).optional(),
    token: z.string().optional().nullable(),
    refreshToken: z.string().optional().nullable(),
  });
  if (!dataSchema) return base;
  return base.extend({ data: dataSchema });
};

// Common reusable schemas
export const PaginationSchema = z.object({
  page: z.number().int().min(1).default(1),
  pageSize: z.number().int().min(1).max(100).default(20),
});

export const IdSchema = z.object({
  id: z.string().uuid(),
});

export const DateRangeSchema = z.object({
  from: z.string().nullable().optional(),
  to: z.string().nullable().optional(),
});
