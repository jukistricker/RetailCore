import { currentDetails, CustomerResponse, refreshToken } from "../thunks/accountThunk"; 
import {
  fetchAccountById,
  fetchAllAccounts,
  loginUser,
  registerUser,
  toggleAccountStatus,
} from "../thunks/accountThunk";
import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { Customer } from "../../types/customer";

interface AccountState {
  account: Customer | null;    // Thông tin người dùng đang đăng nhập
  isAuthenticated: boolean;    // Check nhanh trạng thái đã auth chưa
  isInitialized: boolean;      // Check xem đã chạy refresh token xong lần đầu chưa
  loading: boolean;
  error: any;
}

const getSavedAccount = (): Customer | null => {
  const saved = localStorage.getItem("account");
  if (!saved) return null;
  try {
    return JSON.parse(saved);
  } catch {
    return null;
  }
};

const initialState: AccountState = {
  account: getSavedAccount(),
  isAuthenticated: !!getSavedAccount(),
  isInitialized: false, 
  loading: false,
  error: null,
};

const accountSlice = createSlice({
  name: "account",
  initialState,
  reducers: {
    logout: (state) => {
      state.account = null;
      state.isAuthenticated = false;
      state.isInitialized = true; 
      localStorage.removeItem("account");
    },
    clearAccountError: (state) => {
      state.error = null;
    },
  },
  extraReducers: (builder) => {
    builder
      // LOGIN
      .addCase(loginUser.fulfilled, (state, action: PayloadAction<Customer>) => {
        state.loading = false;
        state.isAuthenticated = true;
        state.isInitialized = true; 
      })

      // REFRESH TOKEN
      .addCase(refreshToken.pending, (state) => {
        state.loading = true;
      })
      .addCase(refreshToken.fulfilled, (state, action: PayloadAction<Customer>) => {
        state.loading = false;
        state.isAuthenticated = true;
        state.isInitialized = true; 
      })
      .addCase(refreshToken.rejected, (state, action) => {
        state.loading = false;
        state.isAuthenticated = false;
        state.isInitialized = true; // QUAN TRỌNG: Lỗi cũng phải cho qua
        state.error = action.payload;
      })

      // CURRENT DETAILS
      .addCase(currentDetails.pending, (state) => {
        state.loading = true;
      })
      .addCase(currentDetails.fulfilled, (state, action: PayloadAction<CustomerResponse>) => {
        state.loading = false;
        state.account = action.payload;
        state.isAuthenticated = true;
        state.isInitialized = true; 
        localStorage.setItem("account", JSON.stringify(action.payload));
      })
      .addCase(currentDetails.rejected, (state, action) => {
        state.loading = false;
        state.account = null;
        state.isAuthenticated = false;
        state.isInitialized = true;
        state.error = action.payload;
        localStorage.removeItem("account");
      })


      // Fix Matcher để bắt cả 'auth/' và 'account/' hoặc dùng regex
      .addMatcher(
        (action) => action.type.endsWith('/pending'),
        (state) => {
          state.loading = true;
        }
      )
      .addMatcher(
        (action) => action.type.endsWith('/rejected') && !action.type.includes('refreshToken'),
        (state, action: any) => {
          state.loading = false;
          state.error = action.payload;
        }
      );
  },
});

export const { logout, clearAccountError } = accountSlice.actions;
export default accountSlice.reducer;
