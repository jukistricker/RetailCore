using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;

namespace RetailCore.Application.UseCases;

public class FileStorageService : IStorageService
{
    private readonly string _userContentFolder;
    private readonly string _folderName; 

    public FileStorageService(IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
    {
        _folderName = configuration["StorageConfig:UserContentFolder"] ?? "user-content";
        _userContentFolder = Path.Combine(webHostEnvironment.WebRootPath, _folderName);
    }

    public async Task<string> SaveFileAsync(IFormFile file, string folderName)
    {
        string originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.ToString().Trim('"');
        string fileName = $"{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";
        
        string outputFolder = Path.Combine(_userContentFolder, folderName);
        if (!Directory.Exists(outputFolder)) Directory.CreateDirectory(outputFolder);

        string filePath = Path.Combine(outputFolder, fileName);
        using FileStream output = File.Create(filePath);
        await file.OpenReadStream().CopyToAsync(output);

        return fileName; 
    }

    public void DeleteFile(string fileName, string folderName)
    {
        string filePath = Path.Combine(_userContentFolder, folderName, fileName);
        if (File.Exists(filePath)) File.Delete(filePath);
    }

    public async Task<List<string>> SaveFilesAsync(List<IFormFile> files, string folderName)
    {
        List<string> savedFiles = new List<string>();
        foreach (IFormFile file in files)
        {
            if (file.Length > 0) 
            {
                string fileName = await SaveFileAsync(file, folderName);
                savedFiles.Add(fileName);
            }
        }
        return savedFiles;
    }

    public void DeleteFiles(List<string> fileNames, string folderName)
    {
        foreach (string fileName in fileNames)
        {
            DeleteFile(fileName, folderName);
        }
    }
}