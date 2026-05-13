using System.Net;
using System.Security.Claims;
using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using RetailCore.Application.UseCases;
using RetailCore.Domain.Entities;
using RetailCore.Domain.Interfaces.Repositories;
using RetailCore.Infrastructure.Data;
using RetailCore.Infrastructure.Data.Configurations.Identity;
using RetailCore.Shared.DTOs;

namespace RetailCore.Test.Services.Api;

public class AuthServiceTests
{
    private readonly Mock<UserManager<IdentityUser<Guid>>> _mockUserManager;
    private readonly Mock<ICustomerRepository> _mockCustomerRepo;
    private readonly Mock<AppDbContext> _mockDbContext; // Cần cấu hình DB In-Memory hoặc Mock Transaction
    private readonly Mock<IHttpContextAccessor> _mockHttpAccessor;
    private readonly Mock<IValidator<RegisterRequest>> _mockValidator;
    private readonly AuthService _authService;
    private readonly IdentityServerOptions _options;
    private readonly AppDbContext _realDbContext;

    public AuthServiceTests()
    {
        var dbOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning)) // Thêm dòng này
            .Options;

        _realDbContext = new AppDbContext(dbOptions);
    
        // Khởi tạo Mock bình thường
        var store = new Mock<IUserStore<IdentityUser<Guid>>>();
        _mockUserManager = new Mock<UserManager<IdentityUser<Guid>>>(store.Object, null, null, null, null, null, null, null, null);
    
        _mockCustomerRepo = new Mock<ICustomerRepository>();
        _mockHttpAccessor = new Mock<IHttpContextAccessor>();
        _mockValidator = new Mock<IValidator<RegisterRequest>>();
        
        // Setup IdentityServer Options
        _options = new IdentityServerOptions 
        { 
            Authority = "https://localhost:5001",
            AdminClientId = "admin",
            AdminSecret = "secret",
            ApiScopeName = "api"
        };
        var optionsMock = new Mock<IOptions<IdentityServerOptions>>();
        optionsMock.Setup(o => o.Value).Returns(_options);

        var handlerMock = new Mock<HttpMessageHandler>();
        var httpClient = new HttpClient(handlerMock.Object);

        _authService = new AuthService(
            _mockUserManager.Object,
            _mockCustomerRepo.Object,
            _realDbContext, 
            httpClient,
            optionsMock.Object,
            _mockHttpAccessor.Object,
            _mockValidator.Object
        );
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnFailure_WhenValidationFails()
    {
        // Arrange
        var request = new RegisterRequest { Email = "invalid-email" };
        var validationFailures = new List<FluentValidation.Results.ValidationFailure>
        {
            new("Email", "Invalid email format")
        };
        _mockValidator.Setup(v => v.ValidateAsync(request, default))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult(validationFailures));

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Key == "Email");
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnFailure_WhenEmailAlreadyExists()
    {
        // Arrange
        var request = new RegisterRequest { Email = "exists@gmail.com" };
        _mockValidator.Setup(v => v.ValidateAsync(request, default))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        _mockCustomerRepo.Setup(r => r.IsEmailUniqueAsync(request.Email)).ReturnsAsync(false);

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Message == "Email already exists");
    }
    
    [Fact]
    public async Task RegisterAsync_ShouldReturnSuccess_WhenEverythingIsValid()
    {
        // Arrange
        var request = new RegisterRequest 
        { 
            Email = "success@gmail.com", 
            Password = "Password123!", 
            FullName = "Duong Ngoc Thanh" 
        };

        // 1. Validation pass
        _mockValidator.Setup(v => v.ValidateAsync(request, default))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        // 2. Email unique
        _mockCustomerRepo.Setup(r => r.IsEmailUniqueAsync(request.Email)).ReturnsAsync(true);

        // 3. UserManager create success
        _mockUserManager.Setup(m => m.CreateAsync(It.IsAny<IdentityUser<Guid>>(), request.Password))
            .ReturnsAsync(IdentityResult.Success);

        // 4. Add to Role success
        _mockUserManager.Setup(m => m.AddToRoleAsync(It.IsAny<IdentityUser<Guid>>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(201);
    
        _mockCustomerRepo.Verify(r => r.AddAsync(It.IsAny<RetailCore.Domain.Entities.Customer>()), Times.Once);
    }
    
    [Fact]
    public async Task RegisterAsync_ShouldReturnFailure_WhenIdentityCreateFails()
    {
        // Arrange
        var request = new RegisterRequest { Email = "fail@gmail.com", Password = "123" };
        _mockValidator.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(new FluentValidation.Results.ValidationResult());
        _mockCustomerRepo.Setup(r => r.IsEmailUniqueAsync(request.Email)).ReturnsAsync(true);
    
        // Mock Identity Result Failure
        _mockUserManager.Setup(m => m.CreateAsync(It.IsAny<IdentityUser<Guid>>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Code = "PassTooShort", Description = "Short" }));

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Key == "PassTooShort");
    }
    
    [Fact]
    public async Task LoginAsync_ShouldReturnSuccess_WhenCredentialsAreValid()
    {
        // Arrange
        var request = new LoginRequest { Email = "test@gmail.com", Password = "Password123!" };
        var user = new IdentityUser<Guid> { Email = request.Email };
        _mockUserManager.Setup(m => m.FindByEmailAsync(request.Email)).ReturnsAsync(user);

        // Mock Discovery Document & Token Response
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"token_endpoint\":\"https://test.com/token\", \"access_token\":\"abc\", \"expires_in\":3600, \"refresh_token\":\"ref\"}")
            });

        var httpClient = new HttpClient(handlerMock.Object);
        // Lưu ý: Bạn cần khởi tạo lại _authService với httpClient này hoặc tiêm từ ngoài
    
        var mockResponse = new Mock<HttpResponse>();
        var mockCookies = new Mock<IResponseCookies>();
        var mockContext = new Mock<HttpContext>();
        mockResponse.Setup(r => r.Cookies).Returns(mockCookies.Object);
        mockContext.Setup(c => c.Response).Returns(mockResponse.Object);
        _mockHttpAccessor.Setup(a => a.HttpContext).Returns(mockContext.Object);

        // Act 
        var result = await _authService.LoginAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(200);
    }
    
    [Fact]
    public async Task LoginAsync_ShouldReturnFailure_WhenEmailNotFound()
    {
        // Arrange
        var request = new LoginRequest { Email = "notfound@gmail.com" };
        _mockUserManager.Setup(m => m.FindByEmailAsync(request.Email))
            .ReturnsAsync((IdentityUser<Guid>)null);

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(401);
        result.Errors.Should().Contain(e => e.Message == "Email not found");
    }

    [Fact]
    public async Task LogoutAsync_ShouldClearCookies_AndReturnNoContent()
    {
        // Arrange
        var mockResponse = new Mock<HttpResponse>();
        var mockCookies = new Mock<IResponseCookies>();
        var mockContext = new Mock<HttpContext>();

        mockResponse.Setup(r => r.Cookies).Returns(mockCookies.Object);
        mockContext.Setup(c => c.Response).Returns(mockResponse.Object);
        _mockHttpAccessor.Setup(a => a.HttpContext).Returns(mockContext.Object);

        // Act
        var result = await _authService.LogoutAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(204);
        mockCookies.Verify(c => c.Delete("X-Access-Token", It.IsAny<CookieOptions>()), Times.Once);
    }
    
    [Fact]
    public async Task GetCurrentDetailsAsync_ShouldReturnFailure_WhenUserNotAuthenticated()
    {
        // Arrange
        var mockContext = new Mock<HttpContext>();
        mockContext.Setup(c => c.User).Returns(new System.Security.Claims.ClaimsPrincipal()); // No claims
        _mockHttpAccessor.Setup(a => a.HttpContext).Returns(mockContext.Object);

        // Act
        var result = await _authService.GetCurrentDetailsAsync();

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(401);
    }
    
    [Fact]
    public async Task GetCurrentDetailsAsync_ShouldReturnDetails_WhenAuthenticated()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var userId = Guid.NewGuid();
    
        var customer = new RetailCore.Domain.Entities.Customer 
        { 
            Id = customerId, 
            UserId = userId, 
            Email = "test@gmail.com",
            FullName = "Test User"
        };

        // Giả lập ClaimsPrincipal
        var claims = new List<Claim> { new("customerId", customerId.ToString()) };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var userPrincipal = new ClaimsPrincipal(identity);

        var mockContext = new Mock<HttpContext>();
        mockContext.Setup(c => c.User).Returns(userPrincipal);
        _mockHttpAccessor.Setup(a => a.HttpContext).Returns(mockContext.Object);

        _mockCustomerRepo.Setup(r => r.GetByIdAsync(customerId))
            .ReturnsAsync(customer); 

        // Setup UserManager
        var user = new IdentityUser<Guid> { Id = userId, Email = "test@gmail.com" };
        _mockUserManager.Setup(m => m.FindByIdAsync(userId.ToString()))
            .ReturnsAsync(user);
        
        _mockUserManager.Setup(m => m.GetRolesAsync(It.IsAny<IdentityUser<Guid>>()))
            .ReturnsAsync(new List<string> { "Customer" });

        // Act
        var result = await _authService.GetCurrentDetailsAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Email.Should().Be("test@gmail.com");
    }
    
}