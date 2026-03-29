import { z } from 'zod';
import { PaginationSchema, DateRangeSchema } from '../base.js';

export const CourierRegisterRequestSchema = z.object({
  courierTypeId: z.number().int(),
  vehicleType: z.string().min(1),
  vehiclePlate: z.string().optional(),
  identityNumber: z.string().optional(),
  iban: z.string().optional(),
  courierCompanyId: z.string().uuid().optional(),
});

export const UpdateCourierProfileRequestSchema = z.object({
  vehicleType: z.string().optional(),
  vehiclePlate: z.string().optional(),
  iban: z.string().optional(),
  phone: z.string().optional(),
});

export const UpdateLocationRequestSchema = z.object({
  latitude: z.number(),
  longitude: z.number(),
});

export const AssignmentIdSchema = z.object({
  assignmentId: z.string().uuid(),
});

export const AgreementIdSchema = z.object({
  agreementId: z.string().uuid(),
});

export const AssignmentHistoryQuerySchema = PaginationSchema;

export const EarningsQuerySchema = PaginationSchema.merge(DateRangeSchema);
