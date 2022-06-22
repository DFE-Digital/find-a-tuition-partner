using Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Tests;

public class SliceFixture : IAsyncLifetime
{
    private readonly IConfiguration _configuration;

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly WebApplicationFactory<Program> _factory;

    public SliceFixture()
    {
        _factory = new ContosoTestApplicationFactory();

        _configuration = _factory.Services.GetRequiredService<IConfiguration>();
        _scopeFactory = _factory.Services.GetRequiredService<IServiceScopeFactory>();
    }

    private class ContosoTestApplicationFactory
        : WebApplicationFactory<Program>
    {
        protected override TestServer CreateServer(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddDbContext<NtpDbContext>(options =>
                {
                    options.UseSqlite(Guid.NewGuid().ToString());
                });
            });

            return base.CreateServer(builder);
        }
    }

    public async Task ExecuteScopeAsync(Func<IServiceProvider, Task> action)
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<NtpDbContext>();

        try
        {
            await dbContext.Database.BeginTransactionAsync();

            await action(scope.ServiceProvider);

            await dbContext.Database.CommitTransactionAsync();
        }
        catch (Exception)
        {
            dbContext.Database.RollbackTransaction();
            throw;
        }
    }

    public async Task<T> ExecuteScopeAsync<T>(Func<IServiceProvider, Task<T>> action)
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<NtpDbContext>();

        try
        {
            await dbContext.Database.BeginTransactionAsync();

            var result = await action(scope.ServiceProvider);

            await dbContext.Database.CommitTransactionAsync();

            return result;
        }
        catch (Exception)
        {
            dbContext.Database.RollbackTransaction();
            throw;
        }
    }

    public Task ExecuteDbContextAsync(Func<NtpDbContext, Task> action)
        => ExecuteScopeAsync(sp => action(sp.GetService<NtpDbContext>()));

    public Task ExecuteDbContextAsync(Func<NtpDbContext, ValueTask> action)
        => ExecuteScopeAsync(sp => action(sp.GetService<NtpDbContext>()).AsTask());

    public Task ExecuteDbContextAsync(Func<NtpDbContext, IMediator, Task> action)
        => ExecuteScopeAsync(sp => action(sp.GetService<NtpDbContext>(), sp.GetService<IMediator>()));

    public Task<T> ExecuteDbContextAsync<T>(Func<NtpDbContext, Task<T>> action)
        => ExecuteScopeAsync(sp => action(sp.GetService<NtpDbContext>()));

    public Task<T> ExecuteDbContextAsync<T>(Func<NtpDbContext, ValueTask<T>> action)
        => ExecuteScopeAsync(sp => action(sp.GetService<NtpDbContext>()).AsTask());

    public Task<T> ExecuteDbContextAsync<T>(Func<NtpDbContext, IMediator, Task<T>> action)
        => ExecuteScopeAsync(sp => action(sp.GetService<NtpDbContext>(), sp.GetService<IMediator>()));

    public Task InsertAsync<T>(params T[] entities) where T : class
    {
        return ExecuteDbContextAsync(db =>
        {
            foreach (var entity in entities)
            {
                db.Set<T>().Add(entity);
            }
            return db.SaveChangesAsync();
        });
    }

    public Task InsertAsync<TEntity>(TEntity entity) where TEntity : class
    {
        return ExecuteDbContextAsync(db =>
        {
            db.Set<TEntity>().Add(entity);

            return db.SaveChangesAsync();
        });
    }

    public Task InsertAsync<TEntity, TEntity2>(TEntity entity, TEntity2 entity2)
        where TEntity : class
        where TEntity2 : class
    {
        return ExecuteDbContextAsync(db =>
        {
            db.Set<TEntity>().Add(entity);
            db.Set<TEntity2>().Add(entity2);

            return db.SaveChangesAsync();
        });
    }

    public Task InsertAsync<TEntity, TEntity2, TEntity3>(TEntity entity, TEntity2 entity2, TEntity3 entity3)
        where TEntity : class
        where TEntity2 : class
        where TEntity3 : class
    {
        return ExecuteDbContextAsync(db =>
        {
            db.Set<TEntity>().Add(entity);
            db.Set<TEntity2>().Add(entity2);
            db.Set<TEntity3>().Add(entity3);

            return db.SaveChangesAsync();
        });
    }

    public Task InsertAsync<TEntity, TEntity2, TEntity3, TEntity4>(TEntity entity, TEntity2 entity2, TEntity3 entity3, TEntity4 entity4)
        where TEntity : class
        where TEntity2 : class
        where TEntity3 : class
        where TEntity4 : class
    {
        return ExecuteDbContextAsync(db =>
        {
            db.Set<TEntity>().Add(entity);
            db.Set<TEntity2>().Add(entity2);
            db.Set<TEntity3>().Add(entity3);
            db.Set<TEntity4>().Add(entity4);

            return db.SaveChangesAsync();
        });
    }

    public Task<T?> FindAsync<T>(int id)
        where T : class
    {
        return ExecuteDbContextAsync(db => db.Set<T>().FindAsync(id).AsTask());
    }

    internal Task<List<T>> GetDbAsync<T>()
        where T : class
        => ExecuteDbContextAsync(db => db.Set<T>().ToListAsync());

    public Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
    {
        return ExecuteScopeAsync(sp =>
        {
            var mediator = sp.GetRequiredService<IMediator>();

            return mediator.Send(request);
        });
    }

    public Task SendAsync(IRequest request)
    {
        return ExecuteScopeAsync(sp =>
        {
            var mediator = sp.GetRequiredService<IMediator>();

            return mediator.Send(request);
        });
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync()
    {
        _factory?.Dispose();
        return Task.CompletedTask;
    }
}