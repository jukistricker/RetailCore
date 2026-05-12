using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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
        IQueryable<Category> query = _categoryRepository.GetQueryable().AsNoTracking();
        PagingResponse<Category> response = await _categoryRepository.GetByPageAsync(query,request.PageNumber, request.PageSize);
        return Result<PagingResponse<CategoryResponse>>.Success(CategoryMapping.ToPagingResponse(response));
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
        Guid userId = _httpContextAccessor.HttpContext.User.GetCustomerId();
        if (await _categoryRepository.GetQueryable().AsNoTracking()
                .AnyAsync(c => c.Name == request.Name))
        {
            return Result<Guid>.Failure("Category name already exists");
        }
        Category category = CategoryMapping.ToEntityCreate(request, userId);
        await _categoryRepository.AddAsync(category);
        await _categoryRepository.SaveChangesAsync();
        return Result<Guid>.Success(category.Id);
    }

    public async Task<Result<bool>> UpdateAsync(UpdateCategoryRequest request)
    {
        Category? category = await _categoryRepository.GetByIdAsync(request.Id);
        if (category == null)
        {
            return Result<bool>.Failure("Category not found");
        }
        if (category.Name!=request.Name && await _categoryRepository.GetQueryable()
                .AnyAsync(c => c.Name == request.Name && c.Id != request.Id))
        {
            return Result<bool>.Failure("Category name already exists");
        }
        Guid userId = _httpContextAccessor.HttpContext.User.GetCustomerId(); 
        CategoryMapping.ToEntityUpdate(category, request, userId);
        
        _categoryRepository.Update(category); 
        await _categoryRepository.SaveChangesAsync();
        
        return Result<bool>.Success(true,204);
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
        
        return Result<bool>.Success(true,204);
    }
}