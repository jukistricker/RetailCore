import { createAsyncThunk } from '@reduxjs/toolkit';
import axiosClient from '../../lib/axios';
import { Review, CreateReviewRequest, UpdateReviewRequest } from '../../types/review';

export const fetchReviews = createAsyncThunk<
  { items: Review[]; total: number },
  { page?: number; pageSize?: number },
  { rejectValue: string }
>(
  'review/fetchReviews',
  async ({ page = 1, pageSize = 10 }, { rejectWithValue }) => {
    try {
      const response = await axiosClient.get('/reviews', {
        params: { page, pageSize },
      });
      return response;
    } catch (error: any) {
      return rejectWithValue(error.message || 'Failed to fetch reviews');
    }
  }
);

export const createReview = createAsyncThunk<
  Review,
  CreateReviewRequest,
  { rejectValue: string }
>(
  'review/createReview',
  async (data, { rejectWithValue }) => {
    try {
      const response = await axiosClient.post('/reviews', data);
      return response;
    } catch (error: any) {
      return rejectWithValue(error.message || 'Failed to create review');
    }
  }
);

export const updateReview = createAsyncThunk<
  Review,
  UpdateReviewRequest,
  { rejectValue: string }
>(
  'review/updateReview',
  async (data, { rejectWithValue }) => {
    try {
      const { id, ...body } = data;
      const response = await axiosClient.put(`/reviews/${id}`, body);
      return response;
    } catch (error: any) {
      return rejectWithValue(error.message || 'Failed to update review');
    }
  }
);

export const deleteReview = createAsyncThunk<
  string | number,
  string | number,
  { rejectValue: string }
>(
  'review/deleteReview',
  async (id, { rejectWithValue }) => {
    try {
      await axiosClient.delete(`/reviews/${id}`);
      return id;
    } catch (error: any) {
      return rejectWithValue(error.message || 'Failed to delete review');
    }
  }
);
