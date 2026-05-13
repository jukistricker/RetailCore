using Microsoft.AspNetCore.WebUtilities;
using RetailCore.CustomerSite.Services.Interfaces;
using RetailCore.Shared.Constants;

namespace RetailCore.CustomerSite.Services;

public class ProductApiService : BaseApiService, IProductApiService
{
    public ProductApiService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        : base(httpClientFactory, configuration)
    {
    }

    public async Task<Result<ProductDetailResponse>> GetByIdAsync(Guid id)
    {
        // api/products/00000000-0000-0000-0000-000000000000
        var response = await _httpClient.GetAsync($"{ApiRoutes.Products.ProductBase}/{id}");

        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<Result<ProductDetailResponse>>();

        return Result<ProductDetailResponse>.Failure(null, "Product not found");
    }

    public async Task<Result<PagingResponse<ProductSummaryResponse>>> GetByPageAsync(PagingRequest request,
        Guid? categoryId, string? slug)
    {
        var queryParams = GetBaseQuery(request);

        if (categoryId.HasValue) queryParams["categoryId"] = categoryId.ToString();
        if (!string.IsNullOrEmpty(slug)) queryParams["slug"] = slug;

        var url = QueryHelpers.AddQueryString(ApiRoutes.Products.ProductBase, queryParams);

        return await _httpClient.GetFromJsonAsync<Result<PagingResponse<ProductSummaryResponse>>>(url);
    }

    public async Task<Result<IEnumerable<ProductSummaryResponse>>> GetFeaturedAsync(int count)
    {
        return await _httpClient.GetFromJsonAsync<Result<IEnumerable<ProductSummaryResponse>>>(
            $"{ApiRoutes.Products.ProductBase}/featured?count={count}");
    }
}