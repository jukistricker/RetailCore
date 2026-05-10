namespace RetailCore.Application.UseCases.Interfaces;

public interface ICategoryService
{
    Task<Result<IEnumerable<CategoryResponse>>> GetAllAsync();
    Task<Result<CategoryResponse?>> GetByIdAsync(Guid id);
    Task<Result<Guid>> CreateAsync(CreateCategoryRequest request);
    Task<Result<bool>> UpdateAsync(Guid id, UpdateCategoryRequest request);
    Task<Result<bool>> DeleteAsync(Guid id);
}