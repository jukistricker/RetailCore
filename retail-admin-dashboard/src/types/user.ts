export interface User {
  id: string;
  userId?: string;
  fullName: string;
  email: string;
  phone?: string;
  address?: string;
  city?: string;
  isActive: boolean;
  createdDate?: string;
}

export interface CreateUserRequest {
  fullName: string;
  email: string;
  password?: string; // Nếu BE yêu cầu
  phone?: string;
  address?: string;
  city?: string;
}

export interface UpdateUserRequest {
  fullName: string;
  email: string;
  phone?: string;
  address?: string;
  city?: string;
  isActive: boolean;
}