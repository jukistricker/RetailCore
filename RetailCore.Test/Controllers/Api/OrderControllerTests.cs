using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RetailCore.API.Controllers;
using RetailCore.Application.UseCases.Interfaces;
using RetailCore.Domain.Commons;
using RetailCore.Domain.Enums;
using RetailCore.Shared.DTOs;

namespace RetailCore.Test.Controllers.Api;

public class OrderControllerTests
{
    private readonly Mock<IOrderService> _mockOrderService;
    private readonly OrderController _controller;

    public OrderControllerTests()
    {
        _mockOrderService = new Mock<IOrderService>();
        _controller = new OrderController(_mockOrderService.Object);
    }

    [Fact]
    public async Task CreateOrderAsync_ShouldReturnOk_WhenOrderIsPlaced()
    {
        // Arrange
        var request = new OrderCreateRequest { ShippingAddress = "Hanoi, Vietnam", Items = [] };
        _mockOrderService.Setup(s => s.CreateOrderAsync(request))
            .ReturnsAsync(Result<Guid>.Success(Guid.NewGuid()));

        // Act
        var result = await _controller.CreateOrderAsync(request);

        // Assert
        var objResult = result.Should().BeOfType<ObjectResult>().Subject;
        objResult.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task UpdateOrderStatusAsync_ShouldReturnOk_WhenStatusChanged()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var status = OrderStatus.Pending;
        _mockOrderService.Setup(s => s.UpdateOrderStatusAsync(orderId, status))
            .ReturnsAsync(Result<Guid>.Success(orderId));

        // Act
        var result = await _controller.UpdateOrderStatusAsync(orderId, status);

        // Assert
        var objResult = result.Should().BeOfType<ObjectResult>().Subject;
        objResult.StatusCode.Should().Be(200);
    }
}