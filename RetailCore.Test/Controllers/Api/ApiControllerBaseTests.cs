using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using RetailCore.Domain.Commons;
using RetailCore.Test.Helpers;

namespace RetailCore.Test.Controllers.Api;

public class ApiControllerBaseTests
{
    private readonly FakeController _controller;

    public ApiControllerBaseTests()
    {
        _controller = new FakeController();
    }

    [Fact]
    public void HandleResult_ShouldReturnNotFound_WhenResultIsNull()
    {
        // Act
        var result = _controller.TestHandleResult<string>(null!);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public void HandleResult_ShouldReturnNoContent_WhenStatusCodeIs204()
    {
        // Arrange
        var response = new Result<string> { IsSuccess = true, StatusCode = 204 };

        // Act
        var result = _controller.TestHandleResult(response);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public void HandleResult_ShouldReturnObjectResult_WhenIsSuccessAndValueNotNull()
    {
        // Arrange
        var data = "Success Data";
        var response = Result<string>.Success(data, 200);

        // Act
        var result = _controller.TestHandleResult(response);

        // Assert
        var objectResult = result.Should().BeOfType<ObjectResult>().Subject;
        objectResult.StatusCode.Should().Be(200);
        objectResult.Value.Should().BeEquivalentTo(response);
    }

    [Theory]
    [InlineData(400, typeof(BadRequestObjectResult))]
    [InlineData(401, typeof(UnauthorizedObjectResult))]
    [InlineData(404, typeof(NotFoundObjectResult))]
    [InlineData(500, typeof(ObjectResult))]
    [InlineData(999, typeof(BadRequestObjectResult))] // Test trường hợp switch default (_)
    public void HandleResult_ShouldReturnCorrectErrorResult_WhenIsFailure(int statusCode, Type expectedType)
    {
        // Arrange
        var response = Result<string>.Failure("ErrorKey", "ErrorMessage", statusCode);

        // Act
        var result = _controller.TestHandleResult(response);

        // Assert
        result.Should().BeOfType(expectedType);
        if (result is ObjectResult obj)
        {
            obj.StatusCode.Should().Be(statusCode == 999 ? 400 : statusCode);
        }
    }

    [Fact]
    public void HandleResult_ShouldReturnForbid_WhenStatusCodeIs403()
    {
        // Arrange
        var response = Result<string>.Failure("Key", "Msg", 403);

        // Act
        var result = _controller.TestHandleResult(response);

        // Assert
        result.Should().BeOfType<ForbidResult>();
    }
}