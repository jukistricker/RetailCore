import React from 'react';
import { Customer } from '../../../../types/customer';
import {BaseDialog} from '../../../../components/ui/BaseDialog';

interface UserDetailsDialogProps {
  user: Customer;
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
          <div className="col-sm-4 fw-bold">Email:</div>
          <div className="col-sm-8">{user.email}</div>
        </div>
        <div className="row mb-3">
          <div className="col-sm-4 fw-bold">Full Name:</div>
          <div className="col-sm-8">{user.fullName}</div>
        </div>
        {user.phone && (
          <div className="row mb-3">
            <div className="col-sm-4 fw-bold">Phone:</div>
            <div className="col-sm-8">{user.phone}</div>
          </div>
        )}
        {user.address && (
          <div className="row mb-3">
            <div className="col-sm-4 fw-bold">Address:</div>
            <div className="col-sm-8">{user.address}</div>
          </div>
        )}
       
        {user.createdDate && (
          <div className="row mb-3">
            <div className="col-sm-4 fw-bold">Created Date:</div>
            <div className="col-sm-8">{new Date(user.createdDate).toLocaleString('en-US')}</div>
          </div>
        )}
      </div>
      <div className="d-flex gap-2 justify-content-end mt-4">
        <button type="button" className="btn btn-secondary" onClick={onClose}>
          Close
        </button>
      </div>
    </BaseDialog>
  );
};

export default UserDetailsDialog;
