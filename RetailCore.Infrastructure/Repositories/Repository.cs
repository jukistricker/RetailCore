namespace RetailCore.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<IEnumerable<T>> GetAllAsync(int page = 1, int size = 10, bool desc = true)
    {
        var query = _dbSet.AsNoTracking(); 

        query = desc 
            ? query.OrderByDescending(x => EF.Property<Guid>(x, "Id"))
            : query.OrderBy(x => EF.Property<Guid>(x, "Id"));

        return await query
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();
    }

    public async Task<IEnumerable<T>> GetAllAsync() 
    {
        return await _dbSet.AsNoTracking().ToListAsync();
    }

    public async Task<T?> GetByIdAsync(object id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}