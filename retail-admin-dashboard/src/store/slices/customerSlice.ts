import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { Customer } from '../../types/customer';
import { fetchCustomers, updateCustomer, getCustomerById } from '../thunks/customerThunk';

interface CustomerState {
  items: Customer[];
  selectedCustomer: Customer | null;
  loading: boolean;
  error: string | null;
  currentPage: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

const initialState: CustomerState = {
  items: [],
  selectedCustomer: null,
  loading: false,
  error: null,
  currentPage: 1,
  pageSize: 10,
  totalCount: 0,
  totalPages: 0
};

const customerSlice = createSlice({
  name: 'customer',
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
    clearSelectedCustomer: (state) => { state.selectedCustomer = null; }
  },

  extraReducers: (builder) => {
    builder
      .addCase(fetchCustomers.fulfilled, (state, action) => {
        state.loading = false;
        state.items = action.payload.items;
        state.totalCount = action.payload.totalCount;
        state.totalPages = action.payload.totalPages;
        state.currentPage = action.payload.pageNumber;
      })
      .addCase(updateCustomer.fulfilled, (state, action) => {
        state.loading = false;
        const index = state.items.findIndex(c => c.id === action.payload.id);
        if (index !== -1) state.items[index] = action.payload;
        if (state.selectedCustomer?.id === action.payload.id) state.selectedCustomer = action.payload;
      })
      .addCase(getCustomerById.fulfilled, (state, action) => {
        state.loading = false;
        state.selectedCustomer = action.payload;
      })
      .addMatcher(action => action.type.endsWith('/pending'), (state) => {
        state.loading = true;
        state.error = null;
      })
      .addMatcher(action => action.type.endsWith('/rejected'), (state, action: any) => {
        state.loading = false;
        state.error = action.payload;
      });
  }
});

export const { setCurrentPage, setPageSize, clearError, clearSelectedCustomer } = customerSlice.actions;
export default customerSlice.reducer;