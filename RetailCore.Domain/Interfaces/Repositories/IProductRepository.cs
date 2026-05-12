using RetailCore.Domain.Entities;
using RetailCore.Shared.DTOs;

namespace RetailCore.Domain.Interfaces.Repositories;

public interface IProductRepository: IRepository<Product>
{
    Task<bool> ExistsByCategoryIdAsync(Guid id);
    Task<IEnumerable<Product>> GetByFeaturedAsync(int count);
    Task<Product?> GetDetailAsync(Guid id);
}