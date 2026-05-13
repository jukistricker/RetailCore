using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RetailCore.Application.Extensions;
using RetailCore.Domain.Enums;

namespace RetailCore.Application.UseCases;

public class OrderService : IOrderService
{
    private readonly AppDbContext _dbContext;
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderItemRepository _orderItemRepository;
    private readonly ICartItemRepository _cartItemRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public OrderService(AppDbContext dbContext,
        IOrderRepository orderRepository,
        IOrderItemRepository orderItemRepository,
        ICartItemRepository cartItemRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _orderRepository = orderRepository;
        _orderItemRepository = orderItemRepository;
        _cartItemRepository = cartItemRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result<Guid>> CreateOrderAsync(OrderCreateRequest request)
{
    var customerId = _httpContextAccessor.HttpContext.User.GetCustomerId();

    var requestedProductIds = request.Items.Select(i => i.ProductId).ToList();
    
    var cartItems = await _cartItemRepository.GetQueryable()
        .Include(ci => ci.Product)
        .Include(ci => ci.ProductAttribute)
        .ThenInclude(pa => pa.ParentValue)
        .Where(ci => ci.CustomerId == customerId && requestedProductIds.Contains(ci.ProductId))
        .ToListAsync();

    if (!cartItems.Any()) 
        return Result<Guid>.Failure(null, "No items found in cart");

    using var transaction = await _dbContext.Database.BeginTransactionAsync();
    try
    {
        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            ShippingAddress = request.ShippingAddress,
            Note = request.Note,
            Status = OrderStatus.Pending,
            CreatedDate = DateTime.UtcNow,
            CreatedBy = customerId,
            TotalAmount = 0,
            OrderItems = new List<OrderItem>()
        };

        decimal total = 0;

        foreach (var cart in cartItems)
        {
            if (cart.ProductAttributeId.HasValue)
            {
                var attr = cart.ProductAttribute;
                if (attr == null || attr.Stock < cart.Quantity)
                    throw new Exception($"Attribute of {cart.Product?.Name} is out of stock.");

                attr.Stock -= cart.Quantity;
                _dbContext.Update(attr);
            }
            else
            {
                var product = cart.Product;
                if (product == null || product.Stock < cart.Quantity)
                    throw new Exception($"Product {product?.Name} is out of stock.");

                product.Stock -= cart.Quantity; 
                _dbContext.Entry(product).State = EntityState.Modified;
            }

            var attributesSnapshot = GetSelectedAttributesString(cart.ProductAttribute);
            var priceAtPurchase = cart.Product.Price;
            if (cart.ProductAttribute != null) 
                priceAtPurchase += cart.ProductAttribute.PriceAdjustment;

            var orderItem = new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                ProductId = cart.ProductId,
                ProductAttributeId = cart.ProductAttributeId,
                Quantity = cart.Quantity,
                Price = priceAtPurchase,
                SelectedAttributes = attributesSnapshot,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = customerId
            };

            order.OrderItems.Add(orderItem);
            total += priceAtPurchase * cart.Quantity;
        }

        order.TotalAmount = total;

        await _orderRepository.AddAsync(order);

        var cartItemIds = cartItems.Select(ci => ci.Id).ToList();
        await _cartItemRepository.DeleteAsync(cartItemIds, customerId);

        await _dbContext.SaveChangesAsync();

        await transaction.CommitAsync();

        return Result<Guid>.Success(order.Id);
    }
    catch (Exception ex)
    {
        await transaction.RollbackAsync();
        return Result<Guid>.Failure(null, ex.Message);
    }
}

    public async Task<Result<Guid>> CancelOrderAsync(Guid orderId)
    {
        Order? order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null) 
            return Result<Guid>.Failure(null, "Order not found");
        
        if (order.Status != OrderStatus.Pending) 
            return Result<Guid>.Failure(null, "Order is not in pending status");

        if (order.CreatedBy != _httpContextAccessor.HttpContext.User.GetCustomerId())
        {
            return  Result<Guid>.Failure(null, "Order is not in customer");
        }
        
        order.Status = OrderStatus.Cancelled;
        _orderRepository.Update(order);
        await _dbContext.SaveChangesAsync();
        return Result<Guid>.Success(orderId);
    }

    public async Task<Result<Guid>> UpdateOrderStatusAsync(Guid orderId, OrderStatus status)
    {
        Order? order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null) 
            return Result<Guid>.Failure(null, "Order not found");

        order.Status = status;
        _orderRepository.Update(order);
        await _dbContext.SaveChangesAsync();
        return Result<Guid>.Success(orderId);
    }
    
    private string GetSelectedAttributesString(ProductAttribute? attr)
    {
        if (attr == null) return string.Empty;

        var parts = new List<string>();
    
        parts.Add($"{attr.AttributeName}: {attr.Value}");

        if (attr.ParentValueId.HasValue && attr.ParentValue != null)
        {
            var parent = attr.ParentValue;
            parts.Add($"{parent.AttributeName}: {parent.Value}");
        }

        parts.Reverse();

        return string.Join(", ", parts);
    }
    
    // public async Task<Result<PagingResponse<OrderResponse>>> GetOrdersAsync(
    //     PagingRequest request,
    //     OrderStatus? status = null)
    // {
    //     var customerId = _httpContextAccessor.HttpContext.User.GetCustomerId();
    //
    //     var query = _orderRepository.GetQueryable()
    //         .AsNoTracking()
    //         .Where(o => o.CustomerId == customerId);
    //
    //     if (status.HasValue)
    //         query = query.Where(o => o.Status == status.Value);
    //
    //     if (!string.IsNullOrWhiteSpace(request.Search))
    //         query = query.Where(o => o.ShippingAddress.Contains(request.Search));
    //
    //     var response = await _orderRepository.GetByPageAsync(query.OrderByDescending(o => o.CreatedDate), 
    //         request.PageNumber, request.PageSize);
    //
    //     return Result<PagingResponse<OrderResponse>>.Success(new PagingResponse<OrderResponse>
    //     {
    //         Items = OrderMapping.ToOrderResponseList(response.Items),
    //         TotalCount = response.TotalCount,
    //         PageNumber = response.PageNumber,
    //         PageSize = response.PageSize
    //     });
    // }
    //
    // public async Task<Result<OrderDetailResponse>> GetOrderByIdAsync(Guid orderId)
    // {
    //     var customerId = _httpContextAccessor.HttpContext.User.GetCustomerId();
    //
    //     var order = await _orderRepository.GetQueryable()
    //         .AsNoTracking()
    //         .Include(o => o.OrderItems)
    //         .ThenInclude(oi => oi.Product)
    //         .Include(o => o.OrderItems)
    //         .FirstOrDefaultAsync(o => o.Id == orderId && o.CustomerId == customerId);
    //
    //     if (order == null)
    //         return Result<OrderDetailResponse>.Failure(null, "Order not found");
    //
    //     return Result<OrderDetailResponse>.Success(OrderMapping.ToOrderDetailResponse(order));
    // }
    
    
}