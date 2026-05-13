import React, { useEffect, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { RootState, AppDispatch } from '../../../../store/store';
import { fetchUsers } from '../../../../store/thunks/userThunk';
import { User } from '../../../../types/user';
import UserFormDialog from './UserFormDialog';
import UserDetailsDialog from './UserDetailsDialog';

const UserList: React.FC = () => {
  const dispatch = useDispatch<AppDispatch>();
  
  const { 
    items: users, 
    loading, 
    error, 
    currentPage, 
    pageSize, 
    totalCount 
  } = useSelector((state: RootState) => state.user);
  
  const [showFormDialog, setShowFormDialog] = useState(false);
  const [showDetailsDialog, setShowDetailsDialog] = useState(false);
  const [selectedUser, setSelectedUser] = useState<User | null>(null);

  useEffect(() => {
    // Cập nhật đúng tên params: pageNumber (không phải page)
    dispatch(fetchUsers({ pageNumber: currentPage, pageSize }));
  }, [dispatch, currentPage, pageSize]);

  const handleAddClick = () => {
    setSelectedUser(null);
    setShowFormDialog(true);
  };

  const handleEditClick = (user: User) => {
    setSelectedUser(user);
    setShowFormDialog(true);
  };

  const handleViewClick = (user: User) => {
    setSelectedUser(user);
    setShowDetailsDialog(true);
  };

  const handleFormClose = () => {
    setShowFormDialog(false);
    setSelectedUser(null);
  };

  const handleDetailsClose = () => {
    setShowDetailsDialog(false);
    setSelectedUser(null);
  };

  return (
    <div className="container-fluid">
      <div className="d-flex justify-content-between align-items-center mb-4">
        <div>
          <h1 className="fw-bold mb-0">Quản Lý Khách Hàng</h1>
          <p className="text-muted small mb-0">Tổng số: {totalCount} khách hàng</p>
        </div>
        <button className="btn btn-success shadow-sm" onClick={handleAddClick}>
          <i className="bi bi-plus-circle me-2"></i>Thêm Khách Hàng
        </button>
      </div>

      {error && (
        <div className="alert alert-danger shadow-sm" role="alert">
          <i className="bi bi-exclamation-triangle-fill me-2"></i>
          {error}
        </div>
      )}

      <div className="card border-0 shadow-sm">
        <div className="table-responsive">
          <table className="table table-hover align-middle mb-0">
            <thead className="table-light">
              <tr>
                <th className="py-3">STT</th>
                <th className="py-3">Họ Tên</th>
                <th className="py-3">Email</th>
                <th className="py-3">Địa Chỉ</th>
                <th className="py-3">Trạng Thái</th>
                <th className="py-3 text-end">Thao Tác</th>
              </tr>
            </thead>
            <tbody>
              {loading ? (
                <tr>
                  <td colSpan={6} className="text-center py-5">
                    <div className="spinner-border text-primary" role="status">
                      <span className="visually-hidden">Loading...</span>
                    </div>
                  </td>
                </tr>
              ) : users.length === 0 ? (
                <tr>
                  <td colSpan={6} className="text-center py-5 text-muted">
                    Không tìm thấy dữ liệu khách hàng
                  </td>
                </tr>
              ) : (
                users.map((user, index) => (
                  <tr key={user.id}>
                    <td className="text-muted">
                      {(currentPage - 1) * pageSize + index + 1}
                    </td>
                    <td>
                      <span className="fw-bold text-dark">{user.fullName}</span>
                    </td>
                    <td>{user.email}</td>
                    <td>
                      {user.city ? `${user.city}` : <span className="text-muted">---</span>}
                    </td>
                    <td>
                      {user.isActive ? (
                        <span className="badge rounded-pill bg-success-subtle text-success border border-success-subtle">
                          Hoạt động
                        </span>
                      ) : (
                        <span className="badge rounded-pill bg-danger-subtle text-danger border border-danger-subtle">
                          Bị khóa
                        </span>
                      )}
                    </td>
                    <td className="text-end">
                      <button
                        className="btn btn-sm btn-outline-info me-2"
                        onClick={() => handleViewClick(user)}
                        title="Xem chi tiết"
                      >
                        <i className="bi bi-eye"></i>
                      </button>
                      <button
                        className="btn btn-sm btn-outline-primary"
                        onClick={() => handleEditClick(user)}
                        title="Chỉnh sửa"
                      >
                        <i className="bi bi-pencil"></i>
                      </button>
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      </div>

      {showFormDialog && (
        <UserFormDialog
          user={selectedUser}
          onClose={handleFormClose}
        />
      )}

      {showDetailsDialog && selectedUser && (
        <UserDetailsDialog
          user={selectedUser}
          onClose={handleDetailsClose}
        />
      )}
    </div>
  );
};

export default UserList;