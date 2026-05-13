namespace RetailCore.API.Controllers;

[Route("api/cart-items")]
[Authorize]
public class CartController : ApiControllerBase
{
    private readonly ICartItemService _cartItemService;

    public CartController(ICartItemService cartItemService)
    {
        _cartItemService = cartItemService;
    }

    [HttpPost]
    public async Task<IActionResult> AddToCartAsync([FromBody] CartItemSaveRequest request)
    {
        var result = await _cartItemService.AddToCartAsync(request);
        return HandleResult(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetCartItemsAsync([FromQuery] PagingRequest request)
    {
        var result = await _cartItemService.GetCartItemsAsync(request);
        return HandleResult(result);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteCartItemsAsync([FromBody] List<Guid> cartItemIds)
    {
        var result = await _cartItemService.DeleteCartItemsAsync(cartItemIds);
        return HandleResult(result);
    }
    
    [HttpPut]
    public async Task<IActionResult> UpdateCartItemAsync([FromBody] CartItemUpdateRequest request)
    {
        var result = await _cartItemService.UpdateAsync(request);
        return HandleResult(result);
    }
}