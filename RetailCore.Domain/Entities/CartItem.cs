namespace RetailCore.Domain.Entities;

public class CartItem : BaseEntity
{
    public Guid CustomerId { get; set; }
    public Guid ProductId { get; set; }
    public Guid? ProductAttributeId { get; set; }
    public int Quantity { get; set; } = 1;
    public Product Product { get; set; }
    public ProductAttribute ProductAttribute { get; set; }
}