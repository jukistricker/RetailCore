import { createAsyncThunk } from '@reduxjs/toolkit';
import axiosClient from '../../lib/axios';
import { Order, CreateOrderRequest, UpdateOrderRequest } from '../../types/order';

export const fetchOrders = createAsyncThunk<
  { items: Order[]; total: number },
  { page?: number; pageSize?: number },
  { rejectValue: string }
>(
  'order/fetchOrders',
  async ({ page = 1, pageSize = 10 }, { rejectWithValue }) => {
    try {
      const response = await axiosClient.get('/orders', {
        params: { page, pageSize },
      });
      return response;
    } catch (error: any) {
      return rejectWithValue(error.message || 'Failed to fetch orders');
    }
  }
);

export const createOrder = createAsyncThunk<
  Order,
  CreateOrderRequest,
  { rejectValue: string }
>(
  'order/createOrder',
  async (data, { rejectWithValue }) => {
    try {
      const response = await axiosClient.post('/orders', data);
      return response;
    } catch (error: any) {
      return rejectWithValue(error.message || 'Failed to create order');
    }
  }
);

export const updateOrder = createAsyncThunk<
  Order,
  UpdateOrderRequest,
  { rejectValue: string }
>(
  'order/updateOrder',
  async (data, { rejectWithValue }) => {
    try {
      const { id, ...body } = data;
      const response = await axiosClient.put(`/orders/${id}`, body);
      return response;
    } catch (error: any) {
      return rejectWithValue(error.message || 'Failed to update order');
    }
  }
);

export const deleteOrder = createAsyncThunk<
  string | number,
  string | number,
  { rejectValue: string }
>(
  'order/deleteOrder',
  async (id, { rejectWithValue }) => {
    try {
      await axiosClient.delete(`/orders/${id}`);
      return id;
    } catch (error: any) {
      return rejectWithValue(error.message || 'Failed to delete order');
    }
  }
);
