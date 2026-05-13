using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RetailCore.Application.Extensions;
using RetailCore.Application.Mappings;

namespace RetailCore.Application.UseCases;

public class ProductService : IProductService
{
    private readonly AppDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IProductAttributeRepository _productAttributeRepository;
    private readonly IValidator<ProductCreateRequest> _productCreateRequestValidator;
    private readonly string _productImageFolder;
    private readonly IProductRepository _productRepository;
    private readonly IValidator<ProductUpdateRequest> _productUpdateRequestValidator;
    private readonly IStorageService _storageService;
    private readonly string _userContentFolder;

    public ProductService(
        IProductRepository productRepository,
        IConfiguration configuration,
        IStorageService storageService,
        AppDbContext dbContext,
        IHttpContextAccessor httpContextAccessor,
        IValidator<ProductCreateRequest> productCreateRequestValidator,
        IValidator<ProductUpdateRequest> productUpdateRequestValidator,
        IProductAttributeRepository productAttributeRepository)
    {
        _productRepository = productRepository;
        _userContentFolder = configuration["StorageConfig:UserContentFolder"] ?? "user-content";
        _productImageFolder = configuration["StorageConfig:ProductImageFolder"] ?? "products";
        _storageService = storageService;
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
        _productCreateRequestValidator = productCreateRequestValidator;
        _productUpdateRequestValidator = productUpdateRequestValidator;
        _productAttributeRepository = productAttributeRepository;
    }

    public async Task<Result<IEnumerable<ProductSummaryResponse>>> GetFeaturedProductsAsync(int count)
    {
        var products = await _productRepository.GetByFeaturedAsync(count);

        return Result<IEnumerable<ProductSummaryResponse>>.Success(ProductMapping.ToFeaturedProductList(products));
    }

    public async Task<Result<PagingResponse<ProductSummaryResponse>>> GetProductsAsync(
        PagingRequest request,
        Guid? categoryId = null,
        string? slug = null)
    {
        var query = _productRepository.GetQueryable()
            .AsNoTracking()
            .Where(p => p.IsActive);

        if (!string.IsNullOrWhiteSpace(request.Search))
            query = query.Where(p => p.Name.Contains(request.Search));

        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId.Value);

        if (!string.IsNullOrEmpty(slug))
            query = query.Where(p => p.Slug == slug);

        var response = await _productRepository.GetByPageAsync(query, request.PageNumber, request.PageSize);

