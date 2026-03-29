import { z } from 'zod';
import { PaginationSchema } from '../base.js';

// Finance
export const SellerIdSchema = z.object({ sellerId: z.string().uuid() });
export const UpdateCommissionRateSchema = z.object({ rate: z.number().min(0).max(100) });

// Analytics
export const AdminAnalyticsQuerySchema = z.object({
  startDate: z.string(),
  endDate: z.string(),
});
export const AdminTopRestaurantsQuerySchema = AdminAnalyticsQuerySchema.extend({
  limit: z.number().int().optional().default(10),
});

// Orders
export const OverdueOrdersQuerySchema = PaginationSchema;

// Reviews
export const AdminReviewsQuerySchema = PaginationSchema.extend({
  status: z.string().optional(),
});
export const AdminReviewIdSchema = z.object({ reviewId: z.string().uuid() });

// Support
export const AdminTicketsQuerySchema = PaginationSchema.extend({
  statusId: z.number().int().optional(),
  topicId: z.number().int().optional(),
});
export const AdminTicketIdSchema = z.object({ ticketId: z.string().uuid() });
export const AdminActionIdSchema = z.object({ actionId: z.string().uuid() });

// Commission
export const AdminRestaurantIdSchema = z.object({ restaurantId: z.string().uuid() });
export const SetRestaurantCommissionSchema = z.object({
  restaurantId: z.string().uuid(),
  commissionRate: z.number().optional(),
  fixedFee: z.number().optional(),
  effectiveFrom: z.string().optional(),
  notes: z.string().optional(),
});

// Settlement
export const AdminSettlementPeriodIdSchema = z.object({ periodId: z.string().uuid() });
export const AdminSettlementPeriodsQuerySchema = PaginationSchema.extend({
  statusId: z.number().int().optional(),
});
export const PaySettlementSchema = z.object({
  periodId: z.string().uuid(),
  bankTransferRef: z.string().optional(),
  notes: z.string().optional(),
});
export const CancelSettlementSchema = z.object({
  periodId: z.string().uuid(),
  reason: z.string().optional(),
});

// Courier
export const ApproveCourierSchema = z.object({ courierId: z.string().uuid() });

// Platform Schedule
export const CreatePlatformScheduleSchema = z.object({
  commissionRate: z.number().min(0).max(100),
  fixedFee: z.number().min(0).optional(),
  effectiveFrom: z.string().optional(),
  notes: z.string().optional(),
});
