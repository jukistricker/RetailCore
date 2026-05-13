import { useState } from 'react';
import { useSelector } from 'react-redux';
import { RootState } from '../../../store/store';
import { UpdateAccountDialog } from './UpdateAccountDialog';
import { Customer } from '../../../types/customer';

export const AccountDetails = () => {
  const { account, loading } = useSelector((state: RootState) => state.account);
  const [isModalOpen, setIsModalOpen] = useState(false);

  if (!account && loading) {
    return (
      <div className="d-flex justify-content-center p-5">
        <div className="spinner-border text-primary" role="status"></div>
      </div>
    );
  }

  const joinedDate = account?.createdDate 
    ? new Date(account.createdDate).toLocaleDateString('en-US') 
    : 'N/A';

  return (
    <div className="container-fluid py-4">
      <div className="row justify-content-center">
        <div className="col-md-10 col-lg-8">
          {/* Header */}
          <div className="d-flex justify-content-between align-items-center mb-4">
            <div>
              <h2 className="fw-bold mb-1">Personal Details</h2>
              <p className="text-muted small">Manage your account information</p>
            </div>
            <button 
              className="btn btn-primary shadow-sm px-4" 
              onClick={() => setIsModalOpen(true)}
            >
              <i className="bi bi-pencil-square me-2"></i>
              Cập nhật thông tin
            </button>
          </div>

          <div className="row g-4">
            {/* Cột trái: Thông tin chính */}
            <div className="col-md-7">
              <div className="card border-0 shadow-sm h-100">
                <div className="card-header py-3">
                  <h5 className="mb-0 fw-bold">Account Information</h5>
                </div>
                <div className="card-body">
                  <DetailItem label="Full Name" value={account?.fullName} />
                  <DetailItem label="Email" value={account?.email} isEmail />
                  <DetailItem label="Phone" value={account?.phone} placeholder="No data" />
                  <DetailItem label="Address" value={account?.address} placeholder="No data" />
                  <DetailItem label="City" value={account?.city} placeholder="No data" />
                </div>
              </div>
            </div>

            {/* Cột phải: Trạng thái & Hệ thống */}
            <div className="col-md-5">
              <div className="card border-0 shadow-sm mb-4">
                <div className="card-body">
                  <h6 className="text-muted mb-3 uppercase small fw-bold">Account Status</h6>
                  <div className="d-flex align-items-center mb-3">
                    {account?.isActive ? (
                      <span className="badge bg-success-subtle text-success px-3 py-2">
                        <i className="bi bi-check-circle-fill me-2"></i> Active
                      </span>
                    ) : (
                      <span className="badge bg-danger-subtle text-danger px-3 py-2">
                        <i className="bi bi-x-circle-fill me-2"></i> Locked
                      </span>
                    )}
                  </div>
                  <p className="small text-muted mb-0">
                    <i className="bi bi-calendar3 me-2"></i>
                    Created Date: <strong>{joinedDate}</strong>
                  </p>
                </div>
              </div>

              <div className="card border-0 shadow-sm">
                <div className="card-body">
                  <h6 className="text-muted mb-3 uppercase small fw-bold">Role</h6>
                  <div className="d-flex flex-wrap gap-2">
                    {(account as any)?.roles?.length > 0 ? (
                      (account as any).roles.map((role: string, idx: number) => (
                        <span key={idx} className="badge bg-primary-subtle text-primary border border-primary-subtle">
                          {role}
                        </span>
                      ))
                    ) : (
                      <span className="badge bg-secondary-subtle text-secondary">Customer</span>
                    )}
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <UpdateAccountDialog 
        isOpen={isModalOpen} 
        onClose={() => setIsModalOpen(false)} 
        currentData={account as Customer} 
      />
    </div>
  );
};

const DetailItem = ({ label, value, isEmail, placeholder }: any) => (
  <div className="mb-4">
    <label className="form-label text-muted small mb-1">{label}</label>
    <div className={`fw-semibold ${!value ? 'text-muted fst-italic fw-normal' : ''}`}>
      {isEmail && value ? <i className="bi bi-envelope me-2 text-primary"></i> : null}
      {value || placeholder}
    </div>
  </div>
);