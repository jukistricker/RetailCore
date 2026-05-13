import React, { useEffect, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { RootState, AppDispatch } from '../../../../store/store';
import { fetchProducts, deleteProduct } from '../../../../store/thunks/productThunk';
import { Product } from '../../../../types/product';
import toast from 'react-hot-toast';
import ProductFormDialog from './ProductFormDialog';
import ProductDetailsDialog from './ProductDetailsDialog';

const ProductList: React.FC = () => {
  const dispatch = useDispatch<AppDispatch>();
  const { items: products, loading, error, currentPage, total } = useSelector(
    (state: RootState) => state.product
  );
  
  const [showFormDialog, setShowFormDialog] = useState(false);
  const [showDetailsDialog, setShowDetailsDialog] = useState(false);
  const [selectedProduct, setSelectedProduct] = useState<Product | null>(null);
  const pageSize = 10;

  useEffect(() => {
    dispatch(fetchProducts({ page: currentPage, pageSize }));
  }, [dispatch, currentPage]);

  const handleDelete = async (id: string | number) => {
    if (window.confirm('Bạn có chắc chắn muốn xóa sản phẩm này?')) {
      const result = await dispatch(deleteProduct(id));
      if (deleteProduct.fulfilled.match(result)) {
        toast.success('Xóa sản phẩm thành công!');
      }
    }
  };

  const handleAddClick = () => {
    setSelectedProduct(null);
    setShowFormDialog(true);
  };

  const handleEditClick = (product: Product) => {
    setSelectedProduct(product);
    setShowFormDialog(true);
  };

  const handleViewClick = (product: Product) => {
    setSelectedProduct(product);
    setShowDetailsDialog(true);
  };

  const handleFormClose = () => {
    setShowFormDialog(false);
    setSelectedProduct(null);
  };

  const handleDetailsClose = () => {
    setShowDetailsDialog(false);
    setSelectedProduct(null);
  };

  return (
    <div className="container-fluid">
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h1 className="fw-bold mb-0">Quản Lý Sản Phẩm</h1>
        <button className="btn btn-success" onClick={handleAddClick}>
          <i className="bi bi-plus-circle me-2"></i>Thêm Sản Phẩm
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
                <th>Tên Sản Phẩm</th>
                <th>Giá</th>
                <th>Số Lượng Tồn</th>
                <th>Danh Mục</th>
                <th>Thao Tác</th>
              </tr>
            </thead>
            <tbody>
              {loading ? (
                <tr>
                  <td colSpan={6} className="text-center py-4">
                    <div className="spinner-border text-primary" role="status">
                      <span className="visually-hidden">Loading...</span>
                    </div>
                  </td>
                </tr>
              ) : products.length === 0 ? (
                <tr>
                  <td colSpan={6} className="text-center py-4 text-muted">
                    Không có sản phẩm nào
                  </td>
                </tr>
              ) : (
                products.map((product, index) => (
                  <tr key={product.id}>
                    <td>{(currentPage - 1) * pageSize + index + 1}</td>
                    <td>
                      <strong>{product.name}</strong>
                    </td>
                    <td className="text-success fw-bold">
                      {new Intl.NumberFormat('vi-VN', {
                        style: 'currency',
                        currency: 'VND',
                      }).format(product.price)}
                    </td>
                    <td>{product.stock}</td>
                    <td>{product.categoryId}</td>
                    <td>
                      <button
                        className="btn btn-sm btn-info me-2"
                        onClick={() => handleViewClick(product)}
                        title="Xem chi tiết"
                      >
                        <i className="bi bi-eye"></i>
                      </button>
                      <button
                        className="btn btn-sm btn-warning me-2"
                        onClick={() => handleEditClick(product)}
                        title="Chỉnh sửa"
                      >
                        <i className="bi bi-pencil"></i>
                      </button>
                      <button
                        className="btn btn-sm btn-danger"
                        onClick={() => handleDelete(product.id)}
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
        <ProductFormDialog
          product={selectedProduct}
          onClose={handleFormClose}
        />
      )}

      {showDetailsDialog && selectedProduct && (
        <ProductDetailsDialog
          product={selectedProduct}
          onClose={handleDetailsClose}
        />
      )}
    </div>
  );
};

export default ProductList;
