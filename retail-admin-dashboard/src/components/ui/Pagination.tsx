// src/components/ui/Pagination.tsx
interface PaginationProps {
  totalItems: number;
  itemsPerPage: number;
  currentPage: number;
  totalPages: number; // Thêm totalPages từ Server trả về
  onPageChange: (page: number) => void;
}

export const Pagination = ({
  totalItems,
  itemsPerPage,
  currentPage,
  totalPages,
  onPageChange,
}: PaginationProps) => {
  if (totalPages <= 1) return null;

  const getPageNumbers = () => {
    const pages = [];
    for (let i = 1; i <= totalPages; i++) {
      pages.push(i);
    }
    return pages;
  };

  return (
    <div className="d-flex justify-content-between align-items-center px-2 py-3">
      <div className="text-muted small">
        Hiển thị <b>{Math.min((currentPage - 1) * itemsPerPage + 1, totalItems)}</b> đến{" "}
        <b>{Math.min(currentPage * itemsPerPage, totalItems)}</b> trong số{" "}
        <b>{totalItems}</b> khách hàng
      </div>

      <ul className="pagination pagination-sm mb-0">
        <li className={`page-item ${currentPage === 1 ? "disabled" : ""}`}>
          <button className="page-link" onClick={() => onPageChange(currentPage - 1)}>
            <i className="bi bi-chevron-left"></i>
          </button>
        </li>

        {getPageNumbers().map((page) => (
          <li key={page} className={`page-item ${currentPage === page ? "active" : ""}`}>
            <button className="page-link" onClick={() => onPageChange(page)}>
              {page}
            </button>
          </li>
        ))}

        <li className={`page-item ${currentPage === totalPages ? "disabled" : ""}`}>
          <button className="page-link" onClick={() => onPageChange(currentPage + 1)}>
            <i className="bi bi-chevron-right"></i>
          </button>
        </li>
      </ul>
    </div>
  );
};