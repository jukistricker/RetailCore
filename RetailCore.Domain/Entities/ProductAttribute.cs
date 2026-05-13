namespace RetailCore.Domain.Entities;

public class ProductAttribute : BaseEntity
{
    public string Value { get; set; } = string.Empty;
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public string AttributeName { get; set; } = string.Empty;

    public Guid? ParentValueId { get; set; }
    public ProductAttribute? ParentValue { get; set; }
    public ICollection<ProductAttribute> ChildValues { get; set; } = new List<ProductAttribute>();

    public decimal PriceAdjustment { get; set; } = 0;
    public int? Stock { get; set; }
}