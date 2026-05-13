using Duende.IdentityModel.Client;
using Duende.IdentityServer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RetailCore.Application.Extensions;
using RetailCore.Domain.Constants;
using RetailCore.Infrastructure.Data.Configurations.Identity;

namespace RetailCore.Application.UseCases;

public class AuthService : IAuthService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly AppDbContext _dbContext;
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IdentityServerOptions _identityOptions;
    private readonly IValidator<RegisterRequest> _registerRequestValidator;
    private readonly UserManager<IdentityUser<Guid>> _userManager;

    public AuthService(
        UserManager<IdentityUser<Guid>> userManager,
        ICustomerRepository customerRepository,
        AppDbContext dbContext,
        HttpClient httpClient,
        IOptions<IdentityServerOptions> identityOptions,
        IHttpContextAccessor httpContextAccessor,
        IValidator<RegisterRequest> registerRequestValidator)
    {
        _userManager = userManager;
        _customerRepository = customerRepository;
        _dbContext = dbContext;
        _httpClient = httpClient;
        _identityOptions = identityOptions.Value;
        _httpContextAccessor = httpContextAccessor;
        _registerRequestValidator = registerRequestValidator;
    }

    public async Task<Result<LoginResponse>> LoginAsync([FromBody] LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null) return Result<LoginResponse>.Failure("Email", "Email not found", 401);

        // Yêu cầu Token từ IdentityServer
        var disco = await _httpClient.GetDiscoveryDocumentAsync(_identityOptions.Authority);
        if (disco.IsError) return Result<LoginResponse>.Failure(null, "Identity Server is currently unavailable", 500);

        var tokenResponse = await _httpClient.RequestPasswordTokenAsync(new PasswordTokenRequest
        {
            Address = disco.TokenEndpoint,
            ClientId = _identityOptions.AdminClientId,
            ClientSecret = _identityOptions.AdminSecret,
            UserName = request.Email,
            Password = request.Password,
            Scope = $"{IdentityServerConstants.StandardScopes.OpenId} " +
                    $"{IdentityServerConstants.StandardScopes.Profile} " +
                    $"{IdentityServerConstants.StandardScopes.OfflineAccess} " +
                    $"{_identityOptions.ApiScopeName}"
        });

        if (tokenResponse.IsError)
            return Result<LoginResponse>.Failure("Password", "Invalid email or password", 401);

        // Cookie cho Access Token 
        var accessTokenOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddSeconds(tokenResponse.ExpiresIn)
        };

        // Cookie cho Refresh Token 
        var refreshTokenOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddDays(7)
        };

        var response = _httpContextAccessor.HttpContext?.Response;
        response?.Cookies.Append("X-Access-Token", tokenResponse.AccessToken, accessTokenOptions);
        response?.Cookies.Append("X-Refresh-Token", tokenResponse.RefreshToken, refreshTokenOptions);
        return Result<LoginResponse>.Success(new LoginResponse
        {
            AccessToken = tokenResponse.AccessToken,
            RefreshToken = tokenResponse.RefreshToken,
            AccessTokenExpire = tokenResponse.ExpiresIn,
            RefreshTokenExpire = (int)(refreshTokenOptions.Expires.Value - DateTimeOffset.UtcNow).TotalSeconds
        });
    }

    public async Task<Result<bool>> RegisterAsync(RegisterRequest request)
    {
        var validationResult = await _registerRequestValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            var errorDetails = validationResult.Errors.Select(e => new ErrorDetail
            {
                Key = e.PropertyName,
                Message = e.ErrorMessage
            }).ToList();

            return Result<bool>.Failure(errorDetails);
        }

        var isUnique = await _customerRepository.IsEmailUniqueAsync(request.Email);
        if (!isUnique) return Result<bool>.Failure("Email", "Email already exists");

        using var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            var user = new IdentityUser<Guid>
            {
                Id = Guid.CreateVersion7(),
                UserName = request.Email,
                Email = request.Email,
                EmailConfirmed = true 
            };

            var createResult = await _userManager.CreateAsync(user, request.Password);
        
            if (!createResult.Succeeded)
            {
                var errors = createResult.Errors.Select(e => new ErrorDetail { Key = e.Code, Message = e.Description }).ToList();
                return Result<bool>.Failure(errors);
            }

            var roleResult = await _userManager.AddToRoleAsync(user, Roles.Customer);
            if (!roleResult.Succeeded)
            {
                await transaction.RollbackAsync();
                return Result<bool>.Failure("Role", "Could not assign role to user");
            }

            var customer = new Customer
            {
                Id = Guid.CreateVersion7(),
                UserId = user.Id, 
                FullName = request.FullName,
                Email = request.Email,
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
            };

            await _customerRepository.AddAsync(customer);
            await _customerRepository.SaveChangesAsync();

            await transaction.CommitAsync();
            return Result<bool>.Success(true, 201);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return Result<bool>.Failure(null, $"System error: {ex.Message}", 500);
        }
    }

    public async Task<Result<bool>> LogoutAsync()
    {
        var response = _httpContextAccessor.HttpContext?.Response;

        response.Cookies.Delete("X-Access-Token", new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Lax
        });

        response.Cookies.Delete("X-Refresh-Token", new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Lax
        });

        response.Cookies.Delete("XSRF-TOKEN");

        return Result<bool>.Success(true, 204);
    }
    
    public async Task<Result<LoginResponse>> RefreshTokenAsync()
{
    var request = _httpContextAccessor.HttpContext?.Request;
    var refreshToken = request?.Cookies["X-Refresh-Token"];

    if (string.IsNullOrEmpty(refreshToken))
        return Result<LoginResponse>.Failure(null, "Refresh token is missing", 401);

    var disco = await _httpClient.GetDiscoveryDocumentAsync(_identityOptions.Authority);
    if (disco.IsError) return Result<LoginResponse>.Failure(null, "Identity Server unavailable", 500);

    var tokenResponse = await _httpClient.RequestRefreshTokenAsync(new RefreshTokenRequest
    {
        Address = disco.TokenEndpoint,
        ClientId = _identityOptions.AdminClientId,
        ClientSecret = _identityOptions.AdminSecret,
        RefreshToken = refreshToken
    });

    if (tokenResponse.IsError)
        return Result<LoginResponse>.Failure(null, "Invalid or expired refresh token", 401);

    var accessTokenOptions = new CookieOptions
    {
        HttpOnly = true,
        Secure = false,
        SameSite = SameSiteMode.Lax,
        Expires = DateTimeOffset.UtcNow.AddSeconds(tokenResponse.ExpiresIn)
    };

    var newRefreshTokenOptions = new CookieOptions
    {
        HttpOnly = true,
        Secure = false,
        SameSite = SameSiteMode.Lax,
        Expires = DateTimeOffset.UtcNow.AddDays(7)
    };

    var response = _httpContextAccessor.HttpContext?.Response;
    response?.Cookies.Append("X-Access-Token", tokenResponse.AccessToken, accessTokenOptions);
    
    if (!string.IsNullOrEmpty(tokenResponse.RefreshToken))
    {
        response?.Cookies.Append("X-Refresh-Token", tokenResponse.RefreshToken, newRefreshTokenOptions);
    }

    return Result<LoginResponse>.Success(new LoginResponse
    {
        AccessToken = tokenResponse.AccessToken,
        RefreshToken = tokenResponse.RefreshToken ?? refreshToken,
        AccessTokenExpire = tokenResponse.ExpiresIn,
        RefreshTokenExpire = (int)(newRefreshTokenOptions.Expires.Value - DateTimeOffset.UtcNow).TotalSeconds
    });
}

    public async Task<Result<CustomerResponse>> GetCurrentDetailsAsync()
    {
        Guid? customerId = _httpContextAccessor.HttpContext?.User.GetCustomerId();
        if (!customerId.HasValue) 
            return Result<CustomerResponse>.Failure(null, "User not found", 401);
        Customer? customer = await _customerRepository.GetByIdAsync(customerId.Value);
        if (customer == null) 
            return Result<CustomerResponse>.Failure(null, "User does not exist in system");
        var user = await _userManager.FindByIdAsync(customer.UserId.ToString());
        if (user == null) 
            return Result<CustomerResponse>.Failure(null, "User does not exist in system");

        var roles = await _userManager.GetRolesAsync(user);

        var response = new CustomerResponse
        {
            Id = customer.Id,
            UserId = user.Id,
            Email = customer.Email,
            FullName = customer.FullName,
            Roles = roles.ToList(),
            IsActive = customer.IsActive,
            CreatedDate = customer.CreatedDate,
            Phone = customer.Phone,
            Address = customer.Address,
            City = customer.City,
        };

        return Result<CustomerResponse>.Success(response);
    }
}