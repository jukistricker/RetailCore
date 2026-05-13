using RetailCore.Domain.Entities;

namespace RetailCore.Domain.Interfaces.Repositories;

public interface ICartItemRepository : IRepository<CartItem>
{
    Task<bool> DeleteAsync(List<Guid> cartItemIds, Guid customerId);
}