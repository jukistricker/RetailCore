import React, { useState, useEffect } from 'react';
import { useDispatch } from 'react-redux';
import { AppDispatch } from '../../../../store/store';
import { BaseDialog } from '../../../../components/ui/BaseDialog';
import toast from 'react-hot-toast';
import { Customer, UpdateCustomerRequest } from '../../../../types/customer';
import { fetchCustomers, updateCustomer } from '../../../../store/thunks/customerThunk';
import { registerUser } from '../../../../store/thunks/accountThunk';

interface UserFormDialogProps {
  customer: Customer | null;
  onClose: () => void;
}

const UserFormDialog: React.FC<UserFormDialogProps> = ({ customer, onClose }) => {
  const dispatch = useDispatch<AppDispatch>();
  
  // Khởi tạo state
  const [formData, setFormData] = useState({
    fullName: '',
    email: '',
    password: '',
    confirmPassword: '',
    phone: '',
    address: '',
    city: '',
    isActive: true,
  });

  const [loading, setLoading] = useState(false);

  // Cập nhật form khi mở modal (Edit mode)
  useEffect(() => {
    if (customer) {
      setFormData({
        fullName: customer.fullName || '',
        email: customer.email || '',
        password: '', 
        confirmPassword: '',
        phone: customer.phone || '',
        address: customer.address || '',
        city: customer.city || '',
        isActive: customer.isActive ?? true,
      });
    } else {
      // Reset form khi là mode Add New
      setFormData({
        fullName: '',
        email: '',
        password: '',
        confirmPassword: '',
        phone: '',
        address: '',
        city: '',
        isActive: true,
      });
    }
  }, [customer]);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    const target = e.target as HTMLInputElement;
    const name = target.name;
    const value = target.type === 'checkbox' ? target.checked : target.value;

    setFormData(prev => ({
      ...prev,
      [name]: value,
    }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    // Logic kiểm tra mật khẩu chỉ áp dụng khi THÊM MỚI
    if (!customer) {
      if (!formData.password || formData.password !== formData.confirmPassword) {
        toast.error('Mật khẩu xác nhận không khớp!');
        return;
      }
    }

    setLoading(true);

    try {
      let result: any = null;

      if (customer) {
        // --- CẬP NHẬT ---
        const updateBody: UpdateCustomerRequest = {
          fullName: formData.fullName,
          email: formData.email,
          phone: formData.phone,
          address: formData.address,
          city: formData.city,
          isActive: formData.isActive,
        };
        result = await dispatch(updateCustomer({ id: customer.id, data: updateBody }));
      } else {
        // --- THÊM MỚI ---
        const createBody = {
          fullName: formData.fullName,
          email: formData.email,
          password: formData.password,
          confirmPassword: formData.confirmPassword,
        };
        result = await dispatch(registerUser(createBody));
      }

      // Xử lý kết quả trả về từ Thunk
      if (result && (updateCustomer.fulfilled.match(result) || registerUser.fulfilled.match(result))) {
        toast.success(customer ? 'Cập nhật thành công!' : 'Thêm mới thành công!');
        dispatch(fetchCustomers({ pageNumber: 1, pageSize: 10 })); 
        onClose();
      } else if (result?.payload) {
        // Lỗi trả về từ rejectWithValue
        const errorMsg = typeof result.payload === 'string' 
          ? result.payload 
          : (result.payload as any)?.message || "Thao tác thất bại";
        toast.error(errorMsg);
      }
    } catch (error) {
      console.error("Form Error:", error);
      toast.error('Lỗi hệ thống, vui lòng thử lại!');
    } finally {
      setLoading(false);
    }
  };

  return (
    <BaseDialog
      isOpen={true}
      title={customer ? 'Cập nhật người dùng' : 'Thêm người dùng mới'}
      onClose={onClose}
    >
      <form onSubmit={handleSubmit}>
        <div className="row">
          <div className="col-md-6 mb-3">
            <label className="form-label fw-bold">Họ và tên</label>
            <input 
              className="form-control" 
              name="fullName" 
              value={formData.fullName} 
              onChange={handleChange} 
              required 
              placeholder="Nhập họ tên..."
            />
          </div>
          <div className="col-md-6 mb-3">
            <label className="form-label fw-bold">Email</label>
            <input 
              type="email" 
              className="form-control" 
              name="email" 
              value={formData.email} 
              onChange={handleChange} 
              required 
              disabled={!!customer} // Thường không cho đổi email khi update để tránh lỗi logic identity
            />
          </div>
        </div>

        {!customer && (
          <div className="row">
            <div className="col-md-6 mb-3">
              <label className="form-label fw-bold">Mật khẩu</label>
              <input 
                type="password" 
                className="form-control" 
                name="password" 
                value={formData.password} 
                onChange={handleChange} 
                required 
              />
            </div>
            <div className="col-md-6 mb-3">
              <label className="form-label fw-bold">Xác nhận mật khẩu</label>
              <input 
                type="password" 
                className="form-control" 
                name="confirmPassword" 
                value={formData.confirmPassword} 
                onChange={handleChange} 
                required 
              />
            </div>
          </div>
        )}

        {customer && (
          <>
            <div className="row">
              <div className="col-md-6 mb-3">
                <label className="form-label fw-bold">Số điện thoại</label>
                <input className="form-control" name="phone" value={formData.phone} onChange={handleChange} />
              </div>
              <div className="col-md-6 mb-3">
                <label className="form-label fw-bold">Thành phố</label>
                <input className="form-control" name="city" value={formData.city} onChange={handleChange} />
              </div>
            </div>
            <div className="mb-3">
              <label className="form-label fw-bold">Địa chỉ</label>
              <input className="form-control" name="address" value={formData.address} onChange={handleChange} />
            </div>
            <div className="mb-3 form-check form-switch">
              <input 
                type="checkbox" 
                className="form-check-input" 
                id="isActive" 
                name="isActive" 
                checked={formData.isActive} 
                onChange={handleChange} 
              />
              <label className="form-check-label ms-2" htmlFor="isActive">Trạng thái hoạt động</label>
            </div>
          </>
        )}

        <div className="d-flex gap-2 justify-content-end mt-4">
          <button type="button" className="btn btn-light border" onClick={onClose} disabled={loading}>
            Đóng
          </button>
          <button type="submit" className="btn btn-primary px-4" disabled={loading}>
            {loading ? (
              <>
                <span className="spinner-border spinner-border-sm me-2"></span>
                Đang lưu...
              </>
            ) : 'Lưu dữ liệu'}
          </button>
        </div>
      </form>
    </BaseDialog>
  );
};

export default UserFormDialog;