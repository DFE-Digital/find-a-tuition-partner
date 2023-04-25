using System.Linq.Expressions;
using Application.Common.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly NtpDbContext _context;
    public GenericRepository(NtpDbContext context)
    {
        _context = context;
    }

    public void AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        _context.Set<T>().AddAsync(entity, cancellationToken);
    }

    public void Update(T entity)
    {
        _context.Set<T>().Update(entity);
    }

    public void AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        _context.Set<T>().AddRangeAsync(entities, cancellationToken);
    }

    public IEnumerable<T> Find(Expression<Func<T, bool>> expression)
    {
        return _context.Set<T>().Where(expression);
    }

    public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> filter,
        string? includeProperties = null, bool tracked = true, CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = tracked ? _context.Set<T>() : _context.Set<T>().AsNoTracking();

        if (!string.IsNullOrWhiteSpace(includeProperties))
        {
            var properties = includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries);
            foreach (var property in properties)
            {
                query = query.Include(property);
            }
        }

        return await query.FirstOrDefaultAsync(filter, cancellationToken);
    }

    public async Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> filter,
        string? includeProperties = null, bool tracked = true, CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = tracked ? _context.Set<T>() : _context.Set<T>().AsNoTracking();

        if (!string.IsNullOrWhiteSpace(includeProperties))
        {
            var properties = includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries);
            foreach (var property in properties)
            {
                query = query.Include(property);
            }
        }

        return await query.SingleOrDefaultAsync(filter, cancellationToken);
    }

    public IEnumerable<T> GetAll()
    {
        return _context.Set<T>().ToList();
    }

    public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null,
        string? includeProperties = null, bool tracked = true, CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = tracked ? _context.Set<T>() : _context.Set<T>().AsNoTracking();

        if (filter != null)
        {
            query = query.Where(filter);
        }

        if (!string.IsNullOrWhiteSpace(includeProperties))
        {
            var properties = includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries);
            foreach (var property in properties)
            {
                query = query.Include(property);
            }
        }

        return await query.ToListAsync(cancellationToken);
    }

    public T GetById(long id)
    {
        return _context.Set<T>().Find(id)!;
    }

    public void Remove(T entity)
    {
        _context.Set<T>().Remove(entity);
    }

    public void RemoveRange(IEnumerable<T> entities)
    {
        _context.Set<T>().RemoveRange(entities);
    }

    public void RollbackChanges()
    {
        var changedEntries = _context.ChangeTracker.Entries()
            .Where(x => x.State != EntityState.Unchanged).ToList();

        foreach (var entry in changedEntries)
        {
            switch (entry.State)
            {
                case EntityState.Modified:
                    entry.CurrentValues.SetValues(entry.OriginalValues);
                    entry.State = EntityState.Unchanged;
                    break;
                case EntityState.Added:
                    entry.State = EntityState.Detached;
                    break;
                case EntityState.Deleted:
                    entry.State = EntityState.Unchanged;
                    entry.Reload();
                    break;
                default: break;
            }
        }
    }
}