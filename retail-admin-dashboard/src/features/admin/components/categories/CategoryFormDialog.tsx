// components/categories/CategoryFormDialog.tsx
import React, { useState } from 'react';
import { CreateCategoryRequest, UpdateCategoryRequest, Category } from '../../../../types/category';

interface Props {
  category?: Category | null;
  onClose: () => void;
  onSave: (data: any) => void;
}

export const CategoryFormDialog = ({ category, onClose, onSave }: Props) => {
  const [formData, setFormData] = useState({
    name: category?.name || '',
    description: category?.description || '',
    sortOrder: category?.sortOrder || 0,
  });

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (category) {
      const updateData: UpdateCategoryRequest = { id: category.id, ...formData };
      onSave(updateData);
    } else {
      const createData: CreateCategoryRequest = formData;
      onSave(createData);
    }
  };

  return (
    <div className="modal show d-block" style={{ background: 'rgba(0,0,0,0.5)', zIndex: 1050 }}>
      <div className="modal-dialog modal-dialog-centered">
        <form className="modal-content border-0 shadow-lg" onSubmit={handleSubmit}>
          <div className="modal-header bg-success text-white py-3">
            <h5 className="modal-title fw-bold">
              <i className={`bi ${category ? 'bi-pencil-square' : 'bi-plus-circle'} me-2`}></i>
              {category ? 'Chỉnh Sửa Danh Mục' : 'Thêm Danh Mục Mới'}
            </h5>
            <button type="button" className="btn-close btn-close-white" onClick={onClose}></button>
          </div>
          
          <div className="modal-body p-4">
            <div className="mb-3">
              <label className="form-label fw-semibold">Tên danh mục</label>
              <input 
                type="text" 
                className="form-control shadow-sm" 
                value={formData.name}
                onChange={e => setFormData({ ...formData, name: e.target.value })}
                required 
                placeholder="VD: Điện thoại, Laptop..."
              />
            </div>
            
            <div className="mb-3">
              <label className="form-label fw-semibold">Thứ tự hiển thị</label>
              <input 
                type="number" 
                className="form-control shadow-sm" 
                value={formData.sortOrder}
                onChange={e => setFormData({ ...formData, sortOrder: +e.target.value })}
              />
            </div>

            <div className="mb-0">
              <label className="form-label fw-semibold">Mô tả</label>
              <textarea 
                className="form-control shadow-sm" 
                rows={3}
                value={formData.description}
                onChange={e => setFormData({ ...formData, description: e.target.value })}
                placeholder="Nhập mô tả ngắn về danh mục..."
              ></textarea>
            </div>
          </div>

          <div className="modal-footer py-3">
            <button type="button" className="btn btn-link text-secondary text-decoration-none" onClick={onClose}>Hủy</button>
            <button type="submit" className="btn btn-success px-4 shadow-sm">
              <i className="bi bi-save me-2"></i>Lưu thay đổi
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};