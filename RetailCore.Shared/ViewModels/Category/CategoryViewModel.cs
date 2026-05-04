using System.Collections.Generic;

namespace RetailCore.Shared.ViewModels.Category;

public class CategoryViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public int? ParentId { get; set; }
    public List<CategoryViewModel> Children { get; set; } = [];
    public int ProductCount { get; set; }
}