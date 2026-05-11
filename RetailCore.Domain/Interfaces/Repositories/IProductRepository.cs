using RetailCore.Domain.Entities;

namespace RetailCore.Domain.Interfaces.Repositories;

public interface IProductRepository: IRepository<Product>
{
    public Task<bool> ExistsByCategoryIdAsync(Guid id);
}