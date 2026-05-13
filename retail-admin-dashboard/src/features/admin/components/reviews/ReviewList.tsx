import React, { useEffect, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { RootState, AppDispatch } from '../../../../store/store';
import { fetchReviews, deleteReview } from '../../../../store/thunks/reviewThunk';
import { Review } from '../../../../types/review';
import toast from 'react-hot-toast';
import ReviewFormDialog from './ReviewFormDialog';
import ReviewDetailsDialog from './ReviewDetailsDialog';

const ReviewList: React.FC = () => {
  const dispatch = useDispatch<AppDispatch>();
  const { items: reviews, loading, error, currentPage } = useSelector(
    (state: RootState) => state.review
  );
  
  const [showFormDialog, setShowFormDialog] = useState(false);
  const [showDetailsDialog, setShowDetailsDialog] = useState(false);
  const [selectedReview, setSelectedReview] = useState<Review | null>(null);
  const pageSize = 10;

  useEffect(() => {
    dispatch(fetchReviews({ page: currentPage, pageSize }));
  }, [dispatch, currentPage]);

  const handleDelete = async (id: string | number) => {
    if (window.confirm('Bạn có chắc chắn muốn xóa đánh giá này?')) {
      const result = await dispatch(deleteReview(id));
      if (deleteReview.fulfilled.match(result)) {
        toast.success('Xóa đánh giá thành công!');
      }
    }
  };

  const handleAddClick = () => {
    setSelectedReview(null);
    setShowFormDialog(true);
  };

  const handleEditClick = (review: Review) => {
    setSelectedReview(review);
    setShowFormDialog(true);
  };

  const handleViewClick = (review: Review) => {
    setSelectedReview(review);
    setShowDetailsDialog(true);
  };

  const handleFormClose = () => {
    setShowFormDialog(false);
    setSelectedReview(null);
  };

  const handleDetailsClose = () => {
    setShowDetailsDialog(false);
    setSelectedReview(null);
  };

  return (
    <div className="container-fluid">
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h1 className="fw-bold mb-0">Quản Lý Đánh Giá</h1>
        <button className="btn btn-success" onClick={handleAddClick}>
          <i className="bi bi-plus-circle me-2"></i>Thêm Đánh Giá
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
                <th>Sản Phẩm</th>
                <th>Người Dùng</th>
                <th>Đánh Giá</th>
                <th>Bình Luận</th>
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
              ) : reviews.length === 0 ? (
                <tr>
                  <td colSpan={7} className="text-center py-4 text-muted">
                    Không có đánh giá nào
                  </td>
                </tr>
              ) : (
                reviews.map((review, index) => (
                  <tr key={review.id}>
                    <td>{(currentPage - 1) * pageSize + index + 1}</td>
                    <td>Sản Phẩm #{review.productId}</td>
                    <td>Người Dùng #{review.userId}</td>
                    <td>
                      <div className="text-warning">
                        {Array.from({ length: review.rating }).map((_, i) => (
                          <i key={i} className="bi bi-star-fill"></i>
                        ))}
                      </div>
                    </td>
                    <td>{review.comment ? review.comment.substring(0, 30) + '...' : 'N/A'}</td>
                    <td>
                      <span className={`badge ${
                        review.status === 'approved' ? 'bg-success' :
                        review.status === 'rejected' ? 'bg-danger' :
                        'bg-warning'
                      }`}>
                        {review.status || 'pending'}
                      </span>
                    </td>
                    <td>
                      <button
                        className="btn btn-sm btn-info me-2"
                        onClick={() => handleViewClick(review)}
                        title="Xem chi tiết"
                      >
                        <i className="bi bi-eye"></i>
                      </button>
                      <button
                        className="btn btn-sm btn-warning me-2"
                        onClick={() => handleEditClick(review)}
                        title="Chỉnh sửa"
                      >
                        <i className="bi bi-pencil"></i>
                      </button>
                      <button
                        className="btn btn-sm btn-danger"
                        onClick={() => handleDelete(review.id)}
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
        <ReviewFormDialog
          review={selectedReview}
          onClose={handleFormClose}
        />
      )}

      {showDetailsDialog && selectedReview && (
        <ReviewDetailsDialog
          review={selectedReview}
          onClose={handleDetailsClose}
        />
      )}
    </div>
  );
};

export default ReviewList;
