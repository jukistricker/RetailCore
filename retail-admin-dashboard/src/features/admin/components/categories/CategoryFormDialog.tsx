import React, { useState, useEffect } from 'react';
import { useDispatch } from 'react-redux';
import { AppDispatch } from '../../../../store/store';
import { createCategory, updateCategory } from '../../../../store/thunks/categoryThunk';
import { Category, CreateCategoryRequest } from '../../../../types/category';
import {BaseDialog} from '../../../../components/ui/BaseDialog';
import toast from 'react-hot-toast';

interface CategoryFormDialogProps {
  category: Category | null;
  onClose: () => void;
}

const CategoryFormDialog: React.FC<CategoryFormDialogProps> = ({ category, onClose }) => {
  const dispatch = useDispatch<AppDispatch>();
  const [formData, setFormData] = useState<CreateCategoryRequest>({
    name: '',
    description: '',
    image: '',
  });
  const [status, setStatus] = useState(category?.status || 'active');
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (category) {
      setFormData({
        name: category.name,
        description: category.description || '',
        image: category.image || '',
      });
      setStatus(category.status || 'active');
    }
  }, [category]);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
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
      if (category) {
        const result = await dispatch(updateCategory({ ...formData, id: category.id, status: status as any }));
        if (updateCategory.fulfilled.match(result)) {
          toast.success('Cập nhật danh mục thành công!');
          onClose();
        }
      } else {
        const result = await dispatch(createCategory(formData));
        if (createCategory.fulfilled.match(result)) {
          toast.success('Thêm danh mục thành công!');
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
      title={category ? 'Chỉnh Sửa Danh Mục' : 'Thêm Danh Mục Mới'}
      onClose={onClose}
    >
      <form onSubmit={handleSubmit}>
        <div className="mb-3">
          <label htmlFor="name" className="form-label">
            Tên Danh Mục <span className="text-danger">*</span>
          </label>
          <input
            type="text"
            className="form-control"
            id="name"
            name="name"
            value={formData.name}
            onChange={handleChange}
            required
          />
        </div>

        <div className="mb-3">
          <label htmlFor="description" className="form-label">
            Mô Tả
          </label>
          <textarea
            className="form-control"
            id="description"
            name="description"
            rows={3}
            value={formData.description}
            onChange={handleChange}
          ></textarea>
        </div>

        <div className="mb-3">
          <label htmlFor="image" className="form-label">
            URL Hình Ảnh
          </label>
          <input
            type="url"
            className="form-control"
            id="image"
            name="image"
            value={formData.image}
            onChange={handleChange}
          />
        </div>

        {category && (
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
              <option value="active">Hoạt Động</option>
              <option value="inactive">Không Hoạt Động</option>
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

export default CategoryFormDialog;
