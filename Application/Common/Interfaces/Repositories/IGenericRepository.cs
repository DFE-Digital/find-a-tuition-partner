using System.Linq.Expressions;

namespace Application.Common.Interfaces.Repositories;

public interface IGenericRepository<T> where T : class
{
    T GetById(long id);
    IEnumerable<T> GetAll();

    //includeProperties - i.e: "MagicLink,TuitionPartner"
    Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null,
        string? includeProperties = null, bool tracked = true, CancellationToken cancellationToken = default);
    IEnumerable<T> Find(Expression<Func<T, bool>> expression);
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> filter,
        string? includeProperties = null, bool tracked = true, CancellationToken cancellationToken = default);
    Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> filter,
        string? includeProperties = null, bool tracked = true, CancellationToken cancellationToken = default);
    void AddAsync(T entity, CancellationToken cancellationToken = default);
    void AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    void Update(T entity);
    void Remove(T entity);
    void RemoveRange(IEnumerable<T> entities);
}