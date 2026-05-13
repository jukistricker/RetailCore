using RetailCore.Domain.Entities;

namespace RetailCore.Domain.Interfaces.Repositories;

public interface ICustomerRepository : IRepository<Customer>
{
    Task<Customer?> GetByUserIdAsync(Guid userId);
    Task<bool> IsEmailUniqueAsync(string email);
}