import React, { useState, useEffect } from 'react';
import { useDispatch } from 'react-redux';
import { AppDispatch } from '../../../../store/store';
import { createReview, updateReview } from '../../../../store/thunks/reviewThunk';
import { Review, CreateReviewRequest } from '../../../../types/review';
import {BaseDialog} from '../../../../components/ui/BaseDialog';
import toast from 'react-hot-toast';

interface ReviewFormDialogProps {
  review: Review | null;
  onClose: () => void;
}

const ReviewFormDialog: React.FC<ReviewFormDialogProps> = ({ review, onClose }) => {
  const dispatch = useDispatch<AppDispatch>();
  const [formData, setFormData] = useState<CreateReviewRequest>({
    productId: '',
    userId: '',
    rating: 5,
    comment: '',
  });
  const [status, setStatus] = useState(review?.status || 'pending');
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (review) {
      setFormData({
        productId: review.productId,
        userId: review.userId,
        rating: review.rating,
        comment: review.comment || '',
      });
      setStatus(review.status || 'pending');
    }
  }, [review]);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: name === 'rating' ? parseInt(value) : value,
    }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);

    try {
      if (review) {
        const result = await dispatch(updateReview({ ...formData, id: review.id, status: status as any }));
        if (updateReview.fulfilled.match(result)) {
          toast.success('Cập nhật đánh giá thành công!');
          onClose();
        }
      } else {
        const result = await dispatch(createReview(formData));
        if (createReview.fulfilled.match(result)) {
          toast.success('Thêm đánh giá thành công!');
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
      title={review ? 'Chỉnh Sửa Đánh Giá' : 'Thêm Đánh Giá Mới'}
      onClose={onClose}
    >
      <form onSubmit={handleSubmit}>
        <div className="row mb-3">
          <div className="col-md-6">
            <label htmlFor="productId" className="form-label">
              ID Sản Phẩm <span className="text-danger">*</span>
            </label>
            <input
              type="text"
              className="form-control"
              id="productId"
              name="productId"
              value={formData.productId}
              onChange={handleChange}
              required
            />
          </div>
          <div className="col-md-6">
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
        </div>

        <div className="mb-3">
          <label htmlFor="rating" className="form-label">
            Đánh Giá (1-5 Sao) <span className="text-danger">*</span>
          </label>
          <select
            className="form-select"
            id="rating"
            name="rating"
            value={formData.rating}
            onChange={handleChange}
            required
          >
            <option value={1}>1 Sao</option>
            <option value={2}>2 Sao</option>
            <option value={3}>3 Sao</option>
            <option value={4}>4 Sao</option>
            <option value={5}>5 Sao</option>
          </select>
        </div>

        <div className="mb-3">
          <label htmlFor="comment" className="form-label">
            Bình Luận
          </label>
          <textarea
            className="form-control"
            id="comment"
            name="comment"
            rows={3}
            value={formData.comment}
            onChange={handleChange}
          ></textarea>
        </div>

        {review && (
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
              <option value="pending">Chờ Duyệt</option>
              <option value="approved">Phê Duyệt</option>
              <option value="rejected">Từ Chối</option>
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

export default ReviewFormDialog;
