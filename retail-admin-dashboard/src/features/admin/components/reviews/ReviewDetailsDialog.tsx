import React from 'react';
import { Review } from '../../../../types/review';
import {BaseDialog} from '../../../../components/ui/BaseDialog';

interface ReviewDetailsDialogProps {
  review: Review;
  onClose: () => void;
}

const ReviewDetailsDialog: React.FC<ReviewDetailsDialogProps> = ({ review, onClose }) => {
  return (
    <BaseDialog
      isOpen={true}
      title="Chi Tiết Đánh Giá"
      onClose={onClose}
    >
      <div className="space-y-3">
        <div className="row mb-3">
          <div className="col-sm-4 fw-bold">ID Đánh Giá:</div>
          <div className="col-sm-8">{review.id}</div>
        </div>
        <div className="row mb-3">
          <div className="col-sm-4 fw-bold">Sản Phẩm:</div>
          <div className="col-sm-8">Sản Phẩm #{review.productId}</div>
        </div>
        <div className="row mb-3">
          <div className="col-sm-4 fw-bold">Người Dùng:</div>
          <div className="col-sm-8">Người Dùng #{review.userId}</div>
        </div>
        <div className="row mb-3">
          <div className="col-sm-4 fw-bold">Đánh Giá:</div>
          <div className="col-sm-8">
            <div className="text-warning">
              {Array.from({ length: review.rating }).map((_, i) => (
                <i key={i} className="bi bi-star-fill"></i>
              ))}
              <span className="text-muted ms-2">({review.rating}/5)</span>
            </div>
          </div>
        </div>
        {review.comment && (
          <div className="row mb-3">
            <div className="col-sm-4 fw-bold">Bình Luận:</div>
            <div className="col-sm-8">{review.comment}</div>
          </div>
        )}
        <div className="row mb-3">
          <div className="col-sm-4 fw-bold">Trạng Thái:</div>
          <div className="col-sm-8">
            <span className={`badge ${
              review.status === 'approved' ? 'bg-success' :
              review.status === 'rejected' ? 'bg-danger' :
              'bg-warning'
            }`}>
              {review.status || 'pending'}
            </span>
          </div>
        </div>
        {review.createdAt && (
          <div className="row mb-3">
            <div className="col-sm-4 fw-bold">Tạo Lúc:</div>
            <div className="col-sm-8">{new Date(review.createdAt).toLocaleString('vi-VN')}</div>
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

export default ReviewDetailsDialog;
