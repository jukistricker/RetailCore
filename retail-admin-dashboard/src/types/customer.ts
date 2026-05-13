export interface Customer {
  id: string;          
  userId?: string;     
  fullName: string;
  email: string;
  password?: string;
  phone?: string;
  address?: string;
  city?: string;
  isActive: boolean;
  createdDate: string;
}

export interface CreateCustomerRequest {
  fullName: string;
  email: string;
  password?: string;
}

export interface UpdateCustomerRequest {
  fullName: string;
  email: string;
  phone?: string;
  address?: string;
  city?: string;
  isActive: boolean;
}