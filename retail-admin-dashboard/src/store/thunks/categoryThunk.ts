import { createAsyncThunk } from '@reduxjs/toolkit';
import axiosClient from '../../lib/axios';
import { Category, CreateCategoryRequest, UpdateCategoryRequest } from '../../types/category';
import { ApiResponse, PagingResponse } from '../../types/response';
import {SERVER_CATEGORIES} from '../../config/constants/server_routes';

// 1. Lấy danh sách phân trang
export const fetchCategories = createAsyncThunk<
  PagingResponse<Category>,
  { pageNumber?: number; pageSize?: number; search?: string },
  { rejectValue: string }
>(
  'category/fetchCategories',
  async (params, { rejectWithValue }) => {
    try {
      const response = await axiosClient.get<
        ApiResponse<PagingResponse<Category>>,
        ApiResponse<PagingResponse<Category>>
      >(SERVER_CATEGORIES, {
        params: {
          PageNumber: params.pageNumber ?? 1,
          PageSize: params.pageSize ?? 10,
          Search: params.search
        },
      });
      return response.value;
    } catch (error: any) {
      return rejectWithValue(error.message || 'Failed to fetch categories');
    }
  }
);

// 2. Tạo mới
export const createCategory = createAsyncThunk<
  Category,
  CreateCategoryRequest,
  { rejectValue: string }
>(
  'category/createCategory',
  async (data, { rejectWithValue }) => {
    try {
      const response = await axiosClient.post<
        ApiResponse<Category>,
        ApiResponse<Category>
      >(SERVER_CATEGORIES, data);
      return response.value;
    } catch (error: any) {
      return rejectWithValue(error.message || 'Failed to create category');
    }
  }
);

// 3. Cập nhật (BE dùng [FromBody] cho toàn bộ UpdateCategoryRequest)
export const updateCategory = createAsyncThunk<
  Category,
  UpdateCategoryRequest,
  { rejectValue: string }
>(
  'category/updateCategory',
  async (data, { rejectWithValue }) => {
    try {
      // Lưu ý: Controller BE [HttpPut] không có {id} trên route
      const response = await axiosClient.put<
        ApiResponse<Category>,
        ApiResponse<Category>
      >(SERVER_CATEGORIES, data);
      return response.value;
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
      await axiosClient.delete<
        ApiResponse<boolean>,
        ApiResponse<boolean>
      >(`${SERVER_CATEGORIES}/${id}`);
      return id;
    } catch (error: any) {
      return rejectWithValue(error.message || 'Failed to delete category');
    }
  }
);