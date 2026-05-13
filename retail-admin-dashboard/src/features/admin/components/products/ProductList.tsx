import { useDispatch, useSelector } from "react-redux";
import { AppDispatch, RootState } from "../../../../store/store";
import { useEffect, useState } from "react";
import { createProduct, deleteProduct, fetchProducts, updateProduct } from "../../../../store/thunks/productThunk";
import { setCurrentPage } from "../../../../store/slices/productSlice";
import { Pagination } from "../../../../components/ui/Pagination";
import { Product } from "../../../../types/product";
import { ProductFormDialog } from "./ProductFormDialog";
import { apiBaseURL } from "../../../../lib/axios";

export const ProductList = () => {
  const dispatch = useDispatch<AppDispatch>();
  const { items, totalCount, currentPage, pageSize, totalPage, loading } = useSelector((state: RootState) => state.product);

  const [showDialog, setShowDialog] = useState(false);
  const [selectedProduct, setSelectedProduct] = useState<Product | null>(null);
  // ---------------------

  useEffect(() => {
    dispatch(fetchProducts({ pageNumber: currentPage, pageSize }));
  }, [dispatch, currentPage, pageSize]);

  const handleAddClick = () => {
    setSelectedProduct(null); // Reset về null để hiện form thêm mới
    setShowDialog(true);
  };

  const handleEditClick = (product: Product) => {
    setSelectedProduct(product); // Gán product để hiện form chỉnh sửa
    setShowDialog(true);
  };

  const handleDelete = (id: string) => {
    if (window.confirm('Xóa sản phẩm này?')) {
      dispatch(deleteProduct(id));
    }
  };

  const handleSaveProduct = (data: any) => {
    if (selectedProduct) {
      dispatch(updateProduct({ ...data, id: selectedProduct.id }));
      dispatch(fetchProducts({ pageNumber: currentPage, pageSize }));
    } else {
      dispatch(createProduct(data));
      dispatch(fetchProducts({ pageNumber: currentPage, pageSize }));
    }
    setShowDialog(false);
  };

  return (
    <div className="card shadow-sm border-0">
      <div className="card-header d-flex justify-content-between align-items-center py-3">
        <h5 className="mb-0 fw-bold text-primary">Danh mục sản phẩm</h5>
        {/* Sửa lại hàm gọi ở onClick */}
        <button className="btn btn-primary btn-sm shadow-sm" onClick={handleAddClick}>
          <i className="bi bi-plus-circle me-2"></i>Thêm mới
        </button>
      </div>

      <div className="table-responsive">
        <table className="table table-hover align-middle mb-0">
          <thead className="table">
            <tr>
              <th className="ps-3">Image</th>
              <th>Product Name</th>
              <th>Price</th>
              <th>Stock</th>
              <th className="text-end pe-3">Actions</th>
            </tr>
          </thead>
          <tbody>
            {loading ? (
              <tr>
                <td colSpan={5} className="text-center py-5">
                  <div className="spinner-border text-primary" role="status"></div>
                </td>
              </tr>
            ) : items.length === 0 ? (
              <tr>
                <td colSpan={5} className="text-center py-5 text-muted">No data</td>
              </tr>
            ) : (
              items.map(p => (
                <tr key={p.id}>
                  <td className="ps-3">
                    <img 
                      src={apiBaseURL+"/"+p.thumbnailUrl || 'https://via.placeholder.com/40'} 
                      width="40" 
                      height="40"
                      className="rounded object-fit-cover shadow-sm" 
                      alt={p.name} 
                    />
                  </td>
                  <td>
                    <div className="fw-bold">{p.name}</div>
                    <small className="text-muted">{p.slug}</small>
                  </td>
                  <td>{p.price.toLocaleString()}đ</td>
                  <td>
                    <span className={`badge ${p.stock > 0 ? 'bg-info-subtle text-info' : 'bg-danger-subtle text-danger'}`}>
                      {p.stock} 
                    </span>
                  </td>
                  <td className="text-end pe-3">
                    <button 
                      className="btn btn-sm btn-outline-primary me-2" 
                      onClick={() => handleEditClick(p)}
                    >
                      <i className="bi bi-pencil"></i>
                    </button>
                    <button 
                      className="btn btn-sm btn-outline-danger" 
                      onClick={() => handleDelete(p.id)}
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

      <div className="card-footer bg-transparent border-top py-3">
        <Pagination 
          totalItems={totalCount} 
          itemsPerPage={pageSize} 
          currentPage={currentPage} 
          totalPages={totalPage} 
          onPageChange={(p) => dispatch(setCurrentPage(p))} 
        />
      </div>

      {showDialog && (
        <ProductFormDialog 
          product={selectedProduct} 
          onClose={() => setShowDialog(false)} 
          onSave={handleSaveProduct}
        />
      )}
    </div>
  );

};