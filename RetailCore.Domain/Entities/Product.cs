using System;

namespace RetailCore.Domain.Entities;

public class Product: BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; } = 0;
    public Guid CategoryId { get; set; }
    public string? ThumbnailUrl { get; set; }
    public bool IsFeatured { get; set; } = false;
    public bool IsActive { get; set; } = true;
}