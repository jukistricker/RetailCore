using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MockQueryable;
using Moq;
using RetailCore.Application.UseCases;
using RetailCore.Domain.Entities;
using RetailCore.Domain.Interfaces.Repositories;
using RetailCore.Infrastructure.Data;
using RetailCore.Shared.DTOs;

namespace RetailCore.Test.Services.Api;

public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _mockOrderRepo;
    private readonly Mock<ICartItemRepository> _mockCartRepo;
    private readonly AppDbContext _dbContext;
    private readonly OrderService _orderService;
    private readonly Guid _customerId = Guid.NewGuid();

    public OrderServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        _dbContext = new AppDbContext(options);

        _mockOrderRepo = new Mock<IOrderRepository>();
        _mockCartRepo = new Mock<ICartItemRepository>();
        
        var mockHttp = new Mock<IHttpContextAccessor>();
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("customerId", _customerId.ToString()) }));
        mockHttp.Setup(h => h.HttpContext).Returns(new DefaultHttpContext { User = user });

        _orderService = new OrderService(_dbContext, _mockOrderRepo.Object, null, _mockCartRepo.Object, mockHttp.Object);
    }

    [Fact]
    public async Task CreateOrderAsync_ShouldThrowException_WhenStockIsNotEnough()
    {
        // Arrange
        var product = new Product { Id = Guid.NewGuid(), Name = "iPhone", Stock = 1, Price = 1000 };
        var cartItems = new List<CartItem> 
        { 
            new CartItem { ProductId = product.Id, Quantity = 5, Product = product } 
        }.BuildMock();

        _mockCartRepo.Setup(r => r.GetQueryable()).Returns(cartItems.AsQueryable());

        var request = new OrderCreateRequest { Items = new List<OrderItemRequest> { new() { ProductId = product.Id } } };

        // Act
        var result = await _orderService.CreateOrderAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Message.Contains("out of stock"));
    }
}