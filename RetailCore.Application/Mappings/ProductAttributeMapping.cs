namespace RetailCore.Application.Mappings;

public static class ProductAttributeMapping
{
    public static ProductAttribute ToEntityCreate(Product product, ProductAttributeRequest request, Guid? parentId)
    {
        return new ProductAttribute
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            AttributeName = request.AttributeName,
            Value = request.Value,
            PriceAdjustment = request.PriceAdjustment,
            Stock = request.Stock,
            ParentValueId = parentId
        };
    }
}