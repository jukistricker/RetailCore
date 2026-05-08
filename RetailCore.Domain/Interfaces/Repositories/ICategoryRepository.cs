using RetailCore.Domain.Entities;

namespace RetailCore.Domain.Interfaces.Repositories;

public interface ICategoryRepository : IRepository<Category>
{
    Task<Category?> GetBySlugAsync(string slug);
}