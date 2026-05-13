using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Moq;
using RetailCore.Application.UseCases;
using RetailCore.Domain.Entities;
using RetailCore.Domain.Interfaces.Repositories;
using System.Security.Claims;
using FluentAssertions;
using FluentValidation.Results;
using MockQueryable;
using RetailCore.Application.UseCases.Interfaces;
using RetailCore.Infrastructure.Data;
using RetailCore.Shared.DTOs;

namespace RetailCore.Test.Services.Api;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _mockProductRepo;
    private readonly Mock<IProductAttributeRepository> _mockAttrRepo;
    private readonly Mock<IStorageService> _mockStorage;
    private readonly Mock<IConfiguration> _mockConfig;
    private readonly Mock<IHttpContextAccessor> _mockHttpAccessor;
    private readonly Mock<IValidator<ProductCreateRequest>> _mockCreateValidator;
    private readonly Mock<IValidator<ProductUpdateRequest>> _mockUpdateValidator;
    private readonly AppDbContext _dbContext;
    private readonly ProductService _productService;
    private readonly Guid _currentUserId = Guid.NewGuid();

    public ProductServiceTests()
    {
        // 1. Setup In-Memory Database và lờ đi cảnh báo Transaction
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        _dbContext = new AppDbContext(options);

        // 2. Mock Repositories
        _mockProductRepo = new Mock<IProductRepository>();
        _mockAttrRepo = new Mock<IProductAttributeRepository>();
        _mockStorage = new Mock<IStorageService>();

        // 3. Mock Configuration (Để lấy folder lưu ảnh)
        _mockConfig = new Mock<IConfiguration>();
        _mockConfig.Setup(c => c["StorageConfig:UserContentFolder"]).Returns("user-content");
        _mockConfig.Setup(c => c["StorageConfig:ProductImageFolder"]).Returns("products");

        // 4. Mock HttpContext (Để lấy UserId của người tạo sản phẩm)
        _mockHttpAccessor = new Mock<IHttpContextAccessor>();
        var claims = new[] { new Claim("customerId", _currentUserId.ToString()) };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = user };
        _mockHttpAccessor.Setup(a => a.HttpContext).Returns(httpContext);

        // 5. Mock Validators
        _mockCreateValidator = new Mock<IValidator<ProductCreateRequest>>();
        _mockUpdateValidator = new Mock<IValidator<ProductUpdateRequest>>();

        // 6. Khởi tạo Service với đầy đủ các Mock
        _productService = new ProductService(
            _mockProductRepo.Object,
            _mockConfig.Object,
            _mockStorage.Object,
            _dbContext,
            _mockHttpAccessor.Object,
            _mockCreateValidator.Object,
            _mockUpdateValidator.Object,
            _mockAttrRepo.Object
        );
    }

    [Fact]
    public async Task GetProductDetailAsync_ShouldReturnSuccess_WhenProductExists()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = new Product { Id = productId, Name = "Test Product" };
        _mockProductRepo.Setup(r => r.GetDetailAsync(productId)).ReturnsAsync(product);

        // Act
        var result = await _productService.GetProductDetailAsync(productId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("Test Product");
    }

    [Fact]
    public async Task CreateProductAsync_ShouldReturnFailure_WhenRequestIsInvalid()
    {
        // Arrange
        var request = new ProductCreateRequest { Name = "" }; // Tên trống
        var validationFailures = new List<ValidationFailure> { new("Name", "Name is required") };
        _mockCreateValidator.Setup(v => v.ValidateAsync(request, default))
            .ReturnsAsync(new ValidationResult(validationFailures));

        // Act
        var result = await _productService.CreateProductAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Key == "Name");
    }
    
    [Fact]
    public async Task CreateProductAsync_ShouldAppendTicksToSlug_WhenSlugAlreadyExists()
    {
        // Arrange
        var request = new ProductCreateRequest { Name = "iPhone 15", Slug = "iphone-15" };
        var existingProduct = new Product { Slug = "iphone-15" };
    
        var query = new List<Product> { existingProduct }.BuildMock();
        
        _mockProductRepo.Setup(r => r.GetQueryable()).Returns(query);
    
        _mockCreateValidator.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(new ValidationResult());
        _mockStorage.Setup(s => s.SaveFilesAsync(It.IsAny<List<IFormFile>>(), It.IsAny<string>()))
            .ReturnsAsync(new List<string> { "img.jpg" });

        // Act
        var result = await _productService.CreateProductAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _mockProductRepo.Verify(r => r.AddAsync(It.Is<Product>(p => p.Slug.StartsWith("iphone-15-"))), Times.Once);
    }
    
    [Fact]
    public async Task CreateProductAsync_ShouldReturnFailure_WhenAttributesJsonIsInvalid()
    {
        // Arrange
        var request = new ProductCreateRequest 
        { 
            Name = "Product", 
            Attributes = "{ invalid json }" 
        };
        _mockCreateValidator.Setup(v => v.ValidateAsync(request, default)).ReturnsAsync(new ValidationResult());

        // Act
        var result = await _productService.CreateProductAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Message == "Attributes have invalid json format.");
    }
    
    [Fact]
    public async Task DeleteProductAsync_ShouldSetIsActiveToFalse_WhenProductExists()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = new Product { Id = productId, IsActive = true };
        _mockProductRepo.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync(product);

        // Act
        await _productService.DeleteProductAsync(productId);

        // Assert
        product.IsActive.Should().BeFalse();
        _mockProductRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}