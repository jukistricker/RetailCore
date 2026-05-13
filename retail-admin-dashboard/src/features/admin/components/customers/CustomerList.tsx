import React, { useEffect, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { RootState, AppDispatch } from '../../../../store/store';
import UserFormDialog from './UserFormDialog';
import UserDetailsDialog from './UserDetailsDialog';
import { Customer } from '../../../../types/customer';
import { fetchCustomers } from '../../../../store/thunks/customerThunk';
import { Pagination } from '../../../../components/ui/Pagination';

export const CustomerList: React.FC = () => {
  const dispatch = useDispatch<AppDispatch>();
  const { 
    items:customers, 
    totalCount, 
    currentPage, 
    pageSize, 
    totalPages, 
    loading 
  } = useSelector((state: RootState) => state.customer);

  const handlePageChange = (page: number) => {
    dispatch(fetchCustomers({ 
      pageNumber: page, 
      pageSize: pageSize 
    }));
  }; 
  
  const [showFormDialog, setShowFormDialog] = useState(false);
  const [showDetailsDialog, setShowDetailsDialog] = useState(false);
  const [selectedCustomer, setSelectedCustomer] = useState<Customer | null>(null);

  useEffect(() => {
    dispatch(fetchCustomers({ 
      pageNumber: currentPage, 
      pageSize: pageSize 
    }));
  }, [dispatch, currentPage, pageSize]);

  const handleAddClick = () => {
    setSelectedCustomer(null);
    setShowFormDialog(true);
  };

  const handleEditClick = (customer: Customer) => {
    setSelectedCustomer(customer);
    setShowFormDialog(true);
  };

  const handleViewClick = (customer: Customer) => {
    setSelectedCustomer(customer);
    setShowDetailsDialog(true);
  };

  const handleFormClose = () => {
    setShowFormDialog(false);
    setSelectedCustomer(null);
  };

  const handleDetailsClose = () => {
    setShowDetailsDialog(false);
    setSelectedCustomer(null);
  };

  return (
    <div className="container-fluid">
      <div className="d-flex justify-content-between align-items-center mb-4">
        <div>
          <h1 className="fw-bold mb-0">Manage Customers</h1>
          <p className="text-muted small mb-0">Total: {totalCount} customers</p>
        </div>
        <button className="btn btn-success shadow-sm" onClick={handleAddClick}>
          <i className="bi bi-plus-circle me-2"></i>Add Customeer
        </button>
      </div>

      

      <div className="card border-0 shadow-sm">
        <div className="table-responsive">
          <table className="table table-hover align-middle mb-0">
            <thead className="table">
              <tr>
                <th className="py-3">No.</th>
                <th className="py-3">Full Name</th>
                <th className="py-3">Email</th>
                <th className="py-3">Address</th>
                <th className="py-3">Status</th>
                <th className="py-3 text-end">Action</th>
              </tr>
            </thead>
            <tbody>
              {loading ? (
                <tr>
                  <td colSpan={6} className="text-center py-5">
                    <div className="spinner-border text-primary" role="status"></div>
                  </td>
                </tr>
              ) : customers.length === 0 ? (
                <tr>
                  <td colSpan={6} className="text-center py-5 text-muted">No data</td>
                </tr>
              ) : (
                customers.map((customer, index) => (
                  <tr key={customer.id}>
                    <td>{(currentPage - 1) * pageSize + index + 1}</td>
                    <td><span className="fw-bold">{customer.fullName}</span></td>
                    <td>{customer.email}</td>
                    <td>{customer.city || "---"}</td>
                    <td>
                      <span className={`badge rounded-pill ${customer.isActive ? 'bg-success-subtle text-success' : 'bg-danger-subtle text-danger'}`}>
                        {customer.isActive ? 'Active' : 'Locked'}
                      </span>
                    </td>
                    <td className="text-end">
                      <button className="btn btn-sm btn-outline-info me-2" onClick={() => handleViewClick(customer)}>
                        <i className="bi bi-eye"></i>
                      </button>
                      <button className="btn btn-sm btn-outline-primary" onClick={() => handleEditClick(customer)}>
                        <i className="bi bi-pencil"></i>
                      </button>
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      </div>

      <div className="card-footer border-0">
        <Pagination
          totalItems={totalCount}
          itemsPerPage={pageSize}
          currentPage={currentPage}
          totalPages={totalPages}
          onPageChange={handlePageChange}
        />
      </div>

      {showFormDialog && (
        <UserFormDialog
          customer={selectedCustomer} 
          onClose={handleFormClose}
        />
      )}

      {showDetailsDialog && selectedCustomer && (
        <UserDetailsDialog
          user={selectedCustomer} 
          onClose={handleDetailsClose}
        />
      )}
    </div>
  );
};