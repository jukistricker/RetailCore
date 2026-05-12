using RetailCore.Shared.DTOs;

namespace RetailCore.Infrastructure.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public  ProductRepository(AppDbContext dbContext) : base(dbContext){}

    public Task<bool> ExistsByCategoryIdAsync(Guid id)
    {
        return _dbSet.AnyAsync(p => p.CategoryId == id);
    }

    public async Task<IEnumerable<Product>> GetByFeaturedAsync(int count)
    {
        List<Product> featuredProducts = await _dbSet.AsNoTracking()
            .Where(p => p.IsFeatured && p.IsActive)
            .OrderByDescending(p => p.CreatedDate)
            .Take(count)
            .ToListAsync();

        return featuredProducts;
    }
    
    public async Task<Product?> GetDetailAsync(Guid id)
    {
        return await _dbSet.AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.ProductImages)
            .Include(p => p.ProductAttributes.Where(a => a.ParentValueId == null))
            .ThenInclude(a => a.ChildValues)
            .FirstOrDefaultAsync(p => p.Id == id);
    }
}