import React from 'react';
import { Order } from '../../../../types/order';
import {BaseDialog} from '../../../../components/ui/BaseDialog';

interface OrderDetailsDialogProps {
  order: Order;
  onClose: () => void;
}

const OrderDetailsDialog: React.FC<OrderDetailsDialogProps> = ({ order, onClose }) => {
  return (
    <BaseDialog
      isOpen={true}
      title="Chi Tiết Đơn Hàng"
      onClose={onClose}
    >
      <div className="space-y-3">
        <div className="row mb-3">
          <div className="col-sm-4 fw-bold">Mã Đơn Hàng:</div>
          <div className="col-sm-8">#{order.id}</div>
        </div>
        <div className="row mb-3">
          <div className="col-sm-4 fw-bold">ID Người Dùng:</div>
          <div className="col-sm-8">{order.userId}</div>
        </div>
        <div className="row mb-3">
          <div className="col-sm-4 fw-bold">Số Sản Phẩm:</div>
          <div className="col-sm-8">{order.items?.length || 0}</div>
        </div>
        <div className="row mb-3">
          <div className="col-sm-4 fw-bold">Tổng Tiền:</div>
          <div className="col-sm-8 text-success fw-bold">
            {new Intl.NumberFormat('vi-VN', {
              style: 'currency',
              currency: 'VND',
            }).format(order.totalAmount)}
          </div>
        </div>
        {order.shippingAddress && (
          <div className="row mb-3">
            <div className="col-sm-4 fw-bold">Địa Chỉ Giao:</div>
            <div className="col-sm-8">{order.shippingAddress}</div>
          </div>
        )}
        <div className="row mb-3">
          <div className="col-sm-4 fw-bold">Trạng Thái:</div>
          <div className="col-sm-8">
            <span className={`badge ${
              order.status === 'delivered' ? 'bg-success' :
              order.status === 'shipped' ? 'bg-info' :
              order.status === 'processing' ? 'bg-warning' :
              order.status === 'cancelled' ? 'bg-danger' : 'bg-secondary'
            }`}>
              {order.status || 'pending'}
            </span>
          </div>
        </div>
        {order.notes && (
          <div className="row mb-3">
            <div className="col-sm-4 fw-bold">Ghi Chú:</div>
            <div className="col-sm-8">{order.notes}</div>
          </div>
        )}
        {order.createdAt && (
          <div className="row mb-3">
            <div className="col-sm-4 fw-bold">Tạo Lúc:</div>
            <div className="col-sm-8">{new Date(order.createdAt).toLocaleString('vi-VN')}</div>
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

export default OrderDetailsDialog;
