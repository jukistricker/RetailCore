namespace RetailCore.Application.UseCases.Interfaces;

public interface ICartItemService
{
    Task<Result<bool>> AddToCartAsync(CartItemSaveRequest request);
    Task<Result<PagingResponse<CartItemResponse>>> GetCartItemsAsync(PagingRequest request);
    Task<Result<bool>> DeleteCartItemsAsync(List<Guid> cartItemIds);
    Task<Result<CartItemResponse>> GetByIdAsync(Guid id);
    Task<Result<bool>> UpdateAsync(CartItemUpdateRequest request);
}