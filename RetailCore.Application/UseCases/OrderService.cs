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

        var cartItems = await _cartItemRepository.GetQueryable()
            .Include(ci => ci.Product)
            .Include(ci => ci.ProductAttribute)
            .Where(ci => ci.CustomerId == customerId)
            .ToListAsync();

        if (!cartItems.Any()) return Result<Guid>.Failure(null, "Cart is empty");

        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            ShippingAddress = request.ShippingAddress,
            Status = OrderStatus.Pending,
            OrderItems = new List<OrderItem>(),
            CreatedDate = DateTime.UtcNow,
            CreatedBy = customerId
        };

        decimal total = 0;

        foreach (var cart in cartItems)
        {
            if (cart.ProductAttributeId.HasValue)
            {
                var attr = cart.ProductAttribute;
                if (attr == null || attr.Stock < cart.Quantity)
                    return Result<Guid>.Failure("AttributeId",
                        $"Attribute of {cart.Product?.Name} doesn't have enough stock");

                attr.Stock -= cart.Quantity;
            }
            else
            {
                var product = cart.Product;
                if (product == null || product.Stock < cart.Quantity)
                    return Result<Guid>.Failure("ProductId", $"Product {product?.Name} doesn't have enough stock");

                product.Stock -= cart.Quantity;
            }

            var priceAtPurchase = cart.Product.Price;
            if (cart.ProductAttribute != null) priceAtPurchase += cart.ProductAttribute.PriceAdjustment;

            order.OrderItems.Add(new OrderItem
            {
                Id = Guid.NewGuid(),
                ProductId = cart.ProductId,
                ProductAttributeId = cart.ProductAttributeId,
                Quantity = cart.Quantity,
                Price = priceAtPurchase,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = customerId
            });

            total += priceAtPurchase * cart.Quantity;
        }

        order.TotalAmount = total;
        var cartItemIds = cartItems.Select(ci => ci.Id).ToList();

        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            await _orderRepository.AddAsync(order);

            await _cartItemRepository.DeleteAsync(cartItemIds, customerId);

            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return Result<Guid>.Success(order.Id);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}