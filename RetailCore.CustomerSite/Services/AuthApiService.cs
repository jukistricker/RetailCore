using FluentValidation;
using RetailCore.CustomerSite.Services.Interfaces;
using RetailCore.Shared.Constants;

namespace RetailCore.CustomerSite.Services;

public class AuthApiService : BaseApiService, IAuthApiService
{
    private readonly IValidator<RegisterRequest> _registerValidator;

    public AuthApiService(IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        IValidator<RegisterRequest> registerValidator) : base(httpClientFactory, configuration)
    {
        _registerValidator = registerValidator;
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
}