using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RetailCore.API.Controllers;
using RetailCore.Application.UseCases.Interfaces;
using RetailCore.Domain.Commons;
using RetailCore.Shared.DTOs;
using Xunit;

namespace RetailCore.Test.Controllers.Api;

public class CategoryControllerTests
{
    private readonly Mock<ICategoryService> _mockService;
    private readonly CategoryController _controller;

    public CategoryControllerTests()
    {
        _mockService = new Mock<ICategoryService>();
        _controller = new CategoryController(_mockService.Object);
    }

    [Fact]
    public async Task Create_ShouldReturnOk_WhenSuccess()
    {
        // Arrange
        var request = new CreateCategoryRequest { Name = "Laptop", Description = "Gaming Laptops" };
        var categoryResponse = new CategoryResponse { Id = Guid.NewGuid(), Name = "Laptop" };
        _mockService.Setup(s => s.CreateAsync(request))
            .ReturnsAsync(Result<Guid>.Success(categoryResponse.Id));

        // Act
        var result = await _controller.Create(request);

        // Assert
        var objResult = result.Should().BeOfType<ObjectResult>().Subject;
        objResult.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNotFound_WhenCategoryDoesNotExist()
    {
        // Arrange
        var id = Guid.NewGuid();
        var failureResult = Result<CategoryResponse>.Failure("Category", "Not found", 404);
        _mockService.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(failureResult);

        // Act
        var result = await _controller.GetByIdAsync(id);

        // Assert
        var objResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
        objResult.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnNoContent_WhenDeletedSuccessfully()
    {
        // Arrange
        var id = Guid.NewGuid();
        var successResult = new Result<bool> { IsSuccess = true, StatusCode = 204 };
        _mockService.Setup(s => s.DeleteAsync(id)).ReturnsAsync(successResult);

        // Act
        var result = await _controller.DeleteAsync(id);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }
    
    [Fact]
    public async Task Update_ShouldReturnOk_WhenUpdateIsSuccessful()
    {
        // Arrange
        var request = new UpdateCategoryRequest { Id = Guid.NewGuid(), Name = "Updated Laptop" };
        _mockService.Setup(s => s.UpdateAsync(request))
            .ReturnsAsync(Result<bool>.Success(true));

        // Act
        var result = await _controller.Update(request);

        // Assert
        var objResult = result.Should().BeOfType<ObjectResult>().Subject;
        objResult.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnPagedCategories()
    {
        // Arrange
        var request = new PagingRequest { PageNumber = 1, PageSize = 10 };
        var pagedResponse = new PagingResponse<CategoryResponse>
        {
            Items = [new CategoryResponse { Name = "Electronic" }],
            TotalCount = 1,
            PageNumber = 1,
            PageSize = 10
        };
        _mockService.Setup(s => s.GetByPageAsync(request))
            .ReturnsAsync(Result<PagingResponse<CategoryResponse>>.Success(pagedResponse));

        // Act
        var result = await _controller.GetAllAsync(request);

        // Assert
        var objResult = result.Should().BeOfType<ObjectResult>().Subject;
        objResult.StatusCode.Should().Be(200);
        objResult.Value.Should().BeEquivalentTo(Result<PagingResponse<CategoryResponse>>.Success(pagedResponse));
    }

    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenNameIsDuplicate()
    {
        // Arrange
        var request = new CreateCategoryRequest { Name = "Existing Category" };
        var failureResult = Result<Guid>.Failure("Name", "Category name already exists", 400);
        
        _mockService.Setup(s => s.CreateAsync(request)).ReturnsAsync(failureResult);

        // Act
        var result = await _controller.Create(request);

        // Assert
        var badRequest = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequest.StatusCode.Should().Be(400);
        badRequest.Value.Should().BeEquivalentTo(failureResult);
    }
}