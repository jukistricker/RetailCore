using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace RetailCore.Shared.DTOs;

public class ProductSummaryResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
    public decimal Price { get; set; }
    public string? ThumbnailUrl { get; set; }
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
    public IEnumerable<ProductImageResponse> Images { get; set; }
    public IEnumerable<ProductAttributeResponse> Attributes { get; set; } = new List<ProductAttributeResponse>();
}

public class ProductAttributeResponse
{
    public Guid Id { get; set; }
    public string AttributeName { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;        
    public decimal? PriceAdjustment { get; set; }
    public int? Stock { get; set; }
    
    public IEnumerable<ProductAttributeResponse>? ChildAttributes { get; set; }
}

public class ProductImageResponse
{
    public Guid Id { get; set; }
    public string Url { get; set; }
    public string? AltText { get; set; }
}

public class ProductCreateRequest
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public string Slug { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public Guid CategoryId { get; set; }
    public string? ThumbnailUrl { get; set; }
    public bool IsFeatured { get; set; }
    public List<IFormFile> Images { get; set; }
    public List<ProductAttributeRequest>? Attributes { get; set; }
}

public class ProductUpdateRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string Slug { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public Guid CategoryId { get; set; }
    public string? ThumbnailUrl { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsActive { get; set; }
    public List<IFormFile> Images { get; set; }
    public List<ProductAttributeRequest>? Attributes { get; set; }
}

public class ProductAttributeRequest
{
    public string AttributeName { get; set; } = string.Empty; 
    public string Value { get; set; } = string.Empty;         
    public decimal? PriceAdjustment { get; set; }
    public int? Stock { get; set; }
    
    public List<ProductAttributeRequest>? ChildAttributes { get; set; } 
}