// components/products/ProductFormDialog.tsx
import React, { useState, useEffect, useRef } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { AppDispatch, RootState } from '../../../../store/store';
import { fetchCategories } from '../../../../store/thunks/categoryThunk';
import { ProductAttributeManager } from './ProductAttributeManager';

export const ProductFormDialog = ({ product, onClose, onSave }: any) => {
  const dispatch = useDispatch<AppDispatch>();
  const { items: categories, loading: categoryLoading } = useSelector((state: RootState) => state.category);

  const [formData, setFormData] = useState({
    name: product?.name || '',
    slug: product?.slug || '',
    price: product?.price || 0,
    stock: product?.stock || 0,
    isFeatured: product?.isFeatured || false, // Thêm field mới
    categoryId: product?.categoryId || '',
    attributes: product?.attributes || '[]'
  });

  const [selectedFiles, setSelectedFiles] = useState<File[]>([]);
  const [previews, setPreviews] = useState<string[]>([]);
  
  const [searchTerm, setSearchTerm] = useState('');
  const [showDropdown, setShowDropdown] = useState(false);
  const dropdownRef = useRef<HTMLDivElement>(null);

  const generateSlug = (text: string) => {
    return text
      .toLowerCase()
      .normalize('NFD')
      .replace(/[\u0300-\u036f]/g, '')
      .replace(/[^\w\s-]/g, '')
      .replace(/[\s_]+/g, '-')
      .replace(/^-+|-+$/g, '');
  };

  const handleNameChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const name = e.target.value;
    setFormData(prev => ({
      ...prev,
      name,
      slug: product ? prev.slug : generateSlug(name)
    }));
  };

  useEffect(() => {
    const delayDebounceFn = setTimeout(() => {
      dispatch(fetchCategories({ pageNumber: 1, pageSize: 10, search: searchTerm }));
    }, 400);

    return () => clearTimeout(delayDebounceFn);
  }, [searchTerm, dispatch]);

  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (dropdownRef.current && !dropdownRef.current.contains(event.target as Node)) {
        setShowDropdown(false);
      }
    };
    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files) {
      const filesArray = Array.from(e.target.files);
      setSelectedFiles(filesArray);
      const newPreviews = filesArray.map(file => URL.createObjectURL(file));
      setPreviews(newPreviews);
    }
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (!formData.categoryId) {
      alert("Vui lòng chọn danh mục!");
      return;
    }
    // Payload tự động bao gồm isFeatured từ formData
    onSave({ ...formData, images: selectedFiles });
  };

  const selectedCategory = categories.find(c => c.id === formData.categoryId);

  return (
    <div className="modal show d-block" style={{ background: 'rgba(0,0,0,0.5)', zIndex: 1050 }}>
      <div className="modal-dialog modal-xl modal-dialog-centered">
        <form className="modal-content shadow-lg border-0" onSubmit={handleSubmit}>
          {/* Header */}
          <div className="modal-header bg-primary text-white py-3">
            <h5 className="modal-title fw-bold">
              <i className="bi bi-box-seam me-2"></i>
              {product ? 'Cập Nhật Sản Phẩm' : 'Thêm Sản Phẩm Mới'}
            </h5>
            <button type="button" className="btn-close btn-close-white" onClick={onClose}></button>
          </div>

          <div className="modal-body p-4" style={{ maxHeight: '75vh', overflowY: 'auto' }}>
            <div className="row g-3">
              <div className="col-md-6">
                <label className="form-label fw-semibold">Tên sản phẩm</label>
                <input className="form-control" value={formData.name} required
                  onChange={handleNameChange} placeholder="Ví dụ: iPhone 15 Pro Max..." />
              </div>

              <div className="col-md-6">
                <label className="form-label fw-semibold">Slug (Đường dẫn)</label>
                <input className="form-control" value={formData.slug} required
                  onChange={e => setFormData({ ...formData, slug: e.target.value })} />
              </div>
              
              <div className="col-md-6 position-relative" ref={dropdownRef}>
                <label className="form-label fw-semibold">Danh mục</label>
                <div className="input-group">
                  <span className="input-group-text"><i className="bi bi-search"></i></span>
                  <input 
                    type="text" 
                    className="form-control" 
                    placeholder={selectedCategory ? `Đang chọn: ${selectedCategory.name}` : "Tìm kiếm danh mục..."}
                    value={searchTerm}
                    onFocus={() => setShowDropdown(true)}
                    onChange={(e) => setSearchTerm(e.target.value)}
                  />
                </div>
                {/* Dropdown menu giữ nguyên... */}
                {showDropdown && (
                   <div className="list-group position-absolute w-100 shadow-lg mt-1" style={{ zIndex: 1100, maxHeight: '200px', overflowY: 'auto' }}>
                    {categoryLoading ? (
                      <div className="list-group-item text-center"><div className="spinner-border spinner-border-sm"></div></div>
                    ) : categories.length > 0 ? (
                      categories.map(cat => (
                        <button key={cat.id} type="button"
                          className={`list-group-item list-group-item-action ${formData.categoryId === cat.id ? 'active' : ''}`}
                          onClick={() => {
                            setFormData({ ...formData, categoryId: cat.id });
                            setShowDropdown(false);
                            setSearchTerm('');
                          }}
                        >
                          {cat.name}
                        </button>
                      ))
                    ) : (
                      <div className="list-group-item text-muted">Không tìm thấy danh mục</div>
                    )}
                  </div>
                )}
              </div>

              <div className="col-md-2">
                <label className="form-label fw-semibold">Giá gốc</label>
                <div className="input-group">
                  <input type="number" className="form-control" value={formData.price}
                    onChange={e => setFormData({ ...formData, price: +e.target.value })} />
                  <span className="input-group-text">đ</span>
                </div>
              </div>

              <div className="col-md-2">
                <label className="form-label fw-semibold">Kho hàng</label>
                <input type="number" className="form-control" value={formData.stock}
                  onChange={e => setFormData({ ...formData, stock: +e.target.value })} />
              </div>

              {/* Dấu tích chọn isFeatured */}
              <div className="col-md-2 d-flex align-items-end pb-2">
                <div className="form-check form-switch border rounded p-2 w-100 d-flex align-items-center justify-content-center">
                  <input 
                    className="form-check-input me-2" 
                    type="checkbox" 
                    id="isFeatured" 
                    checked={formData.isFeatured}
                    onChange={e => setFormData({ ...formData, isFeatured: e.target.checked })}
                    style={{ cursor: 'pointer', width: '2.5em', height: '1.25em' }}
                  />
                  <label className="form-check-label fw-bold text-primary mb-0" htmlFor="isFeatured" style={{ cursor: 'pointer' }}>
                    Nổi bật
                  </label>
                </div>
              </div>

              {/* Ảnh sản phẩm và Attribute Manager giữ nguyên... */}
              <div className="col-md-12 my-3">
                <label className="form-label fw-semibold d-block">Ảnh sản phẩm</label>
                <div className="upload-container border border-dashed rounded p-3 text-center">
                  <input type="file" multiple id="productImages" className="form-control d-none" accept="image/*" onChange={handleFileChange} />
                  <label htmlFor="productImages" className="btn btn-outline-primary mb-2">
                    <i className="bi bi-cloud-arrow-up me-2"></i>Chọn ảnh từ thiết bị
                  </label>
                  <div className="d-flex flex-wrap gap-2 mt-3 justify-content-center">
                    {previews.map((url, index) => (
                      <div key={index} className="position-relative">
                        <img src={url} className="rounded shadow-sm border" style={{ width: '80px', height: '80px', objectFit: 'cover' }} alt="" />
                        <span className="position-absolute top-0 start-100 translate-middle badge rounded-pill bg-danger"
                          style={{ cursor: 'pointer' }}
                          onClick={() => {
                            setPreviews(previews.filter((_, i) => i !== index));
                            setSelectedFiles(selectedFiles.filter((_, i) => i !== index));
                          }}>
                          &times;
                        </span>
                      </div>
                    ))}
                  </div>
                </div>
              </div>
            </div>

            <hr className="my-4" />
            <div className=" rounded border p-3">
              <ProductAttributeManager onChange={(json) => setFormData({ ...formData, attributes: json })} />
            </div>
          </div>

          <div className="modal-footer py-3">
            <button type="button" className="btn btn-link text-secondary" onClick={onClose}>Hủy bỏ</button>
            <button type="submit" className="btn btn-primary px-4 shadow-sm" disabled={!formData.categoryId}>
              <i className="bi bi-check2-circle me-2"></i>Lưu sản phẩm
            </button>
          </div>
        </form>
      </div>
      
      <style>{`
        .border-dashed { border-style: dashed !important; border-width: 2px !important; }
        .form-check-input:checked { background-color: #0d6efd; border-color: #0d6efd; }
      `}</style>
    </div>
  );
};