import React, { useState, useEffect } from 'react';
import { useDispatch } from 'react-redux';
import { AppDispatch } from '../../../../store/store';
import { createUser, updateUser } from '../../../../store/thunks/userThunk';
import { User, CreateUserRequest } from '../../../../types/user';
import {BaseDialog} from '../../../../components/ui/BaseDialog';
import toast from 'react-hot-toast';

interface UserFormDialogProps {
  user: User | null;
  onClose: () => void;
}

const UserFormDialog: React.FC<UserFormDialogProps> = ({ user, onClose }) => {
  const dispatch = useDispatch<AppDispatch>();
  const [formData, setFormData] = useState<CreateUserRequest>({
    username: '',
    email: '',
    fullName: '',
    phone: '',
    address: '',
    role: 'customer',
  });
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (user) {
      setFormData({
        username: user.username,
        email: user.email,
        fullName: user.fullName,
        phone: user.phone || '',
        address: user.address || '',
        role: user.role || 'customer',
      });
    }
  }, [user]);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value,
    }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);

    try {
      if (user) {
        const result = await dispatch(updateUser({ ...formData, id: user.id }));
        if (updateUser.fulfilled.match(result)) {
          toast.success('Cập nhật người dùng thành công!');
          onClose();
        }
      } else {
        const result = await dispatch(createUser(formData));
        if (createUser.fulfilled.match(result)) {
          toast.success('Thêm người dùng thành công!');
          onClose();
        }
      }
    } catch (error) {
      toast.error('Có lỗi xảy ra!');
    } finally {
      setLoading(false);
    }
  };

  return (
    <BaseDialog
      isOpen={true}
      title={user ? 'Chỉnh Sửa Người Dùng' : 'Thêm Người Dùng Mới'}
      onClose={onClose}
    >
      <form onSubmit={handleSubmit}>
        <div className="row mb-3">
          <div className="col-md-6">
            <label htmlFor="username" className="form-label">
              Tên Đăng Nhập <span className="text-danger">*</span>
            </label>
            <input
              type="text"
              className="form-control"
              id="username"
              name="username"
              value={formData.username}
              onChange={handleChange}
              required
            />
          </div>
          <div className="col-md-6">
            <label htmlFor="email" className="form-label">
              Email <span className="text-danger">*</span>
            </label>
            <input
              type="email"
              className="form-control"
              id="email"
              name="email"
              value={formData.email}
              onChange={handleChange}
              required
            />
          </div>
        </div>

        <div className="mb-3">
          <label htmlFor="fullName" className="form-label">
            Tên Đầy Đủ <span className="text-danger">*</span>
          </label>
          <input
            type="text"
            className="form-control"
            id="fullName"
            name="fullName"
            value={formData.fullName}
            onChange={handleChange}
            required
          />
        </div>

        <div className="row mb-3">
          <div className="col-md-6">
            <label htmlFor="phone" className="form-label">
              Điện Thoại
            </label>
            <input
              type="tel"
              className="form-control"
              id="phone"
              name="phone"
              value={formData.phone}
              onChange={handleChange}
            />
          </div>
          <div className="col-md-6">
            <label htmlFor="role" className="form-label">
              Vai Trò <span className="text-danger">*</span>
            </label>
            <select
              className="form-select"
              id="role"
              name="role"
              value={formData.role}
              onChange={handleChange}
              required
            >
              <option value="customer">Khách Hàng</option>
              <option value="staff">Nhân Viên</option>
              <option value="admin">Quản Trị Viên</option>
            </select>
          </div>
        </div>

        <div className="mb-3">
          <label htmlFor="address" className="form-label">
            Địa Chỉ
          </label>
          <input
            type="text"
            className="form-control"
            id="address"
            name="address"
            value={formData.address}
            onChange={handleChange}
          />
        </div>

        <div className="d-flex gap-2 justify-content-end">
          <button type="button" className="btn btn-secondary" onClick={onClose}>
            Hủy
          </button>
          <button type="submit" className="btn btn-primary" disabled={loading}>
            {loading ? 'Đang Lưu...' : 'Lưu'}
          </button>
        </div>
      </form>
    </BaseDialog>
  );
};

export default UserFormDialog;
