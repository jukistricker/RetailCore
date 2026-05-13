import React from 'react';
import { Category } from '../../../../types/category';
import {BaseDialog} from '../../../../components/ui/BaseDialog';

interface CategoryDetailsDialogProps {
  category: Category;
  onClose: () => void;
}

const CategoryDetailsDialog: React.FC<CategoryDetailsDialogProps> = ({ category, onClose }) => {
  return (
    <BaseDialog
      isOpen={true}
      title="Chi Tiết Danh Mục"
      onClose={onClose}
    >
      <div className="space-y-3">
        <div className="row mb-3">
          <div className="col-sm-4 fw-bold">ID:</div>
          <div className="col-sm-8">{category.id}</div>
        </div>
        <div className="row mb-3">
          <div className="col-sm-4 fw-bold">Tên:</div>
          <div className="col-sm-8">{category.name}</div>
        </div>
        {category.description && (
          <div className="row mb-3">
            <div className="col-sm-4 fw-bold">Mô Tả:</div>
            <div className="col-sm-8">{category.description}</div>
          </div>
        )}
        {category.image && (
          <div className="row mb-3">
            <div className="col-sm-4 fw-bold">Hình Ảnh:</div>
            <div className="col-sm-8">
              <a href={category.image} target="_blank" rel="noopener noreferrer" className="text-decoration-none">
                Xem Ảnh
              </a>
            </div>
          </div>
        )}
        <div className="row mb-3">
          <div className="col-sm-4 fw-bold">Trạng Thái:</div>
          <div className="col-sm-8">
            <span className={`badge ${category.status === 'active' ? 'bg-success' : 'bg-danger'}`}>
              {category.status || 'active'}
            </span>
          </div>
        </div>
        {category.createdAt && (
          <div className="row mb-3">
            <div className="col-sm-4 fw-bold">Tạo Lúc:</div>
            <div className="col-sm-8">{new Date(category.createdAt).toLocaleString('vi-VN')}</div>
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

export default CategoryDetailsDialog;
