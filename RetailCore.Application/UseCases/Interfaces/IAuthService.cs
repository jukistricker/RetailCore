namespace RetailCore.Application.UseCases.Interfaces;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
    Task<Result<bool>> RegisterAsync(RegisterRequest request);
    Task<Result<bool>> LogoutAsync();
}