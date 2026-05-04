using System;
using System.Collections.Generic;

namespace RetailCore.Shared.ViewModels.Product;

public class ProductDetailViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string? ThumbnailUrl { get; set; }
    public List<string> ImageUrls { get; set; } = [];
    public string? CategoryName { get; set; }
    public double AverageRating { get; set; }
    public int TotalRatings { get; set; }
    public List<RatingViewModel> Reviews { get; set; } = [];
}

public class RatingViewModel
{
    public string CustomerName { get; set; } = string.Empty;
    public byte Rating { get; set; }
    public string? Review { get; set; }
    public DateTime CreatedAt { get; set; }
}