using System;

namespace RetailCore.Domain.Entities;

public class ProductRating: BaseEntity
{
    public Guid ProductId { get; set; }
    public Guid CustomerId { get; set; }
    public byte Rating { get; set; }
    public string? Review { get; set; }
}