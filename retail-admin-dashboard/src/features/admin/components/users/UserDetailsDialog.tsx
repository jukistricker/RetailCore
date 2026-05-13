import React from 'react';
import { User } from '../../../../types/user';
import {BaseDialog} from '../../../../components/ui/BaseDialog';

interface UserDetailsDialogProps {
  user: User;
  onClose: () => void;
}

const UserDetailsDialog: React.FC<UserDetailsDialogProps> = ({ user, onClose }) => {
  return (
    <BaseDialog
      isOpen={true}
      title="Chi Tiết Người Dùng"
      onClose={onClose}
    >
      <div className="space-y-3">
        <div className="row mb-3">
          <div className="col-sm-4 fw-bold">ID:</div>
          <div className="col-sm-8">{user.id}</div>
        </div>
        <div className="row mb-3">
          <div className="col-sm-4 fw-bold">Tên Đăng Nhập:</div>
          <div className="col-sm-8">{user.username}</div>
        </div>
        <div className="row mb-3">
          <div className="col-sm-4 fw-bold">Email:</div>
          <div className="col-sm-8">{user.email}</div>
        </div>
        <div className="row mb-3">
          <div className="col-sm-4 fw-bold">Tên Đầy Đủ:</div>
          <div className="col-sm-8">{user.fullName}</div>
        </div>
        {user.phone && (
          <div className="row mb-3">
            <div className="col-sm-4 fw-bold">Điện Thoại:</div>
            <div className="col-sm-8">{user.phone}</div>
          </div>
        )}
        {user.address && (
          <div className="row mb-3">
            <div className="col-sm-4 fw-bold">Địa Chỉ:</div>
            <div className="col-sm-8">{user.address}</div>
          </div>
        )}
        <div className="row mb-3">
          <div className="col-sm-4 fw-bold">Vai Trò:</div>
          <div className="col-sm-8">
            <span className="badge bg-primary">{user.role || 'customer'}</span>
          </div>
        </div>
        <div className="row mb-3">
          <div className="col-sm-4 fw-bold">Trạng Thái:</div>
          <div className="col-sm-8">
            <span className={`badge ${user.status === 'active' ? 'bg-success' : 'bg-danger'}`}>
              {user.status || 'active'}
            </span>
          </div>
        </div>
        {user.createdAt && (
          <div className="row mb-3">
            <div className="col-sm-4 fw-bold">Tạo Lúc:</div>
            <div className="col-sm-8">{new Date(user.createdAt).toLocaleString('vi-VN')}</div>
          </div>
        )}
      </div>
      <div className="d-flex gap-2 justify-content-end mt-4">
        <button type="button" className="btn btn-secondary" onClick={onClose}>
          Đóng
        </button>
      </div>
    </BaseDialog>
  );
};

export default UserDetailsDialog;
