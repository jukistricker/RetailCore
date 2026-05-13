using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RetailCore.Application.Extensions;

namespace RetailCore.Application.UseCases;

public class CartItemService : ICartItemService
{
    private readonly ICartItemRepository _cartItemRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IProductAttributeRepository _productAttributeRepository;
    private readonly IProductRepository _productRepository;

    public CartItemService(
        ICartItemRepository cartItemRepository,
        IHttpContextAccessor httpContextAccessor,
        IProductAttributeRepository productAttributeRepository, 
        IProductRepository productRepository)
    {
        _cartItemRepository = cartItemRepository;
        _httpContextAccessor = httpContextAccessor;
        _productAttributeRepository = productAttributeRepository;
        _productRepository = productRepository;
    }

    public async Task<Result<bool>> AddToCartAsync(CartItemSaveRequest request)
    {
        var customerId = _httpContextAccessor.HttpContext.User.GetCustomerId();

        // Tìm giỏ hàng
        var cartItem = _cartItemRepository.GetQueryable()
            .Include(ci => ci.ProductAttribute)
            .Include(ci => ci.Product)
            .FirstOrDefault(x => x.CustomerId == customerId &&
                                 x.ProductId == request.ProductId &&
                                 x.ProductAttributeId == request.ProductAttributeId);

        if (cartItem == null)
        {
            var product = await _productRepository.GetByIdAsync(request.ProductId);
            var attribute = request.ProductAttributeId.HasValue
                ? await _productAttributeRepository.GetByIdAsync(request.ProductAttributeId.Value)
                : null;

            if (product == null) return Result<bool>.Failure(null, "Product not found");

            var stock = attribute?.Stock ?? product.Stock;
            if (stock < request.Quantity) return Result<bool>.Failure(null, "Out of stock");

            cartItem = new CartItem
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                ProductId = request.ProductId,
                ProductAttributeId = request.ProductAttributeId,
                Quantity = request.Quantity,
                CreatedBy = customerId,
                CreatedDate = DateTime.Now
            };
            await _cartItemRepository.AddAsync(cartItem);
        }
        else
        {
            var availableStock = cartItem.ProductAttribute?.Stock ?? cartItem.Product.Stock;

            if (request.Quantity > availableStock) return Result<bool>.Failure(null, "Total quantity exceeds stock");

            cartItem.Quantity += request.Quantity;
            if (cartItem.Quantity > availableStock)
            {
                return Result<bool>.Failure(null, "Total quantity exceeds stock");
            }
            cartItem.UpdatedBy = customerId;
            cartItem.UpdatedDate = DateTime.Now;
            _cartItemRepository.Update(cartItem);
        }

