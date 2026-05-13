namespace RetailCore.Domain.Entities;

public class OrderItem : BaseEntity
{
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public Guid? ProductAttributeId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public string? SelectedAttributes { get; set; }
    
    public Order Order { get; set; }
    public Product Product { get; set; }
}