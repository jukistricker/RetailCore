import React, { useEffect, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { RootState, AppDispatch } from '../../../../store/store';
import { fetchOrders, deleteOrder } from '../../../../store/thunks/orderThunk';
import { Order } from '../../../../types/order';
import toast from 'react-hot-toast';
import OrderFormDialog from './OrderFormDialog';
import OrderDetailsDialog from './OrderDetailsDialog';

const OrderList: React.FC = () => {
  const dispatch = useDispatch<AppDispatch>();
  const { items: orders, loading, error, currentPage } = useSelector(
    (state: RootState) => state.order
  );
  
  const [showFormDialog, setShowFormDialog] = useState(false);
  const [showDetailsDialog, setShowDetailsDialog] = useState(false);
  const [selectedOrder, setSelectedOrder] = useState<Order | null>(null);
  const pageSize = 10;

  useEffect(() => {
    dispatch(fetchOrders({ page: currentPage, pageSize }));
  }, [dispatch, currentPage]);

  const handleDelete = async (id: string | number) => {
    if (window.confirm('Bạn có chắc chắn muốn xóa đơn hàng này?')) {
      const result = await dispatch(deleteOrder(id));
      if (deleteOrder.fulfilled.match(result)) {
        toast.success('Xóa đơn hàng thành công!');
      }
    }
  };

  const handleAddClick = () => {
    setSelectedOrder(null);
    setShowFormDialog(true);
  };

  const handleEditClick = (order: Order) => {
    setSelectedOrder(order);
    setShowFormDialog(true);
  };

  const handleViewClick = (order: Order) => {
    setSelectedOrder(order);
    setShowDetailsDialog(true);
  };

  const handleFormClose = () => {
    setShowFormDialog(false);
    setSelectedOrder(null);
  };

  const handleDetailsClose = () => {
    setShowDetailsDialog(false);
    setSelectedOrder(null);
  };

  return (
    <div className="container-fluid">
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h1 className="fw-bold mb-0">Quản Lý Đơn Hàng</h1>
        <button className="btn btn-success" onClick={handleAddClick}>
          <i className="bi bi-plus-circle me-2"></i>Thêm Đơn Hàng
        </button>
      </div>

      {error && (
        <div className="alert alert-danger" role="alert">
          {error}
        </div>
      )}

      <div className="card border-0 shadow-sm">
        <div className="table-responsive">
          <table className="table table-hover mb-0">
            <thead className="bg-light">
              <tr>
                <th>STT</th>
                <th>Mã Đơn Hàng</th>
                <th>Người Dùng</th>
                <th>Số Lượng Sản Phẩm</th>
                <th>Tổng Tiền</th>
                <th>Trạng Thái</th>
                <th>Thao Tác</th>
              </tr>
            </thead>
            <tbody>
              {loading ? (
                <tr>
                  <td colSpan={7} className="text-center py-4">
                    <div className="spinner-border text-primary" role="status">
                      <span className="visually-hidden">Loading...</span>
                    </div>
                  </td>
                </tr>
              ) : orders.length === 0 ? (
                <tr>
                  <td colSpan={7} className="text-center py-4 text-muted">
                    Không có đơn hàng nào
                  </td>
                </tr>
              ) : (
                orders.map((order, index) => (
                  <tr key={order.id}>
                    <td>{(currentPage - 1) * pageSize + index + 1}</td>
                    <td>
                      <strong>#{order.id}</strong>
                    </td>
                    <td>{order.userId}</td>
                    <td>{order.items?.length || 0}</td>
                    <td className="text-success fw-bold">
                      {new Intl.NumberFormat('vi-VN', {
                        style: 'currency',
                        currency: 'VND',
                      }).format(order.totalAmount)}
                    </td>
                    <td>
                      <span className={`badge ${
                        order.status === 'delivered' ? 'bg-success' :
                        order.status === 'shipped' ? 'bg-info' :
                        order.status === 'processing' ? 'bg-warning' :
                        order.status === 'cancelled' ? 'bg-danger' : 'bg-secondary'
                      }`}>
                        {order.status || 'pending'}
                      </span>
                    </td>
                    <td>
                      <button
                        className="btn btn-sm btn-info me-2"
                        onClick={() => handleViewClick(order)}
                        title="Xem chi tiết"
                      >
                        <i className="bi bi-eye"></i>
                      </button>
                      <button
                        className="btn btn-sm btn-warning me-2"
                        onClick={() => handleEditClick(order)}
                        title="Chỉnh sửa"
                      >
                        <i className="bi bi-pencil"></i>
                      </button>
                      <button
                        className="btn btn-sm btn-danger"
                        onClick={() => handleDelete(order.id)}
                        title="Xóa"
                      >
                        <i className="bi bi-trash"></i>
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
        <OrderFormDialog
          order={selectedOrder}
          onClose={handleFormClose}
        />
      )}

      {showDetailsDialog && selectedOrder && (
        <OrderDetailsDialog
          order={selectedOrder}
          onClose={handleDetailsClose}
        />
      )}
    </div>
  );
};

export default OrderList;
