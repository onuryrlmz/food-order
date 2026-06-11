import { z } from 'zod';

export const LoginRequestSchema = z.object({
  email: z.string().email(),
  password: z.string().min(1),
});

export const RegisterRequestSchema = z.object({
  firstName: z.string().min(1),
  lastName: z.string().min(1),
  email: z.string().email(),
  phoneNumber: z.string().min(10),
  password: z.string().min(6),
});

export const UpdateProfileRequestSchema = z.object({
  firstName: z.string().min(1).optional(),
  lastName: z.string().min(1).optional(),
  phoneNumber: z.string().optional(),
});

export const ChangePasswordRequestSchema = z.object({
  currentPassword: z.string().min(1),
  newPassword: z.string().min(6),
});

export const ForgotPasswordRequestSchema = z.object({
  emailOrPhone: z.string().min(1),
});

export const VerifyResetCodeRequestSchema = z.object({
  emailOrPhone: z.string().min(1),
  code: z.string().min(1),
});

export const ResetPasswordRequestSchema = z.object({
  emailOrPhone: z.string().min(1),
  code: z.string().min(1),
  newPassword: z.string().min(6),
});

export const LogoutRequestSchema = z.object({
  refreshToken: z.string().nullable().optional(),
});

export const RefreshTokenRequestSchema = z.object({
  refreshToken: z.string().min(1),
});

export const SellerRegisterRequestSchema = z.object({
  companyType: z.number().int(),
  name: z.string().min(1),
  legalName: z.string().min(1),
  taxCode: z.string().regex(/^\d{10,11}$/, 'Vergi numarası 10 veya 11 haneli ve rakamlardan oluşmalıdır'),
  taxArea: z.string().min(1),
  iban: z.string().regex(/^TR\d{24}$/, 'IBAN TR ile başlamalı ve 26 karakter olmalıdır'),
  isEInvoiceAvaible: z.boolean().default(false),
  identityNumber: z.string().length(11).optional(),
  ownerFirstName: z.string().min(1),
  ownerLastName: z.string().min(1),
  ownerEmail: z.string().email(),
  ownerPhone: z.string().min(10),
  password: z.string().min(8),
  cityId: z.string().uuid(),
  townId: z.string().uuid(),
  neighbourhoodId: z.string().uuid(),
  addressLine1: z.string().min(1),
  addressLine2: z.string().optional(),
});
