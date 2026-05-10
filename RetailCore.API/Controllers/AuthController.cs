namespace RetailCore.API.Controllers;

[Route("api/[controller]")]
public class AuthController : ApiControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        Result<LoginResponse> result = await _authService.LoginAsync(request);
        return HandleResult(result); 
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody]RegisterRequest request)
    {
        Result<bool> result = await _authService.RegisterAsync(request);
        return HandleResult(result); 
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] string refreshToken)
    {
        return HandleResult(await _authService.LogoutAsync(refreshToken));
    }
}