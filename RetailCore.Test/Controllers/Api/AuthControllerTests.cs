using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RetailCore.API.Controllers;
using RetailCore.Application.UseCases.Interfaces;
using RetailCore.Domain.Commons;
using RetailCore.Shared.DTOs;

namespace RetailCore.Test.Controllers.Api;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _mockAuthService;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _mockAuthService = new Mock<IAuthService>();

        _controller = new AuthController(_mockAuthService.Object);

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, "user-id-123"),
            new Claim(ClaimTypes.Email, "admin@retailcore.com"),
        }, "mock"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    [Fact]
    public async Task Login_ShouldReturnOk_WhenCredentialsAreCorrect()
    {
        // Arrange
        var request = new LoginRequest { Email = "admin@retailcore.com", Password = "Password123" };
        var loginResponse = new LoginResponse { AccessToken = "fake-jwt-token" };
        var expectedResult = Result<LoginResponse>.Success(loginResponse);
    
        _mockAuthService.Setup(s => s.LoginAsync(request)).ReturnsAsync(expectedResult);

        // Act
        var result = await _controller.Login(request);

        // Assert
        var objectResult = result.Should().BeOfType<ObjectResult>().Subject;
    
        objectResult.StatusCode.Should().Be(200); 
        objectResult.Value.Should().BeEquivalentTo(expectedResult);
    }
    
    [Fact]
    public async Task Register_ShouldReturnOk_WhenRegistrationIsSuccessful()
    {
        // Arrange
        var request = new RegisterRequest { Email = "new@retail.com", Password = "Pass", FullName = "Thanh" };
        
        var expectedResult = Result<bool>.Success(true); 
        _mockAuthService.Setup(s => s.RegisterAsync(request)).ReturnsAsync(expectedResult);

        // Act
        var result = await _controller.Register(request);

        // Assert
        var objectResult = result.Should().BeOfType<ObjectResult>().Subject;
        objectResult.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task Logout_ShouldReturnOk_WhenUserIsAuthenticated()
    {
        // Arrange
        _mockAuthService.Setup(s => s.LogoutAsync()).ReturnsAsync(Result<bool>.Success(true));

        // Act
        var result = await _controller.Logout();

        // Assert
        var objectResult = result.Should().BeOfType<ObjectResult>().Subject;
        objectResult.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task GetCurrentDetails_ShouldReturnUserDetails_WhenFound()
    {
        // Arrange
        var userDetails = new CustomerResponse { Email = "admin@retailcore.com" }; 
        var expectedResult = Result<CustomerResponse>.Success(userDetails);
        
        _mockAuthService.Setup(s => s.GetCurrentDetailsAsync()).ReturnsAsync(expectedResult);

        // Act
        var result = await _controller.GetCurrentDetails();

        // Assert
        var objectResult = result.Should().BeOfType<ObjectResult>().Subject;
        objectResult.StatusCode.Should().Be(200);
        objectResult.Value.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task Login_ShouldReturnBadRequest_WhenServiceFails()
    {
        // Arrange
        var request = new LoginRequest { Email = "wrong@test.com" };
        
        var expectedResult = Result<LoginResponse>.Failure("AuthError", "Invalid credentials", 400);
        
        _mockAuthService.Setup(s => s.LoginAsync(request)).ReturnsAsync(expectedResult);

        // Act
        var result = await _controller.Login(request);

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.StatusCode.Should().Be(400);
        badRequestResult.Value.Should().BeEquivalentTo(expectedResult);
    }
}