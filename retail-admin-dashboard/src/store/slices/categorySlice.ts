import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { Category } from '../../types/category';
import { fetchCategories, createCategory, updateCategory, deleteCategory } from '../thunks/categoryThunk';

interface CategoryState {
  items: Category[];
  loading: boolean;
  error: string | null;
  currentPage: number;
  pageSize: number;
  totalPage: number;
  totalCount: number;
}

const initialState: CategoryState = {
  items: [],
  loading: false,
  error: null,
  currentPage: 1,
  pageSize: 10,
  totalPage: 0,
  totalCount: 0
};

const categorySlice = createSlice({
  name: 'category',
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
    // Fetch Categories
    builder.addCase(fetchCategories.pending, (state) => {
      state.loading = true;
      state.error = null;
    });
    builder.addCase(fetchCategories.fulfilled, (state, action) => {
      state.loading = false;
      state.items = action.payload.items || [];
      state.totalPage = action.payload.totalPages || 0;
      state.totalCount = action.payload.totalCount || 0;
    });
    builder.addCase(fetchCategories.rejected, (state, action) => {
      state.loading = false;
      state.error = action.payload || 'Failed to fetch categories';
    });

    // Create Category
    builder.addCase(createCategory.pending, (state) => {
      state.loading = true;
      state.error = null;
    });
    builder.addCase(createCategory.fulfilled, (state, action) => {
      state.loading = false;
      state.items.unshift(action.payload);
      state.totalCount += 1;
    });
    builder.addCase(createCategory.rejected, (state, action) => {
      state.loading = false;
      state.error = action.payload || 'Failed to create category';
    });

    // Update Category
    builder.addCase(updateCategory.pending, (state) => {
      state.loading = true;
      state.error = null;
    });
    builder.addCase(updateCategory.fulfilled, (state, action) => {
      state.loading = false;
      const index = state.items.findIndex(item => item.id === action.payload.id);
      if (index > -1) {
        state.items[index] = action.payload;
      }
    });
    builder.addCase(updateCategory.rejected, (state, action) => {
      state.loading = false;
      state.error = action.payload || 'Failed to update category';
    });

    // Delete Category
    builder.addCase(deleteCategory.pending, (state) => {
      state.loading = true;
      state.error = null;
    });
    builder.addCase(deleteCategory.fulfilled, (state, action) => {
      state.loading = false;
      state.items = state.items.filter(item => item.id !== action.payload);
      state.totalCount -= 1;
    });
    builder.addCase(deleteCategory.rejected, (state, action) => {
      state.loading = false;
      state.error = action.payload || 'Failed to delete category';
    });
  },
});

export const { setCurrentPage, setPageSize, clearError } = categorySlice.actions;
export default categorySlice.reducer;
