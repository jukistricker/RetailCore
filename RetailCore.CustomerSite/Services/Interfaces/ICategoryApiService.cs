namespace RetailCore.CustomerSite.Services.Interfaces;

public interface ICategoryApiService
{
    Task<Result<PagingResponse<CategoryResponse>>> GetByPageAsync(PagingRequest request);
}