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