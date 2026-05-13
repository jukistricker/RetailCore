import React from 'react';
import { Product } from '../../../../types/product';
import {BaseDialog} from '../../../../components/ui/BaseDialog';

interface ProductDetailsDialogProps {
  product: Product;
  onClose: () => void;
}

const ProductDetailsDialog: React.FC<ProductDetailsDialogProps> = ({ product, onClose }) => {
  return (
    <BaseDialog
      isOpen={true}
      title="Chi Tiết Sản Phẩm"
      onClose={onClose}
    >
      <div className="space-y-3">
        <div className="row mb-3">
          <div className="col-sm-4 fw-bold">ID:</div>
          <div className="col-sm-8">{product.id}</div>
        </div>
        <div className="row mb-3">
          <div className="col-sm-4 fw-bold">Tên:</div>
          <div className="col-sm-8">{product.name}</div>
        </div>
        <div className="row mb-3">
          <div className="col-sm-4 fw-bold">Mô Tả:</div>
          <div className="col-sm-8">{product.description || 'N/A'}</div>
        </div>
        <div className="row mb-3">
          <div className="col-sm-4 fw-bold">Giá:</div>
          <div className="col-sm-8 text-success fw-bold">
            {new Intl.NumberFormat('vi-VN', {
              style: 'currency',
              currency: 'VND',
            }).format(product.price)}
          </div>
        </div>
        <div className="row mb-3">
          <div className="col-sm-4 fw-bold">Số Lượng Tồn:</div>
          <div className="col-sm-8">{product.stock}</div>
        </div>
        <div className="row mb-3">
          <div className="col-sm-4 fw-bold">Danh Mục:</div>
          <div className="col-sm-8">{product.categoryId}</div>
        </div>
        {product.sku && (
          <div className="row mb-3">
            <div className="col-sm-4 fw-bold">SKU:</div>
            <div className="col-sm-8">{product.sku}</div>
          </div>
        )}
        {product.createdAt && (
          <div className="row mb-3">
            <div className="col-sm-4 fw-bold">Tạo Lúc:</div>
            <div className="col-sm-8">{new Date(product.createdAt).toLocaleString('vi-VN')}</div>
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

export default ProductDetailsDialog;
