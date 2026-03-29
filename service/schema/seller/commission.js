import { z } from 'zod';
import { PaginationSchema } from '../base.js';

export const SettlementPeriodsQuerySchema = PaginationSchema;

export const SettlementPeriodIdSchema = z.object({
  periodId: z.string().uuid(),
});
