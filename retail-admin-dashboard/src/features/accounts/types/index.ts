import { z } from 'zod';

// --- LOGIN SCHEMA ---
export const loginSchema = z.object({
  email: z.string()
    .min(1, "Email is required")
    .email("Invalid email format"),
  password: z.string()
    .min(1, "Password is required"),
});

export type LoginFormValues = z.infer<typeof loginSchema>;

// --- REGISTER SCHEMA ---
export const registerSchema = z.object({
  email: z.string()
    .min(1, "Email is required")
    .email("Invalid email format"),
  fullName: z.string()
    .min(1, "Full name is required"),
  password: z.string()
    .min(8, "Password must be at least 8 characters")
    .max(32, "Password must be at most 32 characters"),
  confirmPassword: z.string()
    .min(1, "Confirmation password is required"),
}).refine((data) => data.password === data.confirmPassword, {
  message: "Confirmation password doesn't match.",
  path: ["confirmPassword"], 
});

export type RegisterFormValues = z.infer<typeof registerSchema>;