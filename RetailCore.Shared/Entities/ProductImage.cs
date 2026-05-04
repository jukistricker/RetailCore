using System;

namespace RetailCore.Shared.Entities;

public class ProductImage: BaseEntity
{
    public Guid ProductId { get; set; }
    public string Url { get; set; } = string.Empty;
    public string? AltText { get; set; }
    public int SortOrder { get; set; } = 0;
}