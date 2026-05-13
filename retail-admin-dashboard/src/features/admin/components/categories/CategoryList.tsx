import React, { useEffect, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { RootState, AppDispatch } from '../../../../store/store';
import { fetchCategories, deleteCategory } from '../../../../store/thunks/categoryThunk';
import { Category } from '../../../../types/category';
import toast from 'react-hot-toast';
import CategoryFormDialog from './CategoryFormDialog';
import CategoryDetailsDialog from './CategoryDetailsDialog';

const CategoryList: React.FC = () => {
  const dispatch = useDispatch<AppDispatch>();
  const { items: categories, loading, error, currentPage } = useSelector(
    (state: RootState) => state.category
  );
  
  const [showFormDialog, setShowFormDialog] = useState(false);
  const [showDetailsDialog, setShowDetailsDialog] = useState(false);
  const [selectedCategory, setSelectedCategory] = useState<Category | null>(null);
  const pageSize = 10;

  useEffect(() => {
    dispatch(fetchCategories({ page: currentPage, pageSize }));
  }, [dispatch, currentPage]);

  const handleDelete = async (id: string | number) => {
    if (window.confirm('Bạn có chắc chắn muốn xóa danh mục này?')) {
      const result = await dispatch(deleteCategory(id));
      if (deleteCategory.fulfilled.match(result)) {
        toast.success('Xóa danh mục thành công!');
      }
    }
  };

  const handleAddClick = () => {
    setSelectedCategory(null);
    setShowFormDialog(true);
  };

  const handleEditClick = (category: Category) => {
    setSelectedCategory(category);
    setShowFormDialog(true);
  };

  const handleViewClick = (category: Category) => {
    setSelectedCategory(category);
    setShowDetailsDialog(true);
  };

  const handleFormClose = () => {
    setShowFormDialog(false);
    setSelectedCategory(null);
  };

  const handleDetailsClose = () => {
    setShowDetailsDialog(false);
    setSelectedCategory(null);
  };

  return (
    <div className="container-fluid">
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h1 className="fw-bold mb-0">Quản Lý Danh Mục</h1>
        <button className="btn btn-success" onClick={handleAddClick}>
          <i className="bi bi-plus-circle me-2"></i>Thêm Danh Mục
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
                <th>Tên Danh Mục</th>
                <th>Mô Tả</th>
                <th>Trạng Thái</th>
                <th>Thao Tác</th>
              </tr>
            </thead>
            <tbody>
              {loading ? (
                <tr>
                  <td colSpan={5} className="text-center py-4">
                    <div className="spinner-border text-primary" role="status">
                      <span className="visually-hidden">Loading...</span>
                    </div>
                  </td>
                </tr>
              ) : categories.length === 0 ? (
                <tr>
                  <td colSpan={5} className="text-center py-4 text-muted">
                    Không có danh mục nào
                  </td>
                </tr>
              ) : (
                categories.map((category, index) => (
                  <tr key={category.id}>
                    <td>{(currentPage - 1) * pageSize + index + 1}</td>
                    <td>
                      <strong>{category.name}</strong>
                    </td>
                    <td>{category.description || 'N/A'}</td>
                    <td>
                      <span className={`badge ${category.status === 'active' ? 'bg-success' : 'bg-danger'}`}>
                        {category.status || 'active'}
                      </span>
                    </td>
                    <td>
                      <button
                        className="btn btn-sm btn-info me-2"
                        onClick={() => handleViewClick(category)}
                        title="Xem chi tiết"
                      >
                        <i className="bi bi-eye"></i>
                      </button>
                      <button
                        className="btn btn-sm btn-warning me-2"
                        onClick={() => handleEditClick(category)}
                        title="Chỉnh sửa"
                      >
                        <i className="bi bi-pencil"></i>
                      </button>
                      <button
                        className="btn btn-sm btn-danger"
                        onClick={() => handleDelete(category.id)}
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
        <CategoryFormDialog
          category={selectedCategory}
          onClose={handleFormClose}
        />
      )}

      {showDetailsDialog && selectedCategory && (
        <CategoryDetailsDialog
          category={selectedCategory}
          onClose={handleDetailsClose}
        />
      )}
    </div>
  );
};

export default CategoryList;
