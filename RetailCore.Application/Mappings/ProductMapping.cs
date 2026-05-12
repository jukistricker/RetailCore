namespace RetailCore.Application.Mappings;

public static class ProductMapping
{
    public static IEnumerable<ProductSummaryResponse> ToFeaturedProductList(IEnumerable<Product> products)
    {
        return products.Select(p => new ProductSummaryResponse
        {
            Id = p.Id, 
            Name = p.Name, 
            Slug = p.Slug, 
            Price = p.Price, 
            ThumbnailUrl = p.ThumbnailUrl
        }).ToList();
    }

    public static ProductSummaryResponse ToProductSummaryResponse(Product product)
    {
        return new ProductSummaryResponse
        {
            Id = product.Id,
            Name = product.Name,
            Slug = product.Slug,
            Price = product.Price,
            ThumbnailUrl = product.ThumbnailUrl

        };
    }

    public static ProductDetailResponse ToProductDetailResponse(Product product)
    {
        return new ProductDetailResponse
        {
            Id = product.Id,
            Name = product.Name,
            Slug = product.Slug,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
            CategoryName = product.Category.Name,
            Images = product.ProductImages.Select(p => new ProductImageResponse
            {
                Id = p.Id,
                Url = p.Url,
                AltText = p.AltText
            }),
            Attributes = product.ProductAttributes
                .Where(a => a.ParentValueId == null)
                .Select(a => new ProductAttributeResponse
                {
                    Id = a.Id,
                    AttributeName = a.AttributeName,
                    Value = a.Value,
                    PriceAdjustment = a.PriceAdjustment,
                    Stock = a.Stock,
                    ChildAttributes = a.ChildValues?.Select(c => new ProductAttributeResponse
                    {
                        Id = c.Id,
                        AttributeName = c.AttributeName,
                        Value = c.Value,
                        PriceAdjustment = c.PriceAdjustment,
                        Stock = c.Stock
                    })
                })
        };
    }

    public static PagingResponse<ProductSummaryResponse> ToProductSummayResponseList(PagingResponse<Product> pagingResponse)
    {
        return new PagingResponse<ProductSummaryResponse>
        {
            Items =  pagingResponse.Items.Select(p => ToProductSummaryResponse(p)),
            TotalCount = pagingResponse.TotalCount,
            PageNumber = pagingResponse.PageNumber,
            PageSize = pagingResponse.PageSize
        };
    }
    
    public static Product ToEntityCreate(ProductCreateRequest request, Guid userId)
    {
        return new Product
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Slug = request.Slug,
            Description = request.Description,
            Price = request.Price,
            Stock = request.Stock,
            CategoryId = request.CategoryId,
            IsFeatured = request.IsFeatured,
            IsActive = true,
            CreatedDate = DateTime.Now,
            CreatedBy = userId
        };
    }
    
    public static void ToEntityUpdate(Product product, ProductUpdateRequest request, Guid userId)
    {
        product.Name = request.Name;
        product.Slug = request.Slug;
        product.Description = request.Description;
        product.Price = request.Price;
        product.Stock = request.Stock;
        product.IsFeatured = request.IsFeatured;
        product.IsActive = request.IsActive;
        product.CategoryId = request.CategoryId;
        product.UpdatedDate = DateTime.Now;
        product.UpdatedBy = userId;
    }
}