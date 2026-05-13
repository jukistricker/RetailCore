namespace RetailCore.API.Controllers;

public class OrderController: ApiControllerBase
{
    private readonly IOrderService _orderService;
    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }
    
    
}