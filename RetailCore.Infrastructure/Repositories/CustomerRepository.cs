namespace RetailCore.Infrastructure.Repositories;

public class CustomerRepository : Repository<Customer>, ICustomerRepository
{
    public CustomerRepository(AppDbContext dbContext) : base(dbContext) { }

    public async Task<Customer?> GetByUserIdAsync(Guid userId)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.UserId == userId);
    }

    public async Task<bool> IsEmailUniqueAsync(string email)
    {
        return !await _dbSet.AnyAsync(c => c.Email == email);
    }
}