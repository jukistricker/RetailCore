namespace RetailCore.Application.UseCases.Interfaces;
//
public interface ICustomerService
{
    Task<Result<PagingResponse<CustomerResponse>>> GetAllUsersAsync(PagingRequest request);
    Task<Result<CustomerResponse>> GetUserByIdAsync(Guid id);
    Task<Result<bool>> UpdateUserAsync(Guid id, UpdateCustomerRequest request);
}

