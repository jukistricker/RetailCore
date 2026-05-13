using FluentValidation;
using RetailCore.CustomerSite.Services.Interfaces;
using RetailCore.Shared.Constants;

namespace RetailCore.CustomerSite.Services;

public class AuthApiService : BaseApiService, IAuthApiService
{
    private readonly IValidator<RegisterRequest> _registerValidator;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthApiService(IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        IValidator<RegisterRequest> registerValidator, IHttpContextAccessor httpContextAccessor) : base(httpClientFactory, configuration)
    {
        _registerValidator = registerValidator;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result<LoginResponse>> LoginAsync(LoginRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync(ApiRoutes.Auth.Login, request);

        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<Result<LoginResponse>>()
                   ?? Result<LoginResponse>.Failure(null, "Deserialization error");

        var errorResult = await response.Content.ReadFromJsonAsync<Result<LoginResponse>>();
        return errorResult ?? Result<LoginResponse>.Failure(null, "Login failed", (int)response.StatusCode);
    }

    public async Task<Result<bool>> RegisterAsync(RegisterRequest request)
    {
        var validationResult = await _registerValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => new ErrorDetail
            {
                Key = e.PropertyName,
                Message = e.ErrorMessage
            }).ToList();

            return Result<bool>.Failure(errors);
        }

        var response = await _httpClient.PostAsJsonAsync(ApiRoutes.Auth.Register, request);

        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<Result<bool>>()
                   ?? Result<bool>.Success(true);

        var errorResult = await response.Content.ReadFromJsonAsync<Result<bool>>();
        return errorResult ?? Result<bool>.Failure(null, "Register failed", (int)response.StatusCode);
    }

    public async Task<Result<bool>> LogoutAsync()
    {
        var response = await _httpClient.PostAsync(ApiRoutes.Auth.Logout, null);

        if (response.IsSuccessStatusCode) return Result<bool>.Success(true);

        return Result<bool>.Failure(null, "An error occured", (int)response.StatusCode);
    }

    public async Task<Result<LoginResponse>> RefreshTokenAsync()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null) return Result<LoginResponse>.Failure(null, "HttpContext not found");

        var refreshToken = httpContext.Request.Cookies["X-Refresh-Token"];
        if (string.IsNullOrEmpty(refreshToken)) 
            return Result<LoginResponse>.Failure(null, "No refresh token found in cookies");

        var response = await _httpClient.PostAsJsonAsync(ApiRoutes.Auth.RefreshToken, new { RefreshToken = refreshToken });

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<Result<LoginResponse>>();
            
            return result ?? Result<LoginResponse>.Failure(null, "Deserialization error");
        }

        return Result<LoginResponse>.Failure(null, "Refresh token failed", (int)response.StatusCode);
    }

    public async Task<Result<CustomerResponse>> GetCurrentDetailsAsync()
    {
        var response = await _httpClient.GetFromJsonAsync<Result<CustomerResponse>>(ApiRoutes.Auth.CurrentDetails);
        return response?.IsSuccess == true ? response : Result<CustomerResponse>.Failure(null, "Current details not found", response.StatusCode);
    }
}