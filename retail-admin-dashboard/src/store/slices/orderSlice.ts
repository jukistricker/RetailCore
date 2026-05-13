import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { Order } from '../../types/order';
import { fetchOrders, createOrder, updateOrder, deleteOrder } from '../thunks/orderThunk';

interface OrderState {
  items: Order[];
  loading: boolean;
  error: string | null;
  currentPage: number;
  pageSize: number;
  total: number;
}

const initialState: OrderState = {
  items: [],
  loading: false,
  error: null,
  currentPage: 1,
  pageSize: 10,
  total: 0,
};

const orderSlice = createSlice({
  name: 'order',
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
    // Fetch Orders
    builder.addCase(fetchOrders.pending, (state) => {
      state.loading = true;
      state.error = null;
    });
    builder.addCase(fetchOrders.fulfilled, (state, action) => {
      state.loading = false;
      state.items = action.payload.items || [];
      state.total = action.payload.total || 0;
    });
    builder.addCase(fetchOrders.rejected, (state, action) => {
      state.loading = false;
      state.error = action.payload || 'Failed to fetch orders';
    });

    // Create Order
    builder.addCase(createOrder.pending, (state) => {
      state.loading = true;
      state.error = null;
    });
    builder.addCase(createOrder.fulfilled, (state, action) => {
      state.loading = false;
      state.items.unshift(action.payload);
      state.total += 1;
    });
    builder.addCase(createOrder.rejected, (state, action) => {
      state.loading = false;
      state.error = action.payload || 'Failed to create order';
    });

    // Update Order
    builder.addCase(updateOrder.pending, (state) => {
      state.loading = true;
      state.error = null;
    });
    builder.addCase(updateOrder.fulfilled, (state, action) => {
      state.loading = false;
      const index = state.items.findIndex(item => item.id === action.payload.id);
      if (index > -1) {
        state.items[index] = action.payload;
      }
    });
    builder.addCase(updateOrder.rejected, (state, action) => {
      state.loading = false;
      state.error = action.payload || 'Failed to update order';
    });

    // Delete Order
    builder.addCase(deleteOrder.pending, (state) => {
      state.loading = true;
      state.error = null;
    });
    builder.addCase(deleteOrder.fulfilled, (state, action) => {
      state.loading = false;
      state.items = state.items.filter(item => item.id !== action.payload);
      state.total -= 1;
    });
    builder.addCase(deleteOrder.rejected, (state, action) => {
      state.loading = false;
      state.error = action.payload || 'Failed to delete order';
    });
  },
});

export const { setCurrentPage, setPageSize, clearError } = orderSlice.actions;
export default orderSlice.reducer;
