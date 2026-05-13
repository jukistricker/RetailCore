namespace RetailCore.CustomerSite.Services.Interfaces;

public interface IAuthApiService
{
    Task<Result<LoginResponse>> LoginAsync(LoginRequest request);
    Task<Result<bool>> RegisterAsync(RegisterRequest request);
    Task<Result<bool>> LogoutAsync();
}