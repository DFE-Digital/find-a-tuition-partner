using Application;
using Domain.Search;
using Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace Tests.Utilities;

[CollectionDefinition(nameof(SliceFixture))]
public class SliceFixtureCollection : ICollectionFixture<SliceFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}

public class SliceFixture : IAsyncLifetime
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly NtpApplicationFactory _factory;

    public SliceFixture()
    {
        _factory = new NtpApplicationFactory();
        _scopeFactory = _factory.Services.GetRequiredService<IServiceScopeFactory>();
    }

    public ILocationFilterService LocationFilter => _factory.LocationFilter;

    private class NtpApplicationFactory
        : WebApplicationFactory<Program>
    {
        public NtpApplicationFactory()
        {
            LocationFilter = Substitute.For<ILocationFilterService>();
            LocationFilter
                .GetLocationFilterParametersAsync(Arg.Any<string>())
                .Returns(new LocationFilterParameters
                {
                    Country = "England",
                    LocalAuthorityDistrictCode = "N1",
                });
        }

        public ILocationFilterService LocationFilter { get; }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services.Remove<DbContextOptions<NtpDbContext>>();

                var db = Guid.NewGuid().ToString();

                services.AddDbContext<NtpDbContext>(options =>
                {
                    options.UseSqlite($"Data Source={db}");
                });

                services.Remove<ILocationFilterService>();
                services.AddSingleton(LocationFilter);

                services.AddMvc().AddControllersAsServices();
            });
        }
    }

    public ScopedPageExecutor<TPage> GetPage<TPage>() where TPage : class
    {
        return new(_scopeFactory);
    }

    public Task<TResult> GetPage<TPage, TResult>(Func<TPage, Task<TResult>> action) where TPage : class
    {
        return ExecuteScopeAsync(sp =>
        {
            var ctorparms = typeof(TPage).GetConstructors().First().GetParameters().Select(p => sp.GetRequiredService(p.ParameterType)).ToArray();

            var page = sp.CreateWithDependenciesFromServices<TPage>()
                ?? throw new ArgumentException($"Cannot create page {typeof(TPage).Name}");

            return action(page);
        });
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
        => ExecuteScopeAsync(sp => action(sp.GetRequiredService<NtpDbContext>()));

    public Task ExecuteDbContextAsync(Func<NtpDbContext, IMediator, Task> action)
        => ExecuteScopeAsync(sp => action(sp.GetRequiredService<NtpDbContext>(), sp.GetRequiredService<IMediator>()));

    public Task<T> ExecuteDbContextAsync<T>(Func<NtpDbContext, Task<T>> action)
        => ExecuteScopeAsync(sp => action(sp.GetRequiredService<NtpDbContext>()));

    public Task<T> ExecuteDbContextAsync<T>(Func<NtpDbContext, ValueTask<T>> action)
        => ExecuteScopeAsync(sp => action(sp.GetRequiredService<NtpDbContext>()).AsTask());

    public Task<T> ExecuteDbContextAsync<T>(Func<NtpDbContext, IMediator, Task<T>> action)
        => ExecuteScopeAsync(sp => action(sp.GetRequiredService<NtpDbContext>(), sp.GetRequiredService<IMediator>()));

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

    public Task ResetDatabase() => ExecuteDbContextAsync(db =>
    {
        return db.Database.EnsureCreatedAsync();
    });

    public Task InitializeAsync() => ResetDatabase();

    public Task DisposeAsync()
    {
        _factory?.Dispose();
        return Task.CompletedTask;
    }
}

public class ScopedPageExecutor<T> where T : class
{
    private IServiceScopeFactory scopeFactory;

    public ScopedPageExecutor(IServiceScopeFactory scopeFactory) => this.scopeFactory = scopeFactory;

    internal async Task<TResult> Execute<TResult>(Func<T, Task<TResult>> action)
    {
        using var scope = scopeFactory.CreateScope();

        var page = scope.ServiceProvider.CreateWithDependenciesFromServices<T>()
                ?? throw new ArgumentException($"Cannot create page {typeof(T).Name}");

        var dbContext = scope.ServiceProvider.GetRequiredService<NtpDbContext>();

        try
        {
            await dbContext.Database.BeginTransactionAsync();

            var result = await action(page);

            await dbContext.Database.CommitTransactionAsync();

            return result;
        }
        catch (Exception)
        {
            dbContext.Database.RollbackTransaction();
            throw;
        }
    }
}