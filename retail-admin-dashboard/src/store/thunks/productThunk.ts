import { createAsyncThunk } from '@reduxjs/toolkit';
import axiosClient from '../../lib/axios';
import { Product, ProductCreateRequest, ProductUpdateRequest } from '../../types/product';
import { ApiResponse, PagingResponse } from '../../types/response';
import { SERVER_PRODUCTS } from '../../config/constants/server_routes';

// 1. Lấy danh sách sản phẩm (Phân trang)
export const fetchProducts = createAsyncThunk<
  PagingResponse<Product>,
  { pageNumber?: number; pageSize?: number; categoryId?: string; slug?: string },
  { rejectValue: string }
>(
  'product/fetchProducts',
  async (params, { rejectWithValue }) => {
    try {
      const response = await axiosClient.get<any, ApiResponse<PagingResponse<Product>>>(SERVER_PRODUCTS, {
        params: {
          PageNumber: params.pageNumber ?? 1,
          PageSize: params.pageSize ?? 10,
          categoryId: params.categoryId,
          slug: params.slug
        },
      });
      return response.value;
    } catch (error: any) {
      return rejectWithValue(error.message || 'Failed to fetch products');
    }
  }
);

// 2. Tạo sản phẩm (Sử dụng FormData cho [FromForm])
export const createProduct = createAsyncThunk<
  Product,
  ProductCreateRequest,
  { rejectValue: string }
>(
  'product/createProduct',
  async (data, { rejectWithValue }) => {
    try {
      const formData = new FormData();
      Object.entries(data).forEach(([key, value]) => {
        if (key === 'images' && value) {
          (value as File[]).forEach(file => formData.append('images', file));
        } else if (value !== undefined) {
          formData.append(key, value.toString());
        }
      });

      const response = await axiosClient.post<any, ApiResponse<Product>>(SERVER_PRODUCTS, formData, {
        headers: { 'Content-Type': 'multipart/form-data' }
      });
      return response.value;
    } catch (error: any) {
      return rejectWithValue(error.message || 'Failed to create product');
    }
  }
);

// 3. Cập nhật sản phẩm
export const updateProduct = createAsyncThunk<
  Product,
  ProductUpdateRequest,
  { rejectValue: string }
>(
  'product/updateProduct',
  async (data, { rejectWithValue }) => {
    try {
      const formData = new FormData();
      Object.entries(data).forEach(([key, value]) => {
        if (key === 'images' && value) {
          (value as File[]).forEach(file => formData.append('images', file));
        } else if (value !== undefined) {
          formData.append(key, value.toString());
        }
      });

      const response = await axiosClient.put<any, ApiResponse<Product>>(
        `${SERVER_PRODUCTS}/${data.id}`, 
        formData,
        { headers: { 'Content-Type': 'multipart/form-data' } }
      );
      return response.value;
    } catch (error: any) {
      return rejectWithValue(error.message || 'Failed to update product');
    }
  }
);

// 4. Xóa sản phẩm
export const deleteProduct = createAsyncThunk<
  string,
  string,
  { rejectValue: string }
>(
  'product/deleteProduct',
  async (id, { rejectWithValue }) => {
    try {
      await axiosClient.delete<any, ApiResponse<boolean>>(`${SERVER_PRODUCTS}/${id}`);
      return id;
    } catch (error: any) {
      return rejectWithValue(error.message || 'Failed to delete product');
    }
  }
);