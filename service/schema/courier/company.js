import { z } from 'zod';

export const CompanyRegisterRequestSchema = z.object({
  companyName: z.string().min(1),
  taxNumber: z.string().min(1),
  taxOffice: z.string().optional(),
  iban: z.string().optional(),
  phone: z.string().optional(),
  address: z.string().optional(),
});