        return Result<PagingResponse<ProductSummaryResponse>>.Success(
            ProductMapping.ToProductSummayResponseList(response));
    }

    public async Task<Result<ProductDetailResponse>> GetProductDetailAsync(Guid id)
    {
        var product = await _productRepository.GetDetailAsync(id);

        if (product == null) return Result<ProductDetailResponse>.Failure("Id", "Product not found");

        return Result<ProductDetailResponse>.Success(ProductMapping.ToProductDetailResponse(product));
    }


    public async Task<Result<bool>> CreateProductAsync(ProductCreateRequest request)
    {
        var validationResult = await _productCreateRequestValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            var errorDetails = validationResult.Errors.Select(e => new ErrorDetail
            {
                Key = e.PropertyName,
                Message = e.ErrorMessage
            }).ToList();

            return Result<bool>.Failure(errorDetails);
        }

        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            if (await _productRepository.GetQueryable().AnyAsync(p => p.Slug == request.Slug))
                request.Slug = $"{request.Slug}-{DateTime.Now.Ticks % 10000}";
            var userId = _httpContextAccessor.HttpContext.User.GetCustomerId();
            var product = ProductMapping.ToEntityCreate(request, userId);

            await AddProductImagesAsync(product, request.Images, userId);

            if (!string.IsNullOrEmpty(request.Attributes))
                try
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var attributeList =
                        JsonSerializer.Deserialize<List<ProductAttributeRequest>>(request.Attributes, options);

                    if (attributeList != null && attributeList.Any())
                        await CreateProductAttribute(product, attributeList);
                }
                catch (JsonException ex)
                {
                    return Result<bool>.Failure("Attributes", "Attributes have invalid json format.");
                }

            await _productRepository.AddAsync(product);
            await _productRepository.SaveChangesAsync();
            await _productAttributeRepository.SaveChangesAsync();
            await transaction.CommitAsync();
            return Result<bool>.Success(true, 204);
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            return Result<bool>.Failure(null, "Failed to create product", 500);
        }
    }

    public async Task<Result<bool>> UpdateProductAsync(Guid id, ProductUpdateRequest request)
    {
        var validationResult = await _productUpdateRequestValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            var errorDetails = validationResult.Errors.Select(e => new ErrorDetail
            {
                Key = e.PropertyName,
                Message = e.ErrorMessage
            }).ToList();

            return Result<bool>.Failure(errorDetails);
        }

        var userId = _httpContextAccessor.HttpContext.User.GetCustomerId();
        var product = await _productRepository.GetQueryable()
            .Include(p => p.ProductImages)
            .Include(p => p.ProductAttributes)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null) return Result<bool>.Failure("Id", "Product not found");

        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            ProductMapping.ToEntityUpdate(product, request, userId);

            if (product.Slug != request.Slug &&
                await _productRepository.GetQueryable().AnyAsync(p => p.Slug == request.Slug))
                product.Slug = $"{request.Slug}-{DateTime.Now.Ticks % 10000}";
            else
                product.Slug = request.Slug;

            if (product.ProductImages.Any())
            {
                var oldFileNames = product.ProductImages
                    .Select(x => Path.GetFileName(x.Url))
                    .ToList();

                _storageService.DeleteFiles(oldFileNames, _productImageFolder);
                product.ProductImages.Clear();
            }

            await AddProductImagesAsync(product, request.Images, userId);

            product.ProductAttributes.Clear();

            if (!string.IsNullOrEmpty(request.Attributes))
                try
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var attributeList =
                        JsonSerializer.Deserialize<List<ProductAttributeRequest>>(request.Attributes, options);

                    if (attributeList != null && attributeList.Any())
                        await CreateProductAttribute(product, attributeList);
                }
                catch (JsonException ex)
                {
                    return Result<bool>.Failure("Attributes", "Attributes have invalid json format.");
                }

            _productRepository.Update(product);
            await _productRepository.SaveChangesAsync();
            await _productAttributeRepository.SaveChangesAsync();
            await transaction.CommitAsync();

            return Result<bool>.Success(true, 204);
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            return Result<bool>.Failure(null, "Failed to update product", 500);
        }
    }

    public async Task<Result<bool>> DeleteProductAsync(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null) return Result<bool>.Failure("Id", "Product not found.");

        product.IsActive = false;

        await _productRepository.SaveChangesAsync();
        return Result<bool>.Success(true, 204);
    }

    private async Task CreateProductAttribute(Product product, List<ProductAttributeRequest> requests)
    {
        // tối đa 2 tầng
        foreach (var request in requests)
        {
            var parentAttribute = ProductAttributeMapping.ToEntityCreate(product, request, null);
            product.ProductAttributes.Add(parentAttribute);
            if (request.ChildAttributes != null && request.ChildAttributes.Any())
                foreach (var childAttr in request.ChildAttributes)
                {
                    var childAttribute = ProductAttributeMapping.ToEntityCreate(product, childAttr, parentAttribute.Id);
                    product.ProductAttributes.Add(childAttribute);
                }
        }
    }

    private async Task AddProductImagesAsync(Product product, List<IFormFile> images, Guid userId)
    {
        var fileNames = await _storageService.SaveFilesAsync(images, _productImageFolder);
        foreach (var name in fileNames)
            product.ProductImages.Add(new ProductImage
            {
                Url = $"/{_userContentFolder}/{_productImageFolder}/{name}",
                ProductId = product.Id,
                CreatedDate = DateTime.Now,
                CreatedBy = userId
            });
        product.ThumbnailUrl = product.ProductImages.FirstOrDefault()?.Url;
    }
}