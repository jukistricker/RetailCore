// pages/admin/categories/CategoryListPage.tsx
import React, { useEffect, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { CategoryFormDialog } from './CategoryFormDialog';
import { AppDispatch, RootState } from '../../../../store/store';
import { Category } from '../../../../types/category';
import { createCategory, deleteCategory, fetchCategories, updateCategory } from '../../../../store/thunks/categoryThunk';

export const CategoryList = () => {
  const dispatch = useDispatch<AppDispatch>();
  const { items, loading, totalCount } = useSelector((state: RootState) => state.category);
  
  const [params, setParams] = useState({ pageNumber: 1, pageSize: 10, search: '' });
  const [showModal, setShowModal] = useState(false);
  const [selectedItem, setSelectedItem] = useState<Category | null>(null);

  useEffect(() => {
    dispatch(fetchCategories(params));
  }, [dispatch, params]);

  const handleOpenCreate = () => {
    setSelectedItem(null);
    setShowModal(true);
  };

  const handleOpenEdit = (cat: Category) => {
    setSelectedItem(cat);
    setShowModal(true);
  };

  const handleSave = async (data: any) => {
    if (selectedItem) {
      await dispatch(updateCategory(data));
    } else {
      await dispatch(createCategory(data));
    }
    setShowModal(false);
    dispatch(fetchCategories(params)); // Refresh lại danh sách
  };

  const handleDelete = async (id: string) => {
    if (window.confirm("Bạn có chắc chắn muốn xóa danh mục này?")) {
      await dispatch(deleteCategory(id));
      dispatch(fetchCategories(params));
    }
  };

  return (
    <div className="container-fluid p-4">
      <div className="d-flex justify-content-between align-items-center mb-4">
        <div>
          <h3 className="fw-bold mb-0">Quản Lý Danh Mục</h3>
          <p className="text-muted small">Phân loại sản phẩm cho hệ thống của bạn</p>
        </div>
        <button className="btn btn-success shadow-sm" onClick={handleOpenCreate}>
          <i className="bi bi-plus-lg me-2"></i>Thêm Danh Mục
        </button>
      </div>

      <div className="card border-0 shadow-sm rounded-3">
        <div className="card-header py-3">
          <div className="input-group" style={{ maxWidth: '350px' }}>
            <span className="input-group-text  border-end-0">
              <i className="bi bi-search text-muted"></i>
            </span>
            <input 
              type="text" 
              className="form-control  border-start-0" 
              placeholder="Tìm kiếm danh mục..."
              onChange={(e) => setParams({ ...params, search: e.target.value, pageNumber: 1 })}
            />
          </div>
        </div>

        <div className="card-body p-0">
          <div className="table-responsive">
            <table className="table table-hover align-middle mb-0">
              <thead className="table">
                <tr>
                  <th className="ps-4">Thứ tự</th>
                  <th>Tên danh mục</th>
                  <th>Mô tả</th>
                  <th className="text-center">Hành động</th>
                </tr>
              </thead>
              <tbody>
                {loading ? (
                  <tr><td colSpan={4} className="text-center py-5"><div className="spinner-border text-success"></div></td></tr>
                ) : items.map((cat) => (
                  <tr key={cat.id}>
                    <td className="ps-4 fw-bold text-muted">{cat.sortOrder}</td>
                    <td><span className="fw-semibold">{cat.name}</span></td>
                    <td className="text-muted text-truncate" style={{ maxWidth: '300px' }}>
                      {cat.description || '---'}
                    </td>
                    <td className="text-center">
                      <button className="btn btn-sm btn-outline-primary me-2" onClick={() => handleOpenEdit(cat)}>
                        <i className="bi bi-pencil"></i>
                      </button>
                      <button className="btn btn-sm btn-outline-danger" onClick={() => handleDelete(cat.id)}>
                        <i className="bi bi-trash"></i>
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>

        <div className="card-footer  py-3 d-flex justify-content-between align-items-center">
          <small className="text-muted">Tổng cộng: <strong>{totalCount}</strong> danh mục</small>
          <nav>
            <ul className="pagination pagination-sm mb-0">
              <li className={`page-item ${params.pageNumber === 1 ? 'disabled' : ''}`}>
                <button className="page-link" onClick={() => setParams({ ...params, pageNumber: params.pageNumber - 1 })}>Trước</button>
              </li>
              <li className="page-item active"><span className="page-link">{params.pageNumber}</span></li>
              <li className={`page-item ${items.length < params.pageSize ? 'disabled' : ''}`}>
                <button className="page-link" onClick={() => setParams({ ...params, pageNumber: params.pageNumber + 1 })}>Sau</button>
              </li>
            </ul>
          </nav>
        </div>
      </div>

      {showModal && (
        <CategoryFormDialog 
          category={selectedItem} 
          onClose={() => setShowModal(false)} 
          onSave={handleSave} 
        />
      )}
    </div>
  );
};
