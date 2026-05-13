import React, { useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { RootState, AppDispatch } from '../../../store/store';
import { fetchProducts } from '../../../store/thunks/productThunk';
import { fetchUsers } from '../../../store/thunks/userThunk';
import { fetchOrders } from '../../../store/thunks/orderThunk';
import { fetchCategories } from '../../../store/thunks/categoryThunk';
import { fetchReviews } from '../../../store/thunks/reviewThunk';

const AdminDashboard: React.FC = () => {
  const dispatch = useDispatch<AppDispatch>();
  const { items: products } = useSelector((state: RootState) => state.product);
  const { items: users } = useSelector((state: RootState) => state.user);
  const { items: orders } = useSelector((state: RootState) => state.order);
  const { items: categories } = useSelector((state: RootState) => state.category);
  const { items: reviews } = useSelector((state: RootState) => state.review);
  useEffect(() => {
    dispatch(fetchUsers({ pageNumber: 1, pageSize: 5 }));
    dispatch(fetchProducts({ pageNumber: 1, pageSize: 5 }));
    dispatch(fetchOrders({ page: 1, pageSize: 5 }));
    dispatch(fetchCategories({ pageNumber: 1, pageSize: 5 }));
    dispatch(fetchReviews({ page: 1, pageSize: 5 }));
  }, [dispatch]);

  return (
    <div className="container-fluid">
      <h1 className="mb-4 fw-bold">Admin Dashboard</h1>
      
      <div className="row mb-4">
        <div className="col-md-3 col-6 mb-3">
          <div className="card border-0 shadow-sm">
            <div className="card-body">
              <h6 className="text-muted mb-1">Total Products</h6>
              <h2 className="mb-0 text-primary">{products.length}</h2>
            </div>
          </div>
        </div>
        <div className="col-md-3 col-6 mb-3">
          <div className="card border-0 shadow-sm">
            <div className="card-body">
              <h6 className="text-muted mb-1">Total Users</h6>
              <h2 className="mb-0 text-success">{users.length}</h2>
            </div>
          </div>
        </div>
        <div className="col-md-3 col-6 mb-3">
          <div className="card border-0 shadow-sm">
            <div className="card-body">
              <h6 className="text-muted mb-1">Total Orders</h6>
              <h2 className="mb-0 text-warning">{orders.length}</h2>
            </div>
          </div>
        </div>
        <div className="col-md-3 col-6 mb-3">
          <div className="card border-0 shadow-sm">
            <div className="card-body">
              <h6 className="text-muted mb-1">Total Categories</h6>
              <h2 className="mb-0 text-info">{categories.length}</h2>
            </div>
          </div>
        </div>
      </div>

      <div className="row">
        <div className="col-12">
          <div className="card border-0 shadow-sm">
            <div className="card-header bg-transparent border-bottom py-3">
              <h5 className="mb-0">Recent Reviews</h5>
            </div>
            <div className="card-body">
              {reviews.length > 0 ? (
                <p className="text-muted mb-0">{reviews.length} reviews in total</p>
              ) : (
                <p className="text-muted mb-0">No reviews yet</p>
              )}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default AdminDashboard;