        await _cartItemRepository.SaveChangesAsync();
        return Result<bool>.Success(true, 204);
    }

    public async Task<Result<PagingResponse<CartItemResponse>>> GetCartItemsAsync(PagingRequest request)
    {
        var customerId = _httpContextAccessor.HttpContext.User.GetCustomerId();

        IQueryable<CartItem> query = _cartItemRepository.GetQueryable()
            .AsNoTracking()
            .Include(ci => ci.Product)
            .Where(ci => ci.CustomerId == customerId)
            .OrderByDescending(ci => ci.CreatedDate);

        var pagedData = await _cartItemRepository.GetByPageAsync(query, request.PageNumber, request.PageSize);


        var cartItemResponses = new List<CartItemResponse>();

        foreach (var item in pagedData.Items)
        {
            var cartItemResponse = await MapToResponseAsync(item);
            cartItemResponses.Add(cartItemResponse);
        }

        var response = new PagingResponse<CartItemResponse>
        {
            Items = cartItemResponses,
            TotalCount = cartItemResponses.Count,
            PageNumber = pagedData.PageNumber,
            PageSize = pagedData.PageSize
        };

        return Result<PagingResponse<CartItemResponse>>.Success(response);
    }

    public async Task<Result<CartItemResponse>> GetByIdAsync(Guid id)
    {
        var customerId = _httpContextAccessor.HttpContext.User.GetCustomerId();

        var cartItem = await _cartItemRepository.GetQueryable()
            .AsNoTracking()
            .Include(ci => ci.Product)
            .FirstOrDefaultAsync(x => x.Id == id && x.CustomerId == customerId);

        if (cartItem == null) return Result<CartItemResponse>.Failure("Id", "Cart item not found");

        var response = await MapToResponseAsync(cartItem);
        return Result<CartItemResponse>.Success(response);
    }

    public async Task<Result<bool>> DeleteCartItemsAsync(List<Guid> cartItemIds)
    {
        var customerId = _httpContextAccessor.HttpContext.User.GetCustomerId();
        await _cartItemRepository.DeleteAsync(cartItemIds, customerId);
        return Result<bool>.Success(true, 204);
    }

    public async Task<Result<bool>> UpdateAsync(CartItemUpdateRequest request)
{
    var customerId = _httpContextAccessor.HttpContext.User.GetCustomerId();

    // 1. Xử lý trường hợp số lượng về 0 -> Xóa món hàng
    if (request.Quantity <= 0)
    {
        await _cartItemRepository.DeleteAsync(new List<Guid> { request.CartItemId }, customerId);
        await _cartItemRepository.SaveChangesAsync();
        return Result<bool>.Success(true, 204);
    }

    var cartItem = await _cartItemRepository.GetQueryable()
        .Include(ci => ci.Product)
        .FirstOrDefaultAsync(x => x.Id == request.CartItemId && x.CustomerId == customerId);

    if (cartItem == null) return Result<bool>.Failure(null, "Cart item not found");

    // 2. Kiểm tra tồn kho (Stock)
    var newAttribute = request.ProductAttributeId.HasValue
        ? await _productAttributeRepository.GetByIdAsync(request.ProductAttributeId.Value)
        : null;

    var stock = newAttribute?.Stock ?? cartItem.Product.Stock;
    if (request.Quantity > stock) return Result<bool>.Failure(null, "Quantity not available in stock");

    // 3. Xử lý logic cập nhật hoặc gộp (Merge)
    // Chỉ thực hiện tìm kiếm món khác để gộp NẾU người dùng thay đổi Attribute khác với cái hiện tại
    if (request.ProductAttributeId != cartItem.ProductAttributeId)
    {
        var existingOtherItem = await _cartItemRepository.GetQueryable()
            .FirstOrDefaultAsync(x => x.CustomerId == customerId &&
                                     x.ProductId == cartItem.ProductId &&
                                     x.ProductAttributeId == request.ProductAttributeId &&
                                     x.Id != request.CartItemId);

        if (existingOtherItem != null)
        {
            // Gộp: Cộng dồn số lượng yêu cầu vào món đã có sẵn trong giỏ
            int totalNewQty = existingOtherItem.Quantity + request.Quantity;

            // Kiểm tra stock cho tổng số lượng sau khi gộp
            if (totalNewQty > stock) return Result<bool>.Failure(null, "Total quantity exceeds stock");

            existingOtherItem.Quantity = totalNewQty;
            existingOtherItem.UpdatedDate = DateTime.Now;
            existingOtherItem.UpdatedBy = customerId;

            _cartItemRepository.Update(existingOtherItem);

            // Xóa món hiện tại (vì đã được gộp vào existingOtherItem)
            await _cartItemRepository.DeleteAsync(new List<Guid> { cartItem.Id }, customerId);
        }
        else
        {
            // Không có món trùng: Cập nhật Attribute mới và số lượng mới cho item hiện tại
            cartItem.ProductAttributeId = request.ProductAttributeId;
            cartItem.Quantity = request.Quantity;
            cartItem.UpdatedBy = customerId;
            cartItem.UpdatedDate = DateTime.Now;

            _cartItemRepository.Update(cartItem);
        }
    }
    else
    {
        // TRƯỜNG HỢP CHỈ CẬP NHẬT SỐ LƯỢNG (Không đổi Attribute)
        // Gán trực tiếp số lượng mới từ request, không chạy vào logic xóa/gộp
        cartItem.Quantity = request.Quantity;
        cartItem.UpdatedBy = customerId;
        cartItem.UpdatedDate = DateTime.Now;

        _cartItemRepository.Update(cartItem);
    }

    await _cartItemRepository.SaveChangesAsync();
    return Result<bool>.Success(true, 204);
}
    
    private async Task<List<CartItemAttributeResponse>> GetFullAttributePathAsync(Guid? attributeId)
    {
        var result = new List<CartItemAttributeResponse>();
        if (!attributeId.HasValue) return result;

        var currentProductAttribute = await _productAttributeRepository.GetByIdAsync(attributeId.Value);
        while (currentProductAttribute != null)
        {
            result.Add(new CartItemAttributeResponse
            {
                Name = currentProductAttribute.AttributeName,
                Value = currentProductAttribute.Value
            });

            currentProductAttribute = currentProductAttribute.ParentValueId.HasValue
                ? await _productAttributeRepository.GetByIdAsync(currentProductAttribute.ParentValueId.Value)
                : null;
        }

        //đảo ngược về thứ tự cha -> con
        result.Reverse();
        return result;
    }

    private async Task<CartItemResponse> MapToResponseAsync(CartItem item)
    {
        var selectedAttributes = await GetFullAttributePathAsync(item.ProductAttributeId);

        var currentPrice = item.Product.Price + selectedAttributes.Sum(x => x.PriceAdjustment);

        return new CartItemResponse
        {
            Id = item.Id,
            ProductId = item.ProductId,
            ProductAttributeId = item.ProductAttributeId,
            Quantity = item.Quantity,
            Price = currentPrice,
            Total = currentPrice * item.Quantity,
            ProductSummary = new ProductSummaryResponse
            {
                Id = item.Product.Id,
                Name = item.Product.Name,
                Slug = item.Product.Slug,
                ThumbnailUrl = item.Product.ThumbnailUrl,
                Price = item.Product.Price
            },
            SelectedAttributes = selectedAttributes
        };
    }
}