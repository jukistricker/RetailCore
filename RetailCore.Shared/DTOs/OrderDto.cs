using System;
using System.Collections.Generic;

namespace RetailCore.Shared.DTOs;

public class OrderCreateRequest
{
    public string ShippingAddress { get; set; }
    public string? Note { get; set; }

    public List<OrderItemRequest> Items { get; set; }
}

public class OrderItemRequest
{
    public Guid ProductId { get; set; }
    public Guid? ProductAttributeId { get; set; }
    public int Quantity { get; set; }
}

public class OrderResponse
{
    public Guid Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public string Status { get; set; }
    public decimal TotalAmount { get; set; }
    public string ShippingAddress { get; set; }
    public string? Note { get; set; }
    
    public int TotalItems { get; set; } 
}

public class OrderDetailResponse : OrderResponse
{
    public List<OrderItemResponse> Items { get; set; } = new();
}

public class OrderItemResponse
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; }
    public string? ProductImageUrl { get; set; }
    public Guid? ProductAttributeId { get; set; }
    public string? SelectedAttributes { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal SubTotal => Price * Quantity;
}