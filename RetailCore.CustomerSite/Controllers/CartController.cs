using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailCore.CustomerSite.Services.Interfaces;
using RetailCore.Shared.ViewModels;

namespace RetailCore.CustomerSite.Controllers;

[Authorize]
public class CartController : Controller
{
    private readonly ICartItemApiService _cartItemApiService;

    public CartController(ICartItemApiService cartItemApiService)
    {
        _cartItemApiService = cartItemApiService;
    }
    
    [HttpGet]
    public async Task<IActionResult> Index(PagingRequest request)
    {
        request.PageNumber = request.PageNumber > 0 ? request.PageNumber : 1;
        request.PageSize = request.PageSize > 0 ? request.PageSize : 10;

        var cartData = await _cartItemApiService.GetCartItemsAsync(request);

        var viewModel = new CartPageViewModel
        {
            CartData = cartData ?? new PagingResponse<CartItemResponse>
            {
                Items = new List<CartItemResponse>(),
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = 0
            }
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] CartItemSaveRequest request)
    {
        var result = await _cartItemApiService.AddToCartAsync(request);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
    
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] CartItemUpdateRequest request)
    {

        var success = await _cartItemApiService.UpdateCartItemAsync(request);
        
        if (success) return NoContent();
        
        return BadRequest(new { message = "Update cart item failed" });
    }

    [HttpDelete]
    public async Task<IActionResult> Remove(Guid id)
    {
        // var success = await _cartItemApiService.RemoveItemAsync(id);
        // if (success) return NoContent();
        return BadRequest();
    }
}