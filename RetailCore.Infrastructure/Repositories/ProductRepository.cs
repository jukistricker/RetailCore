namespace RetailCore.Infrastructure.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public  ProductRepository(AppDbContext dbContext) : base(dbContext){}

    public Task<bool> ExistsByCategoryIdAsync(Guid id)
    {
        return _dbSet.AnyAsync(p => p.CategoryId == id);
    }
}