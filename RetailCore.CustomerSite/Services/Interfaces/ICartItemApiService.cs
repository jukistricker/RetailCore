namespace RetailCore.CustomerSite.Services.Interfaces;

public interface ICartItemApiService
{
    Task<Result<bool>> AddToCartAsync(CartItemSaveRequest request);
    Task<bool> UpdateCartItemAsync(CartItemUpdateRequest request);
    Task<PagingResponse<CartItemResponse>?> GetCartItemsAsync(PagingRequest request);
}