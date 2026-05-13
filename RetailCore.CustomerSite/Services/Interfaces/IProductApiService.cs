namespace RetailCore.CustomerSite.Services.Interfaces;

public interface IProductApiService
{
    Task<Result<PagingResponse<ProductSummaryResponse>>> GetByPageAsync(PagingRequest request, Guid? categoryId,
        string? slug);

    Task<Result<ProductDetailResponse>> GetByIdAsync(Guid id);
    Task<Result<IEnumerable<ProductSummaryResponse>>> GetFeaturedAsync(int count);
}