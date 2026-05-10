using RetailCore.Application.Mappings;

namespace RetailCore.Application.UseCases;

public class CategoryService: ICategoryService
{
    private readonly  ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }
    
    
    public async Task<Result<IEnumerable<CategoryResponse>>> GetAllAsync()
    {
        IEnumerable<Category> categories = await _categoryRepository.GetAllAsync();
        return Result<IEnumerable<CategoryResponse>>.Success(CategoryMapping.ToResponses(categories));
    }

    public async Task<Result<CategoryResponse>?> GetByIdAsync(Guid id)
    {
        Category category = await _categoryRepository.GetByIdAsync(id);
        if (category == null)
        {
            Result<CategoryResponse?>.Failure("Category not found");
        }
        return Result<CategoryResponse>.Success(CategoryMapping.ToResponse(category));
    }

    public async Task<Result<Guid>> CreateAsync(CreateCategoryRequest request)
    {
        Category category = CategoryMapping.ToEntityCreate(request);
        await _categoryRepository.AddAsync(category);
        await _categoryRepository.SaveChangesAsync();
        return Result<Guid>.Success(category.Id);
    }

    public Task<Result<bool>> UpdateAsync(Guid id, UpdateCategoryRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}