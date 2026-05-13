using RetailCore.Domain.Enums;

namespace RetailCore.Domain.Entities;

public class Order : BaseEntity
{
    public Guid CustomerId { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public string ShippingAddress { get; set; }
    public string? Note { get; set; }

    public ICollection<OrderItem> OrderItems { get; set; }
}