import React, { useState, useEffect } from 'react';
import { useDispatch } from 'react-redux';
import { AppDispatch } from '../../../../store/store';
import { createOrder, updateOrder } from '../../../../store/thunks/orderThunk';
import { Order, CreateOrderRequest } from '../../../../types/order';
import {BaseDialog} from '../../../../components/ui/BaseDialog';
import toast from 'react-hot-toast';

interface OrderFormDialogProps {
  order: Order | null;
  onClose: () => void;
}

const OrderFormDialog: React.FC<OrderFormDialogProps> = ({ order, onClose }) => {
  const dispatch = useDispatch<AppDispatch>();
  const [formData, setFormData] = useState<CreateOrderRequest>({
    userId: '',
    items: [],
    shippingAddress: '',
    notes: '',
  });
  const [loading, setLoading] = useState(false);
  const [status, setStatus] = useState(order?.status || 'pending');

  useEffect(() => {
    if (order) {
      setFormData({
        userId: order.userId,
        items: order.items || [],
        shippingAddress: order.shippingAddress || '',
        notes: order.notes || '',
      });
      setStatus(order.status || 'pending');
    }
  }, [order]);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>) => {
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
      if (order) {
        const result = await dispatch(updateOrder({ ...formData, id: order.id, status: status as any }));
        if (updateOrder.fulfilled.match(result)) {
          toast.success('Cập nhật đơn hàng thành công!');
          onClose();
        }
      } else {
        const result = await dispatch(createOrder(formData));
        if (createOrder.fulfilled.match(result)) {
          toast.success('Thêm đơn hàng thành công!');
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
      title={order ? 'Chỉnh Sửa Đơn Hàng' : 'Thêm Đơn Hàng Mới'}
      onClose={onClose}
    >
      <form onSubmit={handleSubmit}>
        <div className="mb-3">
          <label htmlFor="userId" className="form-label">
            ID Người Dùng <span className="text-danger">*</span>
          </label>
          <input
            type="text"
            className="form-control"
            id="userId"
            name="userId"
            value={formData.userId}
            onChange={handleChange}
            required
          />
        </div>

        <div className="mb-3">
          <label htmlFor="shippingAddress" className="form-label">
            Địa Chỉ Giao Hàng
          </label>
          <input
            type="text"
            className="form-control"
            id="shippingAddress"
            name="shippingAddress"
            value={formData.shippingAddress}
            onChange={handleChange}
          />
        </div>

        <div className="mb-3">
          <label htmlFor="notes" className="form-label">
            Ghi Chú
          </label>
          <textarea
            className="form-control"
            id="notes"
            name="notes"
            rows={3}
            value={formData.notes}
            onChange={handleChange}
          ></textarea>
        </div>

        {order && (
          <div className="mb-3">
            <label htmlFor="status" className="form-label">
              Trạng Thái <span className="text-danger">*</span>
            </label>
            <select
              className="form-select"
              id="status"
              value={status}
              onChange={(e) => setStatus(e.target.value)}
              required
            >
              <option value="pending">Chờ Xử Lý</option>
              <option value="processing">Đang Xử Lý</option>
              <option value="shipped">Đã Gửi</option>
              <option value="delivered">Đã Giao</option>
              <option value="cancelled">Hủy</option>
            </select>
          </div>
        )}

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

export default OrderFormDialog;
