using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using RetailCore.Application.UseCases;
using RetailCore.Domain.Entities;
using RetailCore.Domain.Interfaces.Repositories;
using RetailCore.Infrastructure.Data;
using RetailCore.Shared.DTOs;

namespace RetailCore.Test.Services.Api;

public class CustomerServiceTests
{
    private readonly Mock<ICustomerRepository> _mockCustomerRepo;
    private readonly Mock<UserManager<IdentityUser<Guid>>> _mockUserManager;
    private readonly Mock<IHttpContextAccessor> _mockHttpAccessor;
    private readonly AppDbContext _dbContext;
    private readonly CustomerService _customerService;

    public CustomerServiceTests()
    {
        // Setup DbContext In-Memory
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        _dbContext = new AppDbContext(options);

        _mockCustomerRepo = new Mock<ICustomerRepository>();
        _mockHttpAccessor = new Mock<IHttpContextAccessor>();
        
        // Mock UserManager (Identity yêu cầu setup rất nhiều tham số)
        var store = new Mock<IUserStore<IdentityUser<Guid>>>();
        _mockUserManager = new Mock<UserManager<IdentityUser<Guid>>>(
            store.Object, null, null, null, null, null, null, null, null);

        _customerService = new CustomerService(
            _mockCustomerRepo.Object, _dbContext, _mockUserManager.Object, _mockHttpAccessor.Object);
    }

    [Fact]
    public async Task UpdateUserAsync_ShouldReturnFailure_WhenEmailAlreadyInUse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var customer = new Customer { Id = userId, UserId = userId, Email = "old@test.com" };
        var request = new UpdateCustomerRequest { Email = "new@test.com" };

        _mockCustomerRepo.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(customer);
        _mockUserManager.Setup(m => m.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new IdentityUser<Guid>());
        
        // Giả lập email mới đã có người khác dùng
        _mockUserManager.Setup(m => m.FindByEmailAsync(request.Email))
            .ReturnsAsync(new IdentityUser<Guid> { Id = Guid.NewGuid() }); 

        // Act
        var result = await _customerService.UpdateUserAsync(userId, request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Message == "Email already in use");
    }
}