import { createAsyncThunk } from '@reduxjs/toolkit';
import axiosClient from '../../lib/axios';
import { User, CreateUserRequest, UpdateUserRequest } from '../../types/user';
import { ROUTES } from '../../config/constants/url_routes';
import { ApiResponse, PagingResponse } from '../../types/response';
import {SERVER_ROUTES, SERVER_CUSTOMERS} from '../../config/constants/server_routes';

// 1. Lấy danh sách người dùng
export const fetchUsers = createAsyncThunk<
  PagingResponse<User>, 
  { pageNumber?: number; pageSize?: number; search?: string; sortBy?: string; isDescending?: boolean },
  { rejectValue: string }
>(
  'user/fetchUsers',
  async (params, { rejectWithValue }) => {
    try {
      const response = await axiosClient.get<
        ApiResponse<PagingResponse<User>>, 
        ApiResponse<PagingResponse<User>>
      >(SERVER_CUSTOMERS, {
        params: {
          PageNumber: params.pageNumber ?? 1,
          PageSize: params.pageSize ?? 10,
          Search: params.search,
          SortBy: params.sortBy ?? "Id",
          IsDescending: params.isDescending ?? true
        },
      });

      if (response.isSuccess) {
        return response.value; 
      }
      
      return rejectWithValue(response.errors?.[0] || 'Failed to fetch users');
    } catch (error: any) {
      const errorMessage = typeof error === 'string' 
        ? error 
        : (error as ApiResponse<any>)?.errors?.[0] || 'Failed to fetch users';
        
      return rejectWithValue(errorMessage);
    }
  }
);

// 2. Tạo người dùng mới (Register)
export const createUser = createAsyncThunk<
  boolean, 
  CreateUserRequest,
  { rejectValue: string }
>(
  'user/createUser',
  async (data, { rejectWithValue }) => {
    try {
      const response = await axiosClient.post<ApiResponse<boolean>>(SERVER_ROUTES.AUTH.REGISTER, data);
      return response.data.value; 
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.message || 'Failed to create user');
    }
  }
);

// 3. Cập nhật người dùng
export const updateUser = createAsyncThunk<
  { id: string; data: UpdateUserRequest }, // Payload trả về cho Redux
  { id: string; data: UpdateUserRequest }, // Tham số truyền vào Thunk
  { rejectValue: string }
>(
  'user/updateUser',
  async ({ id, data }, { rejectWithValue }) => {
    try {
      // Gọi API: PUT /api/customers?id=...
      const response = await axiosClient.put<ApiResponse<boolean>>(`${SERVER_CUSTOMERS}?id=${id}`, data);
      
      if (response.data.isSuccess) {
        return { id, data };
      }
      return rejectWithValue("Cập nhật thất bại từ máy chủ.");
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.message || 'Failed to update user');
    }
  }
);

// 4. Lấy chi tiết người dùng
export const getUserById = createAsyncThunk<
  User,
  string,
  { rejectValue: string }
>(
  'user/getUserById',
  async (id, { rejectWithValue }) => {
    try {
      const response = await axiosClient.get<ApiResponse<User>>(`${SERVER_CUSTOMERS}/${id}`);
      return response.data.value;
    } catch (error: any) {
      return rejectWithValue(error.response?.data?.message || 'Failed to get user details');
    }
  }
);