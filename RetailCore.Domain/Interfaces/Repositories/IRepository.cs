using RetailCore.Shared.DTOs;

namespace RetailCore.Domain.Interfaces.Repositories;

public interface IRepository<T> where T : class
{
    Task<PagingResponse<T>> GetByPageAsync(
        IQueryable<T> query,
        int pageNumber,
        int pageSize);

    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(object id);
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
    Task<bool> SaveChangesAsync();
    IQueryable<T> GetQueryable();
}