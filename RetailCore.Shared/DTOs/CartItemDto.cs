using System;
using System.Collections.Generic;

namespace RetailCore.Shared.DTOs;

public class CartItemSaveRequest
{
    public Guid? Id { get; set; }
    public Guid ProductId { get; set; }
    public Guid? ProductAttributeId { get; set; }
    public int Quantity { get; set; } = 1;
}

public class CartItemUpdateRequest
{
    public Guid CartItemId { get; set; }
    public Guid? ProductAttributeId { get; set; }
    public int Quantity { get; set; }
}

public class CartItemResponse
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public Guid? ProductAttributeId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal Total { get; set; }
    public ProductSummaryResponse ProductSummary { get; set; }
    public List<CartItemAttributeResponse> SelectedAttributes { get; set; }
}

public class CartItemAttributeResponse
{
    public string Name { get; set; }
    public string Value { get; set; }
    public decimal PriceAdjustment { get; set; }
}