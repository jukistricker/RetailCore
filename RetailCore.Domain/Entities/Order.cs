using System;
using RetailCore.Domain.Enums;

namespace RetailCore.Domain.Entities;

public class Order: BaseEntity
{
    public Guid CustomerId { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public decimal TotalAmount { get; set; }
    public string ShippingAddress { get; set; } = string.Empty;
    public string ShippingCity { get; set; } = string.Empty;
    public string? Note { get; set; }
}
