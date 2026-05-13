import { configureStore } from '@reduxjs/toolkit';
import accountReducer from './slices/accountSlice';
import productReducer from './slices/productSlice';
import customerReducer from './slices/customerSlice';
import orderReducer from './slices/orderSlice';
import categoryReducer from './slices/categorySlice';
import reviewReducer from './slices/reviewSlice';

export const store = configureStore({
  reducer: {
    account: accountReducer,
    product: productReducer,
    customer: customerReducer,
    order: orderReducer,
    category: categoryReducer,
    review: reviewReducer,
  },
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware({
      serializableCheck: false, 
    }),
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
