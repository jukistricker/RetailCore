using RetailCore.CustomerSite.Services.Interfaces;
using RetailCore.Shared.Constants;

namespace RetailCore.CustomerSite.Services;

public class CategoryApiService: BaseApiService, ICategoryApiService
{
    public CategoryApiService(IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
        : base(httpClientFactory, configuration)
    {
    }
    
    public async Task<Result<PagingResponse<CategoryResponse>>> GetByPageAsync(PagingRequest request)
    {
        var queryString = $"?pageNumber={request.PageNumber}&pageSize={request.PageSize}";
        var response = await _httpClient.GetFromJsonAsync<Result<PagingResponse<CategoryResponse>>>($"{ApiRoutes.Categories.CategoryBase}{queryString}");
        return response?.IsSuccess == true ? response : null;
    }
    
}