using RetailCore.CustomerSite.Services.Interfaces;
using RetailCore.Shared.Constants;

namespace RetailCore.CustomerSite.Services;

public class CartItemApiService : BaseApiService, ICartItemApiService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CartItemApiService(IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor)
        : base(httpClientFactory, configuration)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result<bool>> AddToCartAsync(CartItemSaveRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync(ApiRoutes.CartItems.CartItemBase, request);

        if (response.IsSuccessStatusCode)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.NoContent || 
                response.Content.Headers.ContentLength == 0)
            {
                return Result<bool>.Success(true, (int)response.StatusCode);
            }
        
            return await response.Content.ReadFromJsonAsync<Result<bool>>();
        }

        if (response.Content.Headers.ContentLength > 0)
        {
            
                return await response.Content.ReadFromJsonAsync<Result<bool>>();
            
        }
        return Result<bool>.Failure(null, $"API Error: {response.StatusCode}", (int)response.StatusCode);
    }
    
    public async Task<PagingResponse<CartItemResponse>?> GetCartItemsAsync(PagingRequest request)
    {
        var queryString = $"?pageNumber={request.PageNumber}&pageSize={request.PageSize}";
        var response = await _httpClient.GetFromJsonAsync<Result<PagingResponse<CartItemResponse>>>($"{ApiRoutes.CartItems.CartItemBase}{queryString}");
        return response?.IsSuccess == true ? response.Value : null;
    }

    public async Task<bool> UpdateCartItemAsync(CartItemUpdateRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync(ApiRoutes.CartItems.CartItemBase, request);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> RemoveItemAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/cart?ids={id}");
        return response.IsSuccessStatusCode;
    }
}