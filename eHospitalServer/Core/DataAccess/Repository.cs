using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Core.DataAccess;

public class Repository<T , TContext> : IRepository<T>
    where T : class
    where TContext : DbContext
{
    private readonly TContext _context;
    private readonly DbSet<T> _dbSet;
    public Repository(TContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }
    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
    }

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(expression, cancellationToken);
    }

    public IQueryable<T> GetAll()
    {
        return _dbSet.AsNoTracking().AsQueryable();
    }

    public async Task<T?> GetByIdAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(expression).FirstOrDefaultAsync(cancellationToken);
    }

    public IQueryable<T> GetWhere(Expression<Func<T, bool>> expression)
    {
        return _dbSet.AsNoTracking().Where(expression).AsQueryable();
    }

    public void Remove(T entity)
    {
        _context.Remove(entity);
    }

    public void Update(T entity)
    {
        _context.Update(entity);
    }
}