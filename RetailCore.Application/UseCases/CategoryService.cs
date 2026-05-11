using Microsoft.AspNetCore.Http;
using RetailCore.Application.Extensions;
using RetailCore.Application.Mappings;

namespace RetailCore.Application.UseCases;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IProductRepository _productRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CategoryService(
        ICategoryRepository categoryRepository,
        IProductRepository productRepository,
        IHttpContextAccessor httpContextAccessor
        )
    {
        _categoryRepository = categoryRepository;
        _productRepository = productRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result<PagingResponse<CategoryResponse>>> GetByPageAsync(PagingRequest request)
    {
        PagingResponse<Category> categories = await _categoryRepository.GetByPageAsync(request);
        return Result<PagingResponse<CategoryResponse>>.Success(CategoryMapping.ToPagingResponse(categories));
    }

    public async Task<Result<CategoryResponse>> GetByIdAsync(Guid id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null)
        {
            return Result<CategoryResponse>.Failure("Category not found");
        }
        return Result<CategoryResponse>.Success(CategoryMapping.ToResponse(category));
    }

    public async Task<Result<Guid>> CreateAsync(CreateCategoryRequest request)
    {
        var category = CategoryMapping.ToEntityCreate(request);
        await _categoryRepository.AddAsync(category);
        await _categoryRepository.SaveChangesAsync();
        return Result<Guid>.Success(category.Id);
    }

    public async Task<Result<bool>> UpdateAsync(Guid id, UpdateCategoryRequest request)
    {
        Category category = await _categoryRepository.GetByIdAsync(id);
        if (category == null)
        {
            return Result<bool>.Failure("Category not found");
        }
        Guid userId = _httpContextAccessor.HttpContext.User.GetUserId(); 
        CategoryMapping.ToEntityUpdate(category, request, userId);
        
        _categoryRepository.Update(category); 
        await _categoryRepository.SaveChangesAsync();
        
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        Category category = await _categoryRepository.GetByIdAsync(id);
        if (category == null)
        {
            return Result<bool>.Failure("Category not found to delete");
        }

        if (await _productRepository.ExistsByCategoryIdAsync(id))
        {
            return Result<bool>.Failure("Category has products");
        }
        
        _categoryRepository.Delete(category);
        await _categoryRepository.SaveChangesAsync();
        
        return Result<bool>.Success(true);
    }
}