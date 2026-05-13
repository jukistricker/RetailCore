using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using RetailCore.CustomerSite.Extensions;
using RetailCore.CustomerSite.Services.Interfaces;
// Để dùng AddResultErrors

namespace RetailCore.CustomerSite.Controllers;

public class AuthController : Controller
{
    private readonly IAuthApiService _authApiService;

    public AuthController(IAuthApiService authApiService)
    {
        _authApiService = authApiService;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _authApiService.LogoutAsync();

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginRequest request, string? returnUrl = null)
    {
        if (!ModelState.IsValid) return View(request);

        var result = await _authApiService.LoginAsync(request);

        if (result.IsSuccess)
        {
            var accessTokenOptions = new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddSeconds(result.Value.AccessTokenExpire)
            };

            var refreshTokenOptions = new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            };

            Response.Cookies.Append("X-Access-Token", result.Value.AccessToken, accessTokenOptions);
            Response.Cookies.Append("X-Refresh-Token", result.Value.RefreshToken, refreshTokenOptions);

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(result.Value.AccessToken);

            var claims = jwtToken.Claims.ToList();
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddSeconds(result.Value.AccessTokenExpire)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);


            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);
            return RedirectToAction("Index", "Home");
        }

        ModelState.AddResultErrors(result.Errors);
        return View(request);
    }


    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        if (!ModelState.IsValid) return View(request);

        var result = await _authApiService.RegisterAsync(request);

        if (result.IsSuccess)
        {
            TempData["SuccessMessage"] = "Register success! Please login.";
            return RedirectToAction("Login", "Auth");
        }

        ModelState.AddResultErrors(result.Errors);
        return View(request);
    }

    [HttpPost]
    public async Task<IActionResult> RefreshToken()
    {
        var result = await _authApiService.RefreshTokenAsync();

        if (result.IsSuccess && result.Value != null)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTimeOffset.UtcNow.AddMinutes(15) 
            };

            Response.Cookies.Append("X-Access-Token", result.Value.AccessToken, cookieOptions);
        
            if (!string.IsNullOrEmpty(result.Value.RefreshToken))
            {
                Response.Cookies.Append("X-Refresh-Token", result.Value.RefreshToken, new CookieOptions 
                { 
                    HttpOnly = true, 
                    Secure = true,
                    Expires = DateTimeOffset.UtcNow.AddDays(7)
                });
            }

            return Ok(result);
        }

        return Unauthorized(result);
    }
    
    
}