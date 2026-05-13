import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { User } from '../../types/user';
import { fetchUsers, createUser, updateUser, getUserById } from '../thunks/userThunk';

interface UserState {
  items: User[];
  selectedUser: User | null; 
  loading: boolean;
  error: string | null;
  currentPage: number;
  pageSize: number;
  totalCount: number;
  totalPage: number;
}

const initialState: UserState = {
  items: [],
  selectedUser: null,
  loading: false,
  error: null,
  currentPage: 1,
  pageSize: 10,
  totalCount: 0,
  totalPage: 0
};

const userSlice = createSlice({
  name: 'user',
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
    clearSelectedUser: (state) => {
      state.selectedUser = null;
    }
  },
  extraReducers: (builder) => {
    // --- Fetch Users ---
    builder.addCase(fetchUsers.fulfilled, (state, action) => {
      state.loading = false;
      state.items = action.payload.items || [];
      state.totalCount = action.payload.totalCount || 0;
      state.totalPage = action.payload.totalPages || 0;
    });

    // --- Create User ---
    builder.addCase(createUser.fulfilled, (state) => {
      state.loading = false;
    });

    // --- Update User ---
    builder.addCase(updateUser.fulfilled, (state, action) => {
      state.loading = false;
      const { id, data } = action.payload;
      const index = state.items.findIndex(item => item.id === id);
      if (index > -1) {
        // Cập nhật dữ liệu mới vào state local để UI thay đổi ngay lập tức
        state.items[index] = { ...state.items[index], ...data };
      }
      if (state.selectedUser?.id === id) {
        state.selectedUser = { ...state.selectedUser, ...data };
      }
    });

    // --- Get User By Id ---
    builder.addCase(getUserById.fulfilled, (state, action) => {
      state.loading = false;
      state.selectedUser = action.payload;
    });

    // --- Xử lý trạng thái Pending và Rejected chung ---
    builder.addMatcher(
      (action) => action.type.endsWith('/pending'),
      (state) => {
        state.loading = true;
        state.error = null;
      }
    );
    builder.addMatcher(
      (action) => action.type.endsWith('/rejected'),
      (state, action: any) => {
        state.loading = false;
        state.error = action.payload || 'Đã có lỗi xảy ra.';
      }
    );
  },
});

export const { setCurrentPage, setPageSize, clearError, clearSelectedUser } = userSlice.actions;
export default userSlice.reducer;