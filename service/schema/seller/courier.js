import { z } from 'zod';

export const CreateAgreementByEmailRequestSchema = z.object({
  restaurantId: z.string().uuid(),
  email: z.string().email(),
});

export const DeleteAgreementRequestSchema = z.object({
  id: z.string().uuid(),
});

export const AssignCourierRequestSchema = z.object({
  restaurantId: z.string().uuid(),
  orderId: z.string().uuid(),
  courierId: z.string().uuid(),
});

export const UpdateCourierSettingsRequestSchema = z.object({
  restaurantId: z.string().uuid(),
  defaultAssignmentStrategy: z.string().optional(),
  hasOwnCouriers: z.boolean().optional(),
  autoAssign: z.boolean().optional(),
  maxDeliveryRadius: z.number().optional(),
}).passthrough();

export const SellerRestaurantIdSchema = z.object({
  restaurantId: z.string().uuid(),
});
