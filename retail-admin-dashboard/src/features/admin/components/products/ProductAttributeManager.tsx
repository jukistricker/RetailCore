// components/products/ProductAttributeManager.tsx
import React, { useState, useEffect } from 'react';

interface Attribute {
  attributeName: string;
  value: string;
  priceAdjustment: number;
  stock: number;
  childAttributes: Attribute[];
}

export const ProductAttributeManager = ({ onChange }: { onChange: (json: string) => void }) => {
  const [parentName, setParentName] = useState('Màu sắc');
  const [childName, setChildName] = useState('Kích thước');
  const [attributes, setAttributes] = useState<Attribute[]>([]);

  // Mỗi khi dữ liệu thay đổi, map lại tên đồng bộ trước khi gửi lên Form chính
  useEffect(() => {
    const synchronizedData = attributes.map(parent => ({
      ...parent,
      attributeName: parentName,
      childAttributes: parent.childAttributes.map(child => ({
        ...child,
        attributeName: childName
      }))
    }));
    onChange(JSON.stringify(synchronizedData));
  }, [attributes, parentName, childName]);

  const addParent = () => {
    const newParent: Attribute = {
      attributeName: parentName,
      value: '',
      priceAdjustment: 0,
      stock: 0,
      // Nếu đã có cấu trúc con ở các parent khác, copy cấu trúc đó sang parent mới
      childAttributes: attributes[0]?.childAttributes.map(c => ({
        ...c,
        value: '',
        priceAdjustment: 0,
        stock: 0
      })) || []
    };
    setAttributes([...attributes, newParent]);
  };

  const addChildToAllParents = () => {
    const updated = attributes.map(parent => ({
      ...parent,
      childAttributes: [
        ...parent.childAttributes,
        { attributeName: childName, value: '', priceAdjustment: 0, stock: 0, childAttributes: [] }
      ]
    }));
    
    // Nếu chưa có parent nào, tạo một khung mẫu
    if (updated.length === 0) {
      setAttributes([{
        attributeName: parentName, value: '', priceAdjustment: 0, stock: 0,
        childAttributes: [{ attributeName: childName, value: '', priceAdjustment: 0, stock: 0, childAttributes: [] }]
      }]);
    } else {
      setAttributes(updated);
    }
  };

  return (
    <div className="border rounded p-3 shadow-sm mt-3">
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h6 className="fw-bold mb-0 text-dark">Phân loại sản phẩm</h6>
        <div>
          <button type="button" className="btn btn-sm btn-primary me-2" onClick={addParent}>
            <i className="bi bi-plus-circle me-1"></i>Thêm nhóm chính
          </button>
          <button type="button" className="btn btn-sm btn-secondary" onClick={addChildToAllParents}>
            <i className="bi bi-node-plus me-1"></i>Thêm lớp con
          </button>
        </div>
      </div>

      {/* Inputs quản lý Tên chung */}
      <div className="row mb-3 g-3">
        <div className="col-md-6">
          <label className="small fw-bold">Tên nhóm chính (ví dụ: Màu sắc, Chất liệu)</label>
          <input className="form-control form-control-sm border-primary" value={parentName} 
            onChange={e => setParentName(e.target.value)} placeholder="Nhập tên thuộc tính chính..." />
        </div>
        <div className="col-md-6">
          <label className="small fw-bold">Tên lớp con (ví dụ: Kích thước, Dung lượng)</label>
          <input className="form-control form-control-sm border-secondary" value={childName}
            onChange={e => setChildName(e.target.value)} placeholder="Nhập tên thuộc tính con..." />
        </div>
      </div>

      <hr />

      {attributes.map((parent, pIdx) => (
        <div key={pIdx} className="card mb-4 border-0 shadow-sm">
          <div className="card-body rounded">
            <div className="row g-3 align-items-end">
              <div className="col-md-4">
                <label className="text-muted small">{parentName}</label>
                <input className="form-control" placeholder="Giá trị (đỏ, xanh...)" value={parent.value}
                  onChange={e => setAttributes(prev => prev.map((p, i) => i === pIdx ? { ...p, value: e.target.value } : p))} />
              </div>
              <div className="col-md-3">
                <label className="text-muted small">Giá tăng thêm</label>
                <div className="input-group">
                  <span className="input-group-text">$</span>
                  <input type="number" className="form-control" value={parent.priceAdjustment}
                    onChange={e => setAttributes(prev => prev.map((p, i) => i === pIdx ? { ...p, priceAdjustment: +e.target.value } : p))} />
                </div>
              </div>
              <div className="col-md-1 text-end">
                <button type="button" className="btn btn-outline-danger border-0" 
                  onClick={() => setAttributes(prev => prev.filter((_, i) => i !== pIdx))}>
                  <i className="bi bi-trash"></i>
                </button>
              </div>
            </div>

            {/* Render Child Attributes */}
            <div className="mt-3 ms-4 border-start ps-4">
              {parent.childAttributes.map((child, cIdx) => (
                <div key={cIdx} className="row g-2 mb-2 align-items-center">
                  <div className="col-md-4">
                    <label className="text-muted extra-small">{childName}</label>
                    <input className="form-control form-control-sm" placeholder="S, M, L..." value={child.value}
                      onChange={e => {
                        const val = e.target.value;
                        // Chỉ cập nhật giá trị cho child cụ thể này của parent này
                        setAttributes(prev => prev.map((p, pi) => pi === pIdx ? {
                          ...p, childAttributes: p.childAttributes.map((c, ci) => ci === cIdx ? { ...c, value: val } : c)
                        } : p));
                      }} />
                  </div>
                  <div className="col-md-3">
                    <label className="text-muted extra-small">Kho</label>
                    <input type="number" className="form-control form-control-sm" value={child.stock}
                      onChange={e => {
                        const val = +e.target.value;
                        setAttributes(prev => prev.map((p, pi) => pi === pIdx ? {
                          ...p, childAttributes: p.childAttributes.map((c, ci) => ci === cIdx ? { ...c, stock: val } : c)
                        } : p));
                      }} />
                  </div>
                </div>
              ))}
            </div>
          </div>
        </div>
      ))}
      
      <style>{`.extra-small { font-size: 0.75rem; }`}</style>
    </div>
  );
};