using System.Linq.Expressions;

namespace Core.DataAccess;

public interface IRepository<T> where T : class
{
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    void Update(T entity);
    void Remove(T entity);
    Task<T> GetByIdAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default);
    IQueryable<T> GetAll();
    IQueryable<T> GetWhere(Expression<Func<T, bool>> expression);
    Task<bool> AnyAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default);
    Task<T?> GetByExpressionWithTrackingAsync(
        Expression<Func<T, bool>> expression,
        CancellationToken cancellationToken = default (CancellationToken));
    Task<T?> GetByExpressionAsync(
        Expression<Func<T, bool>> expression,
        CancellationToken cancellationToken = default (CancellationToken));
}