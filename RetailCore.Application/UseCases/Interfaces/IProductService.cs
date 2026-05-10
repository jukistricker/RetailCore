// namespace RetailCore.Application.UseCases.Interfaces;
//
// public interface IProductService
// {
//     // Admin CRUD
//     Task<PagedResponse<ProductResponse>> GetPagedAsync(int pageIndex, int pageSize);
//     Task<ProductResponse?> GetByIdAsync(Guid id);
//     Task<Guid> CreateAsync(CreateProductRequest request);
//     Task<bool> UpdateAsync(Guid id, UpdateProductRequest request);
//     Task<bool> DeleteAsync(Guid id);
//
//     // Customer Portal logic (Week 2)
//     Task<IEnumerable<ProductResponse>> GetByCategoryIdAsync(Guid categoryId);
//     Task<IEnumerable<ProductResponse>> GetFeaturedProductsAsync(int count);
// }