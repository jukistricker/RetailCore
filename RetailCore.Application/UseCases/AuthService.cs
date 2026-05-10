

using System.Net;
using Duende.IdentityModel.Client;
using Duende.IdentityServer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using RetailCore.Infrastructure.Data;
using RetailCore.Infrastructure.Identity;

namespace RetailCore.Application.UseCases;


using Microsoft.AspNetCore.Identity;

public class AuthService : IAuthService
{
    private readonly UserManager<IdentityUser<Guid>> _userManager;
    private readonly ICustomerRepository _customerRepository;
    private readonly AppDbContext _dbContext;
    private readonly HttpClient _httpClient;
    private readonly IdentityServerOptions _identityOptions;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthService(
        UserManager<IdentityUser<Guid>> userManager,
        ICustomerRepository customerRepository,
        AppDbContext dbContext,
        HttpClient httpClient, 
        IOptions<IdentityServerOptions> identityOptions,
        IHttpContextAccessor httpContextAccessor) 
    {
        _userManager = userManager;
        _customerRepository = customerRepository;
        _dbContext = dbContext;
        _httpClient = httpClient;
        _identityOptions = identityOptions.Value;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result<LoginResponse>> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null) return Result<LoginResponse>.Failure("Invalid credentials", 401);

        // Yêu cầu Token từ IdentityServer
        var disco = await _httpClient.GetDiscoveryDocumentAsync(_identityOptions.Authority);
        if (disco.IsError) return Result<LoginResponse>.Failure("Identity Server is currently unavailable", 500);

        var tokenResponse = await _httpClient.RequestPasswordTokenAsync(new PasswordTokenRequest
        {
            Address = disco.TokenEndpoint,
            ClientId = _identityOptions.AdminClientId,
            ClientSecret = _identityOptions.AdminSecret,
            UserName = request.Email,
            Password = request.Password,
            Scope = $"{IdentityServerConstants.StandardScopes.OpenId} {IdentityServerConstants.StandardScopes.Profile} {_identityOptions.ApiScopeName}"
        });

        if (tokenResponse.IsError) 
            return Result<LoginResponse>.Failure("Invalid email or password", 401);

        // Cookie cho Access Token 
        var accessTokenOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddSeconds(tokenResponse.ExpiresIn)
        };

        // Cookie cho Refresh Token 
        var refreshTokenOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(7) 
        };

        var response = _httpContextAccessor.HttpContext?.Response;
        response?.Cookies.Append("X-Access-Token", tokenResponse.AccessToken, accessTokenOptions);
        response?.Cookies.Append("X-Refresh-Token", tokenResponse.RefreshToken, refreshTokenOptions);
        return Result<LoginResponse>.Success(new LoginResponse
        {
            AccessToken = tokenResponse.AccessToken,
            RefreshToken = tokenResponse.RefreshToken,
            ExpiresIn = tokenResponse.ExpiresIn
        });
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
                Email = request.Email,
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

    public async Task<Result<bool>> LogoutAsync(string refreshToken)
    {
        if (string.IsNullOrEmpty(refreshToken)) 
            return Result<bool>.Failure("Refresh token is required", 400);

        var disco = await _httpClient.GetDiscoveryDocumentAsync(_identityOptions.Authority);
        if (disco.IsError) return Result<bool>.Failure("Identity Server is unavailable", 500);

        await _httpClient.RevokeTokenAsync(new TokenRevocationRequest
        {
            Address = disco.RevocationEndpoint,
            ClientId = _identityOptions.AdminClientId,
            ClientSecret = _identityOptions.AdminSecret,
            Token = refreshToken,
            TokenTypeHint = "refresh_token"
        });
        
        return Result<bool>.Success(true, 204);
    }
}