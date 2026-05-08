using System;

namespace RetailCore.Domain.Entities;

public class CartItem: BaseEntity
{
    public Guid CustomerId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; } = 1;
}