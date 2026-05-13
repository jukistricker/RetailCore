using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RetailCore.Application.Extensions;
using RetailCore.Application.Mappings;

namespace RetailCore.Application.UseCases;

public class CustomerService:ICustomerService
{
    private readonly  ICustomerRepository _customerRepository;
    private readonly  AppDbContext _dbContext;
    private readonly  UserManager<IdentityUser<Guid>> _userManager;
    private IHttpContextAccessor _httpContextAccessor;
    public CustomerService(ICustomerRepository customerRepository, AppDbContext dbContext, UserManager<IdentityUser<Guid>> userManager, IHttpContextAccessor httpContextAccessor)
    {
        _customerRepository = customerRepository;
        _dbContext = dbContext;
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }

public async Task<Result<PagingResponse<CustomerResponse>>> GetAllUsersAsync(PagingRequest request)
{
    var query = _customerRepository.GetQueryable().AsNoTracking();
    var response = await _customerRepository.GetByPageAsync(query, request.PageNumber, request.PageSize);
    return Result<PagingResponse<CustomerResponse>>.Success(CustomerMapping.ToPagingResponse(response));
}

public async Task<Result<CustomerResponse>> GetUserByIdAsync(Guid id)
{
    var customer = await _dbContext.Customers
        .FirstOrDefaultAsync(c => c.UserId == id);

    if (customer == null)
        return Result<CustomerResponse>.Failure("User", "User not found");

    var userDto = CustomerMapping.ToCustomerResponse(customer);

    return Result<CustomerResponse>.Success(userDto);
}

public async Task<Result<bool>> UpdateUserAsync(Guid id, UpdateCustomerRequest request)
{
    var customer = await _customerRepository.GetByIdAsync(id);
    if (customer == null) 
        return Result<bool>.Failure("User", "User not found");
    
    var user = await _userManager.FindByIdAsync(customer.UserId.ToString());
    if (user == null) 
        return Result<bool>.Failure("User", "User not found");


    using var transaction = await _dbContext.Database.BeginTransactionAsync();
    try
    {
        if (customer.Email != request.Email)
        {
            var emailExists = await _userManager.FindByEmailAsync(request.Email);
            if (emailExists != null&&emailExists.Id != id) 
                return Result<bool>.Failure("Email", "Email already in use");

            user.Email = request.Email;
            await _userManager.UpdateAsync(user);
            customer.Email = request.Email;
            _customerRepository.Update(customer);
        }

        CustomerMapping.ToCustomerUpdate(customer, request, customer.Id);

        _dbContext.Customers.Update(customer);
        await _dbContext.SaveChangesAsync();

        await transaction.CommitAsync();
        return Result<bool>.Success(true);
    }
    catch (Exception)
    {
        await transaction.RollbackAsync();
        return Result<bool>.Failure(null, "An error occurred while updating user", 500);
    }
}
}