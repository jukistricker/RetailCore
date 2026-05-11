using System;
using System.Collections.Generic;

namespace RetailCore.Shared.DTOs;

public class ProductSummaryResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
    public decimal Price { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string CategoryName { get; set; }
}
    
public record ProductDetailResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string CategoryName { get; set; }
    public List<ProductImageResponse> Images { get; set; }
}

public record ProductImageResponse
{
    public Guid Id { get; set; }
    public string Url { get; set; }
    public string? AltText { get; set; }
}