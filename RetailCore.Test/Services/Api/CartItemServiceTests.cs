using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using RetailCore.Application.UseCases;
using RetailCore.Domain.Entities;
using RetailCore.Domain.Interfaces.Repositories;
using RetailCore.Shared.DTOs;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MockQueryable;
using RetailCore.Infrastructure.Data;
using MockQueryable.Moq;

namespace RetailCore.Test.Services.Api;

public class CartItemServiceTests
{
    private readonly Mock<ICartItemRepository> _mockCartItemRepo;
    private readonly Mock<IProductRepository> _mockProductRepo;
    private readonly Mock<IProductAttributeRepository> _mockAttrRepo;
    private readonly Mock<IHttpContextAccessor> _mockHttpAccessor;
    private readonly CartItemService _cartItemService;
    private readonly AppDbContext _dbContext;
    private readonly Guid _currentCustomerId = Guid.NewGuid();

    public CartItemServiceTests()
    {
        // Setup In-Memory DB
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning)) // Thêm dòng này
            .Options;
        _dbContext = new AppDbContext(options);

        // Mock Repositories
        _mockCartItemRepo = new Mock<ICartItemRepository>();
        _mockProductRepo = new Mock<IProductRepository>();
        _mockAttrRepo = new Mock<IProductAttributeRepository>();
        _mockHttpAccessor = new Mock<IHttpContextAccessor>();

        // Setup User Claims (Giả lập đã đăng nhập)
        var claims = new[] { new Claim("customerId", _currentCustomerId.ToString()) };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = user };
        _mockHttpAccessor.Setup(a => a.HttpContext).Returns(httpContext);

        _cartItemService = new CartItemService(
            _mockCartItemRepo.Object,
            _mockHttpAccessor.Object,
            _mockAttrRepo.Object,
            _mockProductRepo.Object
        );
        
    }
    
    [Fact]
    public async Task AddToCartAsync_ShouldCreateNewItem_WhenItemDoesNotExist()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var request = new CartItemSaveRequest { ProductId = productId, Quantity = 2 };
        var product = new Product { Id = productId, Stock = 10 };

        // Giả lập giỏ hàng trống (GetQueryable trả về list trống)
        var emptyCart = new List<CartItem>().AsQueryable();
        _mockCartItemRepo.Setup(r => r.GetQueryable()).Returns(emptyCart);
        _mockProductRepo.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync(product);

        // Act
        var result = await _cartItemService.AddToCartAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _mockCartItemRepo.Verify(r => r.AddAsync(It.IsAny<CartItem>()), Times.Once);
        _mockCartItemRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
    
    [Fact]
    public async Task AddToCartAsync_ShouldReturnFailure_WhenTotalQuantityExceedsStock()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = new Product { Id = productId, Stock = 5 }; 
        var existingItem = new CartItem 
        { 
            ProductId = productId, 
            CustomerId = _currentCustomerId, 
            Quantity = 3, 
            Product = product 
        };

        var request = new CartItemSaveRequest { ProductId = productId, Quantity = 3 };

        // TẠO DỮ LIỆU MOCK HỖ TRỢ ASYNC
        // Sử dụng BuildMock() từ thư viện MockQueryable.Moq
        var mockQueryable = new List<CartItem> { existingItem }.BuildMock();

        // SETUP REPO - Ép kiểu rõ ràng để tránh lỗi Ambiguous
        _mockCartItemRepo.Setup(r => r.GetQueryable())
            .Returns(mockQueryable); // Nếu vẫn báo lỗi, hãy dùng .Returns(() => mockQueryable);

        // PHẢI MOCK THÊM PRODUCT REPO
        // Vì nếu FirstOrDefaultAsync không tìm thấy item (do sai ID/Criteria), 
        // code sẽ chạy xuống nhánh check Product.
        _mockProductRepo.Setup(r => r.GetByIdAsync(productId))
            .ReturnsAsync(product);

        // Act
        var result = await _cartItemService.AddToCartAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Message == "Total quantity exceeds stock");
    }
    
    [Fact]
    public async Task UpdateAsync_ShouldMergeItems_WhenChangingToExistingAttribute()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var otherItemId = Guid.NewGuid();
        var targetAttrId = Guid.NewGuid();
    
        var currentItem = new CartItem { Id = itemId, ProductId = Guid.NewGuid(), ProductAttributeId = null, Quantity = 1 };
        var existingOtherItem = new CartItem { Id = otherItemId, ProductId = currentItem.ProductId, ProductAttributeId = targetAttrId, Quantity = 2 };

        var request = new CartItemUpdateRequest { CartItemId = itemId, ProductAttributeId = targetAttrId, Quantity = 2 };

        // Mock Queryable chứa cả 2 item
        var queryData = new List<CartItem> { currentItem, existingOtherItem }.BuildMock();
            
        _mockCartItemRepo.Setup(r => r.GetQueryable()).Returns(queryData);
        _mockAttrRepo.Setup(r => r.GetByIdAsync(targetAttrId)).ReturnsAsync(new ProductAttribute { Id = targetAttrId, Stock = 10 });

        // Act
        var result = await _cartItemService.UpdateAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        existingOtherItem.Quantity.Should().Be(4); // 2 cũ + 2 mới
        _mockCartItemRepo.Verify(r => r.DeleteAsync(It.Is<List<Guid>>(l => l.Contains(itemId)), It.IsAny<Guid>()), Times.Once);
    }
    
    
}