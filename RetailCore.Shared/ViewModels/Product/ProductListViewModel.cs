using System.Collections.Generic;
using RetailCore.Shared.ViewModels.Category;

namespace RetailCore.Shared.ViewModels.Product;

public class ProductListViewModel
{
    public List<ProductItemViewModel> Products { get; set; } = [];
    public List<CategoryViewModel> Categories { get; set; } = [];
    public int? SelectedCategoryId { get; set; }
    public int CurrentPage { get; set; } = 1;
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
}

public class ProductItemViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? CategoryName { get; set; }
    public double AverageRating { get; set; }
}