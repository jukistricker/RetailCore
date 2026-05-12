using Microsoft.AspNetCore.Http;

namespace RetailCore.Application.UseCases.Interfaces;

public interface IStorageService
{
    Task<string> SaveFileAsync(IFormFile file, string folderName);
    Task<List<string>> SaveFilesAsync(List<IFormFile> files, string folderName); 
    void DeleteFile(string fileName, string folderName);
    void DeleteFiles(List<string> fileNames, string folderName); 
}