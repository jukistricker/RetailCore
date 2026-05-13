using Microsoft.AspNetCore.Http;
using Moq;
using RetailCore.Application.UseCases;
using RetailCore.Domain.Entities;
using RetailCore.Domain.Interfaces.Repositories;
using System.Security.Claims;
using MockQueryable.Moq;
using FluentAssertions;
using MockQueryable;
using RetailCore.Shared.DTOs;
using Xunit;

namespace RetailCore.Test.Services;

public class CategoryServiceTests
{
    private readonly Mock<ICategoryRepository> _mockCategoryRepo;
    private readonly Mock<IProductRepository> _mockProductRepo;
    private readonly Mock<IHttpContextAccessor> _mockHttpAccessor;
    private readonly CategoryService _categoryService;
    private readonly Guid _currentUserId = Guid.NewGuid();

    public CategoryServiceTests()
    {
        _mockCategoryRepo = new Mock<ICategoryRepository>();
        _mockProductRepo = new Mock<IProductRepository>();
        _mockHttpAccessor = new Mock<IHttpContextAccessor>();

        // Setup HttpContext giả lập cho GetCustomerId()
        var claims = new[] { new Claim("customerId", _currentUserId.ToString()) };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = user };
        _mockHttpAccessor.Setup(a => a.HttpContext).Returns(httpContext);

        _categoryService = new CategoryService(
            _mockCategoryRepo.Object,
            _mockProductRepo.Object,
            _mockHttpAccessor.Object
        );
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnFailure_WhenNameAlreadyExists()
    {
        // Arrange
        var request = new CreateCategoryRequest { Name = "Laptop" };
        var existingCategories = new List<Category> 
        { 
            new Category { Id = Guid.NewGuid(), Name = "Laptop" } 
        }.BuildMock();

        _mockCategoryRepo.Setup(r => r.GetQueryable()).Returns(existingCategories);

        // Act
        var result = await _categoryService.CreateAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Message == "Category name already exists");
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnSuccess_WhenNameIsUnique()
    {
        // Arrange
        var request = new CreateCategoryRequest { Name = "New Category" };
        var emptyQuery = new List<Category>().BuildMock();
        
        _mockCategoryRepo.Setup(r => r.GetQueryable()).Returns(emptyQuery);

        // Act
        var result = await _categoryService.CreateAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _mockCategoryRepo.Verify(r => r.AddAsync(It.IsAny<Category>()), Times.Once);
        _mockCategoryRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFailure_WhenCategoryHasProducts()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = new Category { Id = categoryId, Name = "Electronics" };

        _mockCategoryRepo.Setup(r => r.GetByIdAsync(categoryId)).ReturnsAsync(category);
        // Giả lập danh mục đang có sản phẩm
        _mockProductRepo.Setup(r => r.ExistsByCategoryIdAsync(categoryId)).ReturnsAsync(true);

        // Act
        var result = await _categoryService.DeleteAsync(categoryId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Message == "Category has products");
        _mockCategoryRepo.Verify(r => r.Delete(It.IsAny<Category>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnSuccess_WhenCategoryIsEmpty()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = new Category { Id = categoryId, Name = "Empty Category" };

        _mockCategoryRepo.Setup(r => r.GetByIdAsync(categoryId)).ReturnsAsync(category);
        _mockProductRepo.Setup(r => r.ExistsByCategoryIdAsync(categoryId)).ReturnsAsync(false);

        // Act
        var result = await _categoryService.DeleteAsync(categoryId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _mockCategoryRepo.Verify(r => r.Delete(category), Times.Once);
        _mockCategoryRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnFailure_WhenIdNotFound()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        _mockCategoryRepo.Setup(r => r.GetByIdAsync(categoryId)).ReturnsAsync((Category)null);

        // Act
        var result = await _categoryService.GetByIdAsync(categoryId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Message == "Category not found");
    }
}