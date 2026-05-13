import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { Product } from '../../types/product';
import { fetchProducts, createProduct, updateProduct, deleteProduct } from '../thunks/productThunk';

interface ProductState {
  items: Product[];
  loading: boolean;
  error: string | null;
  currentPage: number;
  pageSize: number;
  totalPage: number;
  totalCount: number;
}

const initialState: ProductState = {
  items: [],
  loading: false,
  error: null,
  currentPage: 1,
  pageSize: 10,
  totalPage: 0,
  totalCount: 0
};

const productSlice = createSlice({
  name: 'product',
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
    // Fetch Products
    builder.addCase(fetchProducts.pending, (state) => {
      state.loading = true;
      state.error = null;
    });
    builder.addCase(fetchProducts.fulfilled, (state, action) => {
      state.loading = false;
      state.items = action.payload.items || [];
      state.totalPage = action.payload.totalPages || 0;
      state.totalCount = action.payload.totalCount || 0;
    });
    builder.addCase(fetchProducts.rejected, (state, action) => {
      state.loading = false;
      state.error = action.payload || 'Failed to fetch products';
    });

    // Create Product
    builder.addCase(createProduct.pending, (state) => {
      state.loading = true;
      state.error = null;
    });
    builder.addCase(createProduct.fulfilled, (state, action) => {
      state.loading = false;
      state.items.unshift(action.payload);
      state.totalCount += 1;
    });
    builder.addCase(createProduct.rejected, (state, action) => {
      state.loading = false;
      state.error = action.payload || 'Failed to create product';
    });

    // Update Product
    builder.addCase(updateProduct.pending, (state) => {
      state.loading = true;
      state.error = null;
    });
    builder.addCase(updateProduct.fulfilled, (state, action) => {
      state.loading = false;
      const index = state.items.findIndex(item => item.id === action.payload.id);
      if (index > -1) {
        state.items[index] = action.payload;
      }
    });
    builder.addCase(updateProduct.rejected, (state, action) => {
      state.loading = false;
      state.error = action.payload || 'Failed to update product';
    });

    // Delete Product
    builder.addCase(deleteProduct.pending, (state) => {
      state.loading = true;
      state.error = null;
    });
    builder.addCase(deleteProduct.fulfilled, (state, action) => {
      state.loading = false;
      state.items = state.items.filter(item => item.id !== action.payload);
      state.totalCount -= 1;
    });
    builder.addCase(deleteProduct.rejected, (state, action) => {
      state.loading = false;
      state.error = action.payload || 'Failed to delete product';
    });
  },
});

export const { setCurrentPage, setPageSize, clearError } = productSlice.actions;
export default productSlice.reducer;
