using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RetailCore.API.Controllers;
using RetailCore.Application.UseCases.Interfaces;
using RetailCore.Domain.Commons;
using RetailCore.Shared.DTOs;

namespace RetailCore.Test.Controllers.Api;

public class CustomerControllerTests
{
    private readonly Mock<ICustomerService> _mockService;
    private readonly CustomerController _controller;

    public CustomerControllerTests()
    {
        _mockService = new Mock<ICustomerService>();
        _controller = new CustomerController(_mockService.Object);
    }

    [Fact]
    public async Task GetAllUsersAsync_ShouldReturnPagedCustomers()
    {
        // Arrange
        var request = new PagingRequest { PageNumber = 1, PageSize = 10 };
        var pagedResponse = new PagingResponse<CustomerResponse>
        {
            Items = [new CustomerResponse { FullName = "Thành" }],
            TotalCount = 1,
            PageNumber = 1,
            PageSize = 10
        };
        _mockService.Setup(s => s.GetAllUsersAsync(request))
            .ReturnsAsync(Result<PagingResponse<CustomerResponse>>.Success(pagedResponse));

        // Act
        var result = await _controller.GetAllUsersAsync(request);

        // Assert
        var objResult = result.Should().BeOfType<ObjectResult>().Subject;
        objResult.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task UpdateUserAsync_ShouldReturnOk_WhenUpdateValid()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new UpdateCustomerRequest { FullName = "Dương Ngọc Thành", Email = "thanh@ncs.vn" };
        _mockService.Setup(s => s.UpdateUserAsync(id, request))
            .ReturnsAsync(Result<bool>.Success(true));

        // Act
        var result = await _controller.UpdateUserAsync(id, request);

        // Assert
        var objResult = result.Should().BeOfType<ObjectResult>().Subject;
        objResult.StatusCode.Should().Be(200);
    }
}