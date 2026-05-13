import { createAsyncThunk } from "@reduxjs/toolkit";
import { api } from "../../lib/api";
import { LoginFormValues, RegisterFormValues } from "../../features/accounts/types";
import { genericStorage } from "../../utils/storage";
import { USER_KEY } from "../../config/constants/storage_key";
import { SERVER_ROUTES } from "../../config/constants/server_routes";

// Interface khớp với CustomerResponse từ .NET
export interface CustomerResponse {
  id: string;
  userId?: string;
  fullName: string;
  email: string;
  phone?: string;
  address?: string;
  city?: string;
  isActive: boolean;
  createdDate: string;
  roles?: string[];
}

// --- LOGIN USER ---
export const loginUser = createAsyncThunk<CustomerResponse, LoginFormValues>(
  "auth/login",
  async (request, { rejectWithValue }) => {
    try {
      const user = await api.post<CustomerResponse>(SERVER_ROUTES.AUTH.LOGIN, request);
      return user;
    } catch (error: any) {
      console.log(error);
      return rejectWithValue(error.response?.data || { message: "Login failed" });
    }
  }
);

// --- REGISTER USER ---
export const registerUser = createAsyncThunk<{ success: boolean }, RegisterFormValues>(
  "auth/register",
  async (request, { rejectWithValue }) => {
    try {
      // Gửi thẳng object đăng ký cho BE
      await api.post(SERVER_ROUTES.AUTH.REGISTER, request);
      return { success: true };
    } catch (error: any) {
      return rejectWithValue(error.response?.data || { message: "Registration failed" });
    }
  }
);

// --- FETCH ALL CUSTOMERS ---
export const fetchAllAccounts = createAsyncThunk<CustomerResponse[], void>(
  "customers/fetchAll",
  async (_, { rejectWithValue }) => {
    try {
      return await api.get<CustomerResponse[]>("/api/customers");
    } catch (error: any) {
      return rejectWithValue(error.response?.data);
    }
  }
);

// --- FETCH BY ID ---
export const fetchAccountById = createAsyncThunk<CustomerResponse, string>(
  "customers/fetchById",
  async (id, { rejectWithValue }) => {
    try {
      return await api.get<CustomerResponse>(`/api/customers/${id}`);
    } catch (error: any) {
      return rejectWithValue(error.response?.data);
    }
  }
);

// --- TOGGLE STATUS ---
export const toggleAccountStatus = createAsyncThunk<CustomerResponse, { id: string; customer: CustomerResponse }>(
  "customers/toggleStatus",
  async ({ id, customer }, { rejectWithValue }) => {
    try {
      const updateData = {
        ...customer,
        isActive: !customer.isActive 
      };

      // Gọi đúng API Put của BE
      return await api.put<CustomerResponse>(`/api/customers?id=${id}`, updateData);
    } catch (error: any) {
      return rejectWithValue(error.response?.data);
    }
  }
);

// --- REFRESH TOKEN ---
export const refreshToken = createAsyncThunk<CustomerResponse, void>(
  "auth/refreshToken",
  async (_, { rejectWithValue }) => {
    try {
      const response = await api.post<CustomerResponse>(`${SERVER_ROUTES.AUTH.REFRESH_TOKEN}`, {}); 
      return response;
    } catch (error: any) {
      localStorage.remove(USER_KEY);
      return rejectWithValue(error.response?.data || "Session expired");
    }
  }
);

export const currentDetails = createAsyncThunk<CustomerResponse, void>(
  "auth/currentDetails",
  async (_, { rejectWithValue }) => {
    try {
      const response= await api.get<CustomerResponse>(`${SERVER_ROUTES.AUTH.CURRENT_DETAILS}`);
      localStorage.setItem(USER_KEY, JSON.stringify(response));
      return response;
    } catch (error: any) {
      return rejectWithValue(error.response?.data || "Session expired");
    }
  }
);