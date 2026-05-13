using RetailCore.Domain.Enums;

namespace RetailCore.Application.UseCases.Interfaces;

//
public interface IOrderService
{
    Task<Result<Guid>> UpdateOrderStatusAsync(Guid orderId, OrderStatus status);
    Task<Result<Guid>> CreateOrderAsync(OrderCreateRequest request);
}