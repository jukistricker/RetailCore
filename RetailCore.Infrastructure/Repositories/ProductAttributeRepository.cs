namespace RetailCore.Infrastructure.Repositories;

public class ProductAttributeRepository : Repository<ProductAttribute>, IProductAttributeRepository
{
    public ProductAttributeRepository(AppDbContext context) : base(context)
    {
    }
}