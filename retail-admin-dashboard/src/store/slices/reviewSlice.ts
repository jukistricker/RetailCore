import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { Review } from '../../types/review';
import { fetchReviews, createReview, updateReview, deleteReview } from '../thunks/reviewThunk';

interface ReviewState {
  items: Review[];
  loading: boolean;
  error: string | null;
  currentPage: number;
  pageSize: number;
  total: number;
}

const initialState: ReviewState = {
  items: [],
  loading: false,
  error: null,
  currentPage: 1,
  pageSize: 10,
  total: 0,
};

const reviewSlice = createSlice({
  name: 'review',
  initialState,
  reducers: {
    setCurrentPage: (state, action: PayloadAction<number>) => {
      state.currentPage = action.payload;
    },
    setPageSize: (state, action: PayloadAction<number>) => {
      state.pageSize = action.payload;
    },
    clearError: (state) => {
      state.error = null;
    },
  },
  extraReducers: (builder) => {
    // Fetch Reviews
    builder.addCase(fetchReviews.pending, (state) => {
      state.loading = true;
      state.error = null;
    });
    builder.addCase(fetchReviews.fulfilled, (state, action) => {
      state.loading = false;
      state.items = action.payload.items || [];
      state.total = action.payload.total || 0;
    });
    builder.addCase(fetchReviews.rejected, (state, action) => {
      state.loading = false;
      state.error = action.payload || 'Failed to fetch reviews';
    });

    // Create Review
    builder.addCase(createReview.pending, (state) => {
      state.loading = true;
      state.error = null;
    });
    builder.addCase(createReview.fulfilled, (state, action) => {
      state.loading = false;
      state.items.unshift(action.payload);
      state.total += 1;
    });
    builder.addCase(createReview.rejected, (state, action) => {
      state.loading = false;
      state.error = action.payload || 'Failed to create review';
    });

    // Update Review
    builder.addCase(updateReview.pending, (state) => {
      state.loading = true;
      state.error = null;
    });
    builder.addCase(updateReview.fulfilled, (state, action) => {
      state.loading = false;
      const index = state.items.findIndex(item => item.id === action.payload.id);
      if (index > -1) {
        state.items[index] = action.payload;
      }
    });
    builder.addCase(updateReview.rejected, (state, action) => {
      state.loading = false;
      state.error = action.payload || 'Failed to update review';
    });

    // Delete Review
    builder.addCase(deleteReview.pending, (state) => {
      state.loading = true;
      state.error = null;
    });
    builder.addCase(deleteReview.fulfilled, (state, action) => {
      state.loading = false;
      state.items = state.items.filter(item => item.id !== action.payload);
      state.total -= 1;
    });
    builder.addCase(deleteReview.rejected, (state, action) => {
      state.loading = false;
      state.error = action.payload || 'Failed to delete review';
    });
  },
});

export const { setCurrentPage, setPageSize, clearError } = reviewSlice.actions;
export default reviewSlice.reducer;
