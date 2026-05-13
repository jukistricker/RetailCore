using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RetailCore.API.Controllers;
using RetailCore.Application.UseCases.Interfaces;
using RetailCore.Domain.Commons;
using RetailCore.Shared.DTOs;

namespace RetailCore.Test.Controllers.Api;

public class ProductControllerTests
{
    private readonly Mock<IProductService> _mockProductService;
    private readonly ProductController _controller;

    public ProductControllerTests()
    {
        _mockProductService = new Mock<IProductService>();
        _controller = new ProductController(_mockProductService.Object);
    }

    [Fact]
    public async Task Update_ShouldReturnBadRequest_WhenIdMismatch()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new ProductUpdateRequest { Id = Guid.NewGuid() }; // ID khác với id route

        // Act
        var result = await _controller.Update(id, request);

        // Assert
        var badRequest = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequest.Value.Should().Be("ID mismatch");
    }

    [Fact]
    public async Task GetByPage_ShouldReturnPagedProducts_WhenFilterApplied()
    {
        // Arrange
        var request = new PagingRequest { PageNumber = 1, PageSize = 10 };
        var categoryId = Guid.NewGuid();
        var pagedData = new PagingResponse<ProductSummaryResponse> { Items = [], TotalCount = 0 };
        
        _mockProductService.Setup(s => s.GetProductsAsync(request, categoryId, null))
            .ReturnsAsync(Result<PagingResponse<ProductSummaryResponse>>.Success(pagedData));

        // Act
        var result = await _controller.GetByPage(request, categoryId, null);

        // Assert
        var objResult = result.Should().BeOfType<ObjectResult>().Subject;
        objResult.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task Create_ShouldReturnOk_WhenProductIsCreated()
    {
        // Arrange
        var request = new ProductCreateRequest { Name = "iPhone 15", Price = 1000 };
        _mockProductService.Setup(s => s.CreateProductAsync(request))
            .ReturnsAsync(Result<bool>.Success(true));

        // Act
        var result = await _controller.Create(request);

        // Assert
        var objResult = result.Should().BeOfType<ObjectResult>().Subject;
        objResult.StatusCode.Should().Be(200);
    }
    
    [Fact]
    public async Task GetById_ShouldReturnProduct_WhenExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        var response = new ProductDetailResponse { Id = id, Name = "Gaming Chair" };
        _mockProductService.Setup(s => s.GetProductDetailAsync(id))
            .ReturnsAsync(Result<ProductDetailResponse>.Success(response));

        // Act
        var result = await _controller.GetById(id);

        // Assert
        var objResult = result.Should().BeOfType<ObjectResult>().Subject;
        objResult.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task Update_ShouldReturnOk_WhenIdMatchesAndServiceSucceeds()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new ProductUpdateRequest { Id = id, Name = "Updated Name" };
        _mockProductService.Setup(s => s.UpdateProductAsync(id, request))
            .ReturnsAsync(Result<bool>.Success(true));

        // Act
        var result = await _controller.Update(id, request);

        // Assert
        var objResult = result.Should().BeOfType<ObjectResult>().Subject;
        objResult.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task Delete_ShouldReturnNoContent_WhenDeleted()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockProductService.Setup(s => s.DeleteProductAsync(id))
            .ReturnsAsync(Result<bool>.Success(true, 204));

        // Act
        var result = await _controller.Delete(id);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task GetFeatured_ShouldReturnList_WithSpecifiedCount()
    {
        // Arrange
        int count = 5;
        var featuredProducts = new List<ProductSummaryResponse> 
        { 
            new() { Id = Guid.NewGuid(), Name = "Featured 1" } 
        };
        _mockProductService.Setup(s => s.GetFeaturedProductsAsync(count))
            .ReturnsAsync(Result<IEnumerable<ProductSummaryResponse>>.Success(featuredProducts));

        // Act
        var result = await _controller.GetFeatured(count);

        // Assert
        var objResult = result.Should().BeOfType<ObjectResult>().Subject;
        objResult.StatusCode.Should().Be(200);
    }
}