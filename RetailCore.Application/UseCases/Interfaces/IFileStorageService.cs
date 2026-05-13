using Microsoft.AspNetCore.Http;

namespace RetailCore.Application.UseCases.Interfaces;

public interface IFileStorageService
{
    Task<string> UploadFileAsync(IFormFile file, string folderName);
    Task<bool> DeleteFileAsync(string fileName);
    string GetPublicUrl(string fileName);
}