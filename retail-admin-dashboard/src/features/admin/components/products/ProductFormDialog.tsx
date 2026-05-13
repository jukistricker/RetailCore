import React, { useState, useEffect } from 'react';
import { useDispatch } from 'react-redux';
import { AppDispatch } from '../../../../store/store';
import { createProduct, updateProduct } from '../../../../store/thunks/productThunk';
import { Product, CreateProductRequest } from '../../../../types/product';
import {BaseDialog} from '../../../../components/ui/BaseDialog';
import toast from 'react-hot-toast';

interface ProductFormDialogProps {
  product: Product | null;
  onClose: () => void;
}

const ProductFormDialog: React.FC<ProductFormDialogProps> = ({ product, onClose }) => {
  const dispatch = useDispatch<AppDispatch>();
  const [formData, setFormData] = useState<CreateProductRequest>({
    name: '',
    description: '',
    price: 0,
    stock: 0,
    categoryId: '',
    sku: '',
  });
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (product) {
      setFormData({
        name: product.name,
        description: product.description,
        price: product.price,
        stock: product.stock,
        categoryId: product.categoryId,
        sku: product.sku || '',
      });
    }
  }, [product]);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: name === 'price' || name === 'stock' ? parseFloat(value) : value,
    }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);

    try {
      if (product) {
        const result = await dispatch(updateProduct({ ...formData, id: product.id }));
        if (updateProduct.fulfilled.match(result)) {
          toast.success('Cập nhật sản phẩm thành công!');
          onClose();
        }
      } else {
        const result = await dispatch(createProduct(formData));
        if (createProduct.fulfilled.match(result)) {
          toast.success('Thêm sản phẩm thành công!');
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
      title={product ? 'Chỉnh Sửa Sản Phẩm' : 'Thêm Sản Phẩm Mới'}
      onClose={onClose}
    >
      <form onSubmit={handleSubmit}>
        <div className="mb-3">
          <label htmlFor="name" className="form-label">
            Tên Sản Phẩm <span className="text-danger">*</span>
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

        <div className="row">
          <div className="col-md-6 mb-3">
            <label htmlFor="price" className="form-label">
              Giá <span className="text-danger">*</span>
            </label>
            <input
              type="number"
              className="form-control"
              id="price"
              name="price"
              value={formData.price}
              onChange={handleChange}
              step="0.01"
              required
            />
          </div>
          <div className="col-md-6 mb-3">
            <label htmlFor="stock" className="form-label">
              Số Lượng Tồn <span className="text-danger">*</span>
            </label>
            <input
              type="number"
              className="form-control"
              id="stock"
              name="stock"
              value={formData.stock}
              onChange={handleChange}
              required
            />
          </div>
        </div>

        <div className="row">
          <div className="col-md-6 mb-3">
            <label htmlFor="categoryId" className="form-label">
              Danh Mục <span className="text-danger">*</span>
            </label>
            <input
              type="text"
              className="form-control"
              id="categoryId"
              name="categoryId"
              value={formData.categoryId}
              onChange={handleChange}
              required
            />
          </div>
          <div className="col-md-6 mb-3">
            <label htmlFor="sku" className="form-label">
              SKU
            </label>
            <input
              type="text"
              className="form-control"
              id="sku"
              name="sku"
              value={formData.sku}
              onChange={handleChange}
            />
          </div>
        </div>

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

export default ProductFormDialog;
