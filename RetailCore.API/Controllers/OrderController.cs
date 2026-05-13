using RetailCore.Domain.Enums;

namespace RetailCore.API.Controllers;

[Route("api/orders")]
public class OrderController: ApiControllerBase
{
    private readonly IOrderService _orderService;
    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateOrderAsync(OrderCreateRequest request)
    {
        var result = await _orderService.CreateOrderAsync(request);
        return HandleResult(result);
    }
    
    [HttpPut("{orderId}/status/{status}")]
    public async Task<IActionResult> UpdateOrderStatusAsync(Guid orderId, OrderStatus status)
    {
        var result = await _orderService.UpdateOrderStatusAsync(orderId, status);
        return HandleResult(result);
    }
    
    
}