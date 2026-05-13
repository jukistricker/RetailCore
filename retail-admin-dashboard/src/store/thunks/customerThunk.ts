import { createAsyncThunk } from '@reduxjs/toolkit';
import axiosClient from '../../lib/axios';
import { Customer, CreateCustomerRequest, UpdateCustomerRequest } from '../../types/customer';
import { ApiResponse, PagingResponse } from '../../types/response';
import { SERVER_CUSTOMERS, SERVER_ROUTES } from '../../config/constants/server_routes';


export const fetchCustomers = createAsyncThunk<
  PagingResponse<Customer>, // Đây là cái Redux mong đợi
  { pageNumber?: number; pageSize?: number; search?: string; sortBy?: string; isDescending?: boolean },
  { rejectValue: string }
>('customer/fetchAll', async (params, { rejectWithValue }) => {
  try {
    // SỬA Ở ĐÂY: Thay ApiResponse<PagingResponse<Customer>> thành PagingResponse<Customer>
    const response = await axiosClient.get<any, PagingResponse<Customer>>(SERVER_CUSTOMERS, {
      params: {
        PageNumber: params.pageNumber ?? 1,
        PageSize: params.pageSize ?? 10,
        Search: params.search,
        SortBy: params.sortBy ?? "Id",
        IsDescending: params.isDescending ?? true
      },
    });

    return response; 
  } catch (error: any) {
    return rejectWithValue(error.message || 'Failed to fetch customers');
  }
});

export const updateCustomer = createAsyncThunk<
  Customer,
  { id: string; data: UpdateCustomerRequest },
  { rejectValue: string }
>('customer/update', async ({ id, data }, { rejectWithValue }) => {
  try {
    const response = await axiosClient.put<any, Customer>(`${SERVER_CUSTOMERS}?id=${id}`, data);
    
    return response; 
  } catch (error: any) {
    return rejectWithValue(error.message || 'Update failed');
  }
});

export const getCustomerById = createAsyncThunk<Customer, string, { rejectValue: string }>(
  'customer/getById',
  async (id, { rejectWithValue }) => {
    try {
      const response = await axiosClient.get<any, Customer>(`${SERVER_CUSTOMERS}/${id}`);
      
      return response; 
    } catch (error: any) {
      return rejectWithValue(error.message);
    }
  }
);