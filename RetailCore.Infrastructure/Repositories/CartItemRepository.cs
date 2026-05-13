namespace RetailCore.Infrastructure.Repositories;

public class CartItemRepository : Repository<CartItem>, ICartItemRepository
{
    public CartItemRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<bool> DeleteAsync(List<Guid> cartItemIds, Guid customerId)
    {
        //lọc cart item theo id và customer id 
        return await _dbSet
            .Where(x => cartItemIds.Contains(x.Id) && x.CustomerId == customerId)
            .ExecuteDeleteAsync() > 0;
    }
}