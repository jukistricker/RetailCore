namespace RetailCore.Application.UseCases.Interfaces;

public interface IAuthService
{
    Task<Result<LoginResponse>> LoginAsync(LoginRequest request);
    Task<Result<bool>> RegisterAsync(RegisterRequest request);
    Task<Result<bool>> LogoutAsync(string refreshToken);
}