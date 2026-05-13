import React, { useEffect, useState } from 'react';
import { BaseDialog } from '../../../components/ui/BaseDialog';
import { Customer, UpdateCustomerRequest } from '../../../types/customer';
import { useDispatch, useSelector } from 'react-redux';
import { AppDispatch, RootState } from '../../../store/store';
import { updateCustomer } from '../../../store/thunks/customerThunk';
import { currentDetails } from '../../../store/thunks/accountThunk';
import { toast } from 'react-hot-toast';

interface UpdateAccountDialogProps {
  isOpen: boolean;
  onClose: () => void;
  currentData: Customer | null;
}

export const UpdateAccountDialog = ({ isOpen, onClose, currentData }: UpdateAccountDialogProps) => {
  const dispatch = useDispatch<AppDispatch>();
  const { loading } = useSelector((state: RootState) => state.customer || {}); // Giả sử bạn có loading trong customer slice
  const [isSubmitting, setIsSubmitting] = useState(false);

  // Local state để quản lý form
  const [formData, setFormData] = useState<UpdateCustomerRequest>({
    fullName: '',
    email: currentData?.email || '',
    phone: '',
    address: '',
    city: '',
    isActive: true
  });

  // Đồng bộ data từ props vào state khi mở dialog
  useEffect(() => {
    if (currentData && isOpen) {
      setFormData({
        fullName: currentData.fullName,
        email: currentData.email,
        phone: currentData.phone || '',
        address: currentData.address || '',
        city: currentData.city || '',
        isActive: currentData.isActive
      });
    }
  }, [currentData, isOpen]);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!currentData?.id) return;

    try {
      setIsSubmitting(true); // Bắt đầu quay khi BẤM nút
      const resultAction = await dispatch(updateCustomer({ 
        id: currentData.id, 
        data: formData 
      }));

      if (updateCustomer.fulfilled.match(resultAction)) {
        toast.success("Cập nhật thông tin thành công!");
        dispatch(currentDetails()); 
        onClose();
      } else {
        toast.error(resultAction.payload as string);
      }
    } catch (error) {
      toast.error("Có lỗi xảy ra, vui lòng thử lại!");
    }
    finally {
      setIsSubmitting(false);
    }
  };

  return (
    <BaseDialog 
      isOpen={isOpen} 
      onClose={onClose} 
      title="Update Profile"
      footer={
        <>
          <button className="btn btn-secondary" onClick={onClose} disabled={loading}>
            Hủy
          </button>
          <button 
            className="btn btn-primary" 
            form="update-profile-form" 
            disabled={isSubmitting} 
          >
            {isSubmitting && <span className="spinner-border spinner-border-sm me-2"></span>}
            Save changes
          </button>
        </>
      }
    >
      <form id="update-profile-form" onSubmit={handleSubmit}>
        <div className="row">
          <div className="col-md-6 mb-3">
            <label className="form-label fw-bold small">Full Name</label>
            <input 
              type="text" 
              name="fullName"
              className="form-control" 
              value={formData.fullName} 
              onChange={handleChange}
              required 
            />
          </div>
          <div className="col-md-6 mb-3">
            <label className="form-label fw-bold small">Phone</label>
            <input 
              type="text" 
              name="phone"
              className="form-control" 
              value={formData.phone} 
              onChange={handleChange}
            />
          </div>
        </div>


        <div className="mb-3">
          <label className="form-label fw-bold small">Address</label>
          <input 
            type="text" 
            name="address"
            className="form-control" 
            value={formData.address} 
            onChange={handleChange}
          />
        </div>

        <div className="mb-3">
          <label className="form-label fw-bold small">City</label>
          <input 
            type="text" 
            name="city"
            className="form-control" 
            value={formData.city} 
            onChange={handleChange}
          />
        </div>
      </form>
    </BaseDialog>
  );
};