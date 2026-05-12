using Microsoft.AspNetCore.Http;
using RetailCore.Application.Extensions;

namespace RetailCore.Application.UseCases;

public class CartItemService: ICartItemService
{
    private readonly ICartItemRepository _cartItemRepository;
    private readonly AppDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IProductRepository _productRepository;
    
    public CartItemService(
        ICartItemRepository cartItemRepository,
        AppDbContext dbContext,
        IHttpContextAccessor httpContextAccessor, 
        IProductRepository productRepository)
    {
        _cartItemRepository = cartItemRepository;
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
        _productRepository = productRepository;
    }
    
    public async Task<Result<bool>> AddToCartAsync(CartItemSaveRequest request)
    {
        Guid customerId = _httpContextAccessor.HttpContext.User.GetCustomerId(); 
        var cartItem =  _cartItemRepository.
            GetQueryable()
            .FirstOrDefault(x => x.CustomerId == customerId && 
                                               x.ProductId == request.ProductId && 
                                               x.ProductAttributeId == request.ProductAttributeId);

        if (cartItem != null)
        {
            cartItem.Quantity += request.Quantity;
            _cartItemRepository.Update(cartItem);
        }
        else
        {
            cartItem = new CartItem
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                ProductId = request.ProductId,
                ProductAttributeId = request.ProductAttributeId,
                Quantity = request.Quantity
            };
            await _cartItemRepository.AddAsync(cartItem);
        }

        await _cartItemRepository.SaveChangesAsync();

        return Result<bool>.Success(true,204);
    }

    public async Task<Result<PagingResponse<CartItemResponse>>> GetCartItemsAsync(PagingRequest request)
    {
        
    }
    // var product = await _productRepository.GetByIdAsync(request.ProductId);
    //
    //     return new CartItemResponse
    // {
    //     Id = cartItem.Id,
    //     ProductId = cartItem.ProductId,
    //     ProductAttributeId = cartItem.ProductAttributeId,
    //     Quantity = cartItem.Quantity,
    //     Price = product.Price, // Logic tính giá cuối nên tính ở đây (Gốc + Adjustment)
    //     Total = cartItem.Quantity * product.Price,
    //     ProductSummary = new ProductSummaryResponse 
    //     { 
    //         Name = product.Name, 
    //         Slug = product.Slug,
    //         ThumbnailUrl = product.ThumbnailUrl // Đường dẫn tương đối như đã bàn
    //     },
    //     SelectedAttributes = await GetFullAttributePath(cartItem.ProductAttributeId)
    // };
}