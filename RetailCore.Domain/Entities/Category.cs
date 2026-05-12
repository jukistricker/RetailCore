using System;

namespace RetailCore.Domain.Entities;

public class Category: BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    // public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; } = 0;
    public  ICollection<Product> Products { get; set; } = new List<Product>();
}