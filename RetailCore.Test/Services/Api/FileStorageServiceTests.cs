using System.Text;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using RetailCore.Application.UseCases;

namespace RetailCore.Test.Services.Api;

public class FileStorageServiceTests : IDisposable
{
    private readonly string _testPath;
    private readonly FileStorageService _storageService;

    public FileStorageServiceTests()
    {
        _testPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_testPath);

        var mockEnv = new Mock<IWebHostEnvironment>();
        mockEnv.Setup(e => e.WebRootPath).Returns(_testPath);

        var mockConfig = new Mock<IConfiguration>();
        mockConfig.Setup(c => c["StorageConfig:UserContentFolder"]).Returns("uploads");

        _storageService = new FileStorageService(mockEnv.Object, mockConfig.Object);
    }

    [Fact]
    public async Task SaveFileAsync_ShouldCreateFileOnDisk()
    {
        // Arrange
        var content = "fake image content";
        var fileName = "test.jpg";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        var file = new FormFile(stream, 0, stream.Length, "id_from_form", fileName);

        // Act
        var savedName = await _storageService.SaveFileAsync(file, "products");

        // Assert
        var expectedPath = Path.Combine(_testPath, "uploads", "products", savedName);
        File.Exists(expectedPath).Should().BeTrue();
    }

    public void Dispose()
    {
        if (Directory.Exists(_testPath)) Directory.Delete(_testPath, true);
    }
}