namespace RetailCore.Application.UseCases.Interfaces;

public interface IProductService
{
    Task<Result<IEnumerable<ProductSummaryResponse>>> GetFeaturedProductsAsync(int count);

    Task<Result<PagingResponse<ProductSummaryResponse>>> GetProductsAsync(
        PagingRequest request,
        Guid? categoryId = null,
        string? slug = null);

    Task<Result<ProductDetailResponse>> GetProductDetailAsync(Guid id);
    Task<Result<bool>> CreateProductAsync(ProductCreateRequest request);
    Task<Result<bool>> UpdateProductAsync(Guid id, ProductUpdateRequest request);
    Task<Result<bool>> DeleteProductAsync(Guid id);
}