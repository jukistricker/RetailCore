

using System.Net;
using RetailCore.Infrastructure.Data;

namespace RetailCore.Application.UseCases;


using Microsoft.AspNetCore.Identity;

public class AuthService : IAuthService
{
    private readonly UserManager<IdentityUser<Guid>> _userManager;
    private readonly ICustomerRepository _customerRepository;
    private readonly AppDbContext _dbContext;

    public AuthService(
        UserManager<IdentityUser<Guid>> userManager,
        ICustomerRepository customerRepository,
        AppDbContext dbContext)
    {
        _userManager = userManager;
        _customerRepository = customerRepository;
        _dbContext = dbContext;
    }

    public Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<bool>> RegisterAsync(RegisterRequest request)
    {
        var isUnique = await _customerRepository.IsEmailUniqueAsync(request.Email);
        if (!isUnique) return Result<bool>.Failure("Email already exists");

        using var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            var user = new IdentityUser<Guid> 
            { 
                Id = Guid.CreateVersion7(), 
                UserName = request.Email, 
                Email = request.Email 
            };

            await _userManager.CreateAsync(user, request.Password);
            
            var customer = new Customer { 
                Id = Guid.CreateVersion7(), 
                UserId = user.Id, 
                FullName = request.FullName, 
                Email = request.Email 
            };

            await _customerRepository.AddAsync(customer);
            await _customerRepository.SaveChangesAsync();

            await transaction.CommitAsync();
            return Result<bool>.Success(true, 201);
        }
        catch (Exception ex) 
        {
            await transaction.RollbackAsync();
            return Result<bool>.Failure("A system error occurred", 500);
        }
    }

    public Task<Result<bool>> LogoutAsync()
    {
        throw new NotImplementedException();
    }
}