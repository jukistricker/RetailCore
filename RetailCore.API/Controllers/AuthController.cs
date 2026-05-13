namespace RetailCore.API.Controllers;

[Route("api/auth")]
[Authorize]
public class AuthController : ApiControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);
        return HandleResult(result);
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);
        return HandleResult(result);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        return HandleResult(await _authService.LogoutAsync());
    }
    
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken()
    {
        return HandleResult(await _authService.RefreshTokenAsync());
    }
    
    [HttpGet("details")]
    public async Task<IActionResult> GetCurrentDetails()
    {
        return HandleResult(await _authService.GetCurrentDetailsAsync());
    }
}