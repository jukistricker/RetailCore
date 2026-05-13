using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RetailCore.API.Controllers;
using RetailCore.Application.UseCases.Interfaces;
using RetailCore.Domain.Commons;
using RetailCore.Shared.DTOs;

namespace RetailCore.Test.Controllers.Api;

public class CartControllerTests
{
    private readonly Mock<ICartItemService> _mockCartService;
    private readonly CartController _controller;

    public CartControllerTests()
    {
        _mockCartService = new Mock<ICartItemService>();
        _controller = new CartController(_mockCartService.Object);
        
        // Lưu ý: Nếu HandleResult hoặc Service của bạn cần lấy UserId từ Token, 
        // hãy setup HttpContext giống như file AuthControllerTests bạn đã làm.
    }

    [Fact]
    public async Task AddToCartAsync_ShouldReturnOk_WhenSuccess()
    {
        // Arrange
        var request = new CartItemSaveRequest { ProductId = Guid.NewGuid(), Quantity = 2 };
        _mockCartService.Setup(s => s.AddToCartAsync(request))
            .ReturnsAsync(Result<bool>.Success(true));

        // Act
        var result = await _controller.AddToCartAsync(request);

        // Assert
        var objResult = result.Should().BeOfType<ObjectResult>().Subject;
        objResult.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task GetCartItemsAsync_ShouldReturnPagedList_WhenRequestIsValid()
    {
        // Arrange
        var request = new PagingRequest { PageNumber = 1, PageSize = 10 };
        var pagedData = new PagingResponse<CartItemResponse> 
        { 
            Items = [], 
            TotalCount = 0, 
            PageNumber = 1, 
            PageSize = 10 
        };
        _mockCartService.Setup(s => s.GetCartItemsAsync(request))
            .ReturnsAsync(Result<PagingResponse<CartItemResponse>>.Success(pagedData));

        // Act
        var result = await _controller.GetCartItemsAsync(request);

        // Assert
        var objResult = result.Should().BeOfType<ObjectResult>().Subject;
        objResult.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task DeleteCartItemsAsync_ShouldReturnOk_WhenItemsAreDeleted()
    {
        // Arrange
        var ids = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        _mockCartService.Setup(s => s.DeleteCartItemsAsync(ids))
            .ReturnsAsync(Result<bool>.Success(true));

        // Act
        var result = await _controller.DeleteCartItemsAsync(ids);

        // Assert
        var objResult = result.Should().BeOfType<ObjectResult>().Subject;
        objResult.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task UpdateCartItemAsync_ShouldReturnOk_WhenUpdateSuccess()
    {
        // Arrange
        var request = new CartItemUpdateRequest { CartItemId = Guid.NewGuid(), Quantity = 5 };
        _mockCartService.Setup(s => s.UpdateAsync(request))
            .ReturnsAsync(Result<bool>.Success(true));

        // Act
        var result = await _controller.UpdateCartItemAsync(request);

        // Assert
        var objResult = result.Should().BeOfType<ObjectResult>().Subject;
        objResult.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task AddToCartAsync_ShouldReturnBadRequest_WhenQuantityIsInvalid()
    {
        // Arrange
        var request = new CartItemSaveRequest { ProductId = Guid.NewGuid(), Quantity = -1 };
        var failureResult = Result<bool>.Failure("Quantity", "Quantity must be greater than 0", 400);
        
        _mockCartService.Setup(s => s.AddToCartAsync(request))
            .ReturnsAsync(failureResult);

        // Act
        var result = await _controller.AddToCartAsync(request);

        // Assert
        var badRequest = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequest.StatusCode.Should().Be(400);
        badRequest.Value.Should().BeEquivalentTo(failureResult);
    }
}