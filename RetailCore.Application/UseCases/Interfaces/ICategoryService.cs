namespace RetailCore.Application.UseCases.Interfaces;

public interface ICategoryService
{
    Task<Result<PagingResponse<CategoryResponse>>> GetByPageAsync(PagingRequest request);
    Task<Result<CategoryResponse>> GetByIdAsync(Guid id);
    Task<Result<Guid>> CreateAsync(CreateCategoryRequest request);
    Task<Result<bool>> UpdateAsync(UpdateCategoryRequest request);
    Task<Result<bool>> DeleteAsync(Guid id);
}