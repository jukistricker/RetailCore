import { createSlice, PayloadAction } from "@reduxjs/toolkit";
// Import kiểu User hoặc CustomerResponse mới của bạn
import { User } from "../../types/user"; 
import { CustomerResponse } from "../thunks/accountThunk"; 
import {
  fetchAccountById,
  fetchAllAccounts,
  loginUser,
  registerUser,
  toggleAccountStatus,
} from "../thunks/accountThunk";

interface AccountState {
  user: CustomerResponse | null; 
  accounts: CustomerResponse[];
  loading: boolean;
  error: any;
}

const getSavedUser = (): CustomerResponse | null => {
  const saved = localStorage.getItem("user");
  if (!saved) return null;
  try {
    return JSON.parse(saved);
  } catch {
    return null;
  }
};

const initialState: AccountState = {
  user: getSavedUser(),
  accounts: [],
  loading: false,
  error: null,
};

const accountSlice = createSlice({
  name: "account",
  initialState,
  reducers: {
    logout: (state) => {
      state.user = null;
      localStorage.removeItem("user");
    },
    clearError: (state) => {
      state.error = null;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(loginUser.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(
        loginUser.fulfilled,
        (state, action: PayloadAction<CustomerResponse>) => { 
          state.loading = false;
          state.user = action.payload;
          localStorage.setItem("user", JSON.stringify(action.payload));
        },
      )
      .addCase(loginUser.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload;
      })
      .addCase(registerUser.fulfilled, (state) => {
        state.loading = false;
      })
      // Fetch Account By Id
      .addCase(
        fetchAccountById.fulfilled,
        (state, action: PayloadAction<CustomerResponse>) => {
          state.loading = false;
          if (state.user?.id === action.payload.id) {
            state.user = action.payload;
          }
        },
      )
      // Fetch All
      .addCase(fetchAllAccounts.fulfilled, (state, action: PayloadAction<CustomerResponse[]>) => {
        state.loading = false;
        state.accounts = action.payload;
      })
      // Toggle Status
      .addCase(
        toggleAccountStatus.fulfilled, 
        (state, action: PayloadAction<CustomerResponse>) => {
          const updatedAcc = action.payload;
          const index = state.accounts.findIndex(a => a.id === updatedAcc.id);
          if (index !== -1) {
            state.accounts[index] = updatedAcc;
          }
          if (state.user?.id === updatedAcc.id) {
            state.user = updatedAcc;
            localStorage.setItem("user", JSON.stringify(updatedAcc));
          }
        }
      );
  },
});

export const { logout } = accountSlice.actions;
export default accountSlice.reducer;