import { createAsyncThunk } from '@reduxjs/toolkit';
import axiosClient from '../../lib/axios';
import { Category, CreateCategoryRequest, UpdateCategoryRequest } from '../../types/category';
import { PagingResponse } from '../../types/response';
import { SERVER_CATEGORIES } from '../../config/constants/server_routes';

// 1. Lấy danh sách phân trang
export const fetchCategories = createAsyncThunk<
  PagingResponse<Category>,
  { pageNumber?: number; pageSize?: number; search?: string },
  { rejectValue: string }
>(
  'category/fetchCategories',
  async (params, { rejectWithValue }) => {
    try {
      const response = await axiosClient.get<any, PagingResponse<Category>>(SERVER_CATEGORIES, {
        params: {
          PageNumber: params.pageNumber ?? 1,
          PageSize: params.pageSize ?? 10,
          Search: params.search
        },
      });
      return response; 
    } catch (error: any) {
      return rejectWithValue(error.message || 'Failed to fetch categories');
    }
  }
);

export const createCategory = createAsyncThunk<
  Category,
  CreateCategoryRequest,
  { rejectValue: string }
>(
  'category/createCategory',
  async (data, { rejectWithValue }) => {
    try {
      const response = await axiosClient.post<any, Category>(SERVER_CATEGORIES, data);
      return response;
    } catch (error: any) {
      return rejectWithValue(error.message || 'Failed to create category');
    }
  }
);

export const updateCategory = createAsyncThunk<
  Category,
  UpdateCategoryRequest,
  { rejectValue: string }
>(
  'category/updateCategory',
  async (data, { rejectWithValue }) => {
    try {
      // Sửa: Nhận về Category trực tiếp
      const response = await axiosClient.put<any, Category>(SERVER_CATEGORIES, data);
      return response;
    } catch (error: any) {
      return rejectWithValue(error.message || 'Failed to update category');
    }
  }
);

// 4. Xóa
export const deleteCategory = createAsyncThunk<
  string,
  string,
  { rejectValue: string }
>(
  'category/deleteCategory',
  async (id, { rejectWithValue }) => {
    try {
      await axiosClient.delete<any, boolean>(`${SERVER_CATEGORIES}/${id}`);
      return id;
    } catch (error: any) {
      return rejectWithValue(error.message || 'Failed to delete category');
    }
  }
);