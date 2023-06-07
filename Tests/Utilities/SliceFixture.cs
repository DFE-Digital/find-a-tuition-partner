using Application.Common.Interfaces;
using Application.Constants;
using Domain.Search;
using Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notify.Exceptions;
using Notify.Interfaces;
using Notify.Models.Responses;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Tests.TestData;
using UI;

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
    public ITuitionPartnerService TuitionPartnerService => _factory.Services.GetRequiredService<ITuitionPartnerService>();
    public ILookupDataService LookupDataService => _factory.Services.GetRequiredService<ILookupDataService>();

    private class NtpApplicationFactory
        : WebApplicationFactory<Program>
    {
        public NtpApplicationFactory()
        {
            LocationFilter = Substitute.For<ILocationFilterService>();

            var locationsToAdd = new List<District>()
            {
                District.EastRidingOfYorkshire,
                District.NorthEastLincolnshire,
                District.Ryedale
            };

            var school = A.School;

            foreach (var locationToAdd in locationsToAdd)
            {
                LocationFilter
                    .GetLocationFilterParametersAsync(locationToAdd.SamplePostcode)
                    .Returns(new LocationFilterParameters
                    {
                        Country = "England",
                        LocalAuthorityDistrictCode = locationToAdd.Code,
                        LocalAuthorityDistrictId = locationToAdd.Id,
                        LocalAuthorityDistrict = locationToAdd.Name,
                        LocalAuthority = locationToAdd.LocalAuthorityName,
                        Schools = new List<Domain.School>()
                        {
                            school
                        },
                        Urn = school.Urn
                    });
            }

            LocationFilter
                .GetLocationFilterParametersAsync(Arg.Is<string>(x => !locationsToAdd.Any(e => e.SamplePostcode == x)))
                .Returns(new LocationFilterParameters
                {
                    Country = "England",
                    LocalAuthorityDistrictCode = District.Dacorum.Code,
                    LocalAuthorityDistrictId = District.Dacorum.Id,
                    LocalAuthorityDistrict = District.Dacorum.Name,
                    LocalAuthority = District.Dacorum.LocalAuthorityName
                });


            SessionService = new Mock<ISessionService>();

            SessionService.Setup(nc =>
                    nc.SessionDataExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            SessionService.Setup(nc =>
                    nc.AddOrUpdateDataAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            SessionService.Setup(nc =>
                    nc.AddOrUpdateDataAsync(It.IsAny<Dictionary<string, string>>(), It.IsAny<string>()));

            SessionService.Setup(nc => nc.RetrieveDataByKeyAsync(SessionKeyConstants.EmailVerificationPasscode, It.IsAny<string>()))
                .ReturnsAsync("999999");

            SessionService.Setup(nc => nc.RetrieveDataByKeyAsync(SessionKeyConstants.EmailToBeVerified, It.IsAny<string>()))
                .ReturnsAsync("email@example.com");

            NotificationClient = new Mock<IAsyncNotificationClient>();

            NotificationClient.Setup(nc =>
                    nc.SendEmailAsync(It.Is<string>(x => !x.Contains("error")), It.IsAny<string>(),
                        It.IsAny<Dictionary<string, dynamic>>(),
                        It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new EmailNotificationResponse
                {
                    id = "id",
                    reference = "reference",
                    uri = "uri",
                    content = new EmailResponseContent(),
                    template = new Notify.Models.Template()
                });

            NotificationClient.Setup(nc =>
                    nc.SendEmailAsync(It.Is<string>(x => x.Contains("400error")), It.IsAny<string>(),
                        It.IsAny<Dictionary<string, dynamic>>(),
                        It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new NotifyClientException("Status code 400. Error: An error worth retry"));

            NotificationClient.Setup(nc =>
                    nc.SendEmailAsync(It.Is<string>(x => x.Contains("500error")), It.IsAny<string>(),
                        It.IsAny<Dictionary<string, dynamic>>(),
                        It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new NotifyClientException("Status code 500. Error: Some serious issue"));

        }

        public ILocationFilterService LocationFilter { get; }
        public Mock<ISessionService> SessionService { get; }
        public Mock<IAsyncNotificationClient> NotificationClient { get; }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureAppConfiguration(configure =>
            {
                configure.AddInMemoryCollection(new Dictionary<string, string>
                {
                   {"AppLogging:TcpSinkUri", ""},
                });
            });

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

                services.Remove<ISessionService>();
                services.AddSingleton(SessionService.Object);

                services.AddSingleton(NotificationClient.Object);

                services.AddMvc().AddControllersAsServices();
            });
        }
    }

    public ScopedPageExecutor<TPage> GetPage<TPage>() where TPage : class
    {
        return new(_scopeFactory);
    }

    public Task<TPage> GetPage<TPage>(Func<TPage, Task> page) where TPage : class
    {
        return new ScopedPageExecutor<TPage>(_scopeFactory).Execute(async p =>
        {
            await page(p);
            return p;
        });
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
        catch (Exception ex)
        {
            var a = ex.Message;
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

    public Task InsertAsync<TEntity, TEntity2, TEntity3, TEntity4, TEntity5>(TEntity entity, TEntity2 entity2, TEntity3 entity3, TEntity4 entity4, TEntity5 entity5)
        where TEntity : class
        where TEntity2 : class
        where TEntity3 : class
        where TEntity4 : class
        where TEntity5 : class
    {
        return ExecuteDbContextAsync(db =>
        {
            db.Set<TEntity>().Add(entity);
            db.Set<TEntity2>().Add(entity2);
            db.Set<TEntity3>().Add(entity3);
            db.Set<TEntity4>().Add(entity4);
            db.Set<TEntity5>().Add(entity5);

            return db.SaveChangesAsync();
        });
    }

    public Task InsertAsync<TEntity, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6>(TEntity entity, TEntity2 entity2, TEntity3 entity3, TEntity4 entity4, TEntity5 entity5, TEntity6 entity6)
        where TEntity : class
        where TEntity2 : class
        where TEntity3 : class
        where TEntity4 : class
        where TEntity5 : class
        where TEntity6 : class
    {
        return ExecuteDbContextAsync(db =>
        {
            db.Set<TEntity>().Add(entity);
            db.Set<TEntity2>().Add(entity2);
            db.Set<TEntity3>().Add(entity3);
            db.Set<TEntity4>().Add(entity4);
            db.Set<TEntity5>().Add(entity5);
            db.Set<TEntity6>().Add(entity6);

            return db.SaveChangesAsync();
        });
    }

    public Task InsertAsync<TEntity, TEntity2, TEntity3, TEntity4, TEntity5, TEntity6, TEntity7>(TEntity entity, TEntity2 entity2, TEntity3 entity3, TEntity4 entity4, TEntity5 entity5, TEntity6 entity6, TEntity7 entity7)
        where TEntity : class
        where TEntity2 : class
        where TEntity3 : class
        where TEntity4 : class
        where TEntity5 : class
        where TEntity6 : class
        where TEntity7 : class

    {
        return ExecuteDbContextAsync(db =>
        {
            db.Set<TEntity>().Add(entity);
            db.Set<TEntity2>().Add(entity2);
            db.Set<TEntity3>().Add(entity3);
            db.Set<TEntity4>().Add(entity4);
            db.Set<TEntity5>().Add(entity5);
            db.Set<TEntity6>().Add(entity6);
            db.Set<TEntity7>().Add(entity7);

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

    internal async Task AddTuitionPartner(Domain.TuitionPartner partner)
        => await ExecuteDbContextAsync(db =>
        {
            db.TuitionPartners.Add(partner);
            return db.SaveChangesAsync();
        });

    internal async Task AddSchool(Domain.School school)
        => await ExecuteDbContextAsync(db =>
        {
            db.Schools.Add(school);
            return db.SaveChangesAsync();
        });
}

public class ScopedPageExecutor<T> where T : class
{
    private readonly IServiceScopeFactory _scopeFactory;

    public ScopedPageExecutor(IServiceScopeFactory scopeFactory) => _scopeFactory = scopeFactory;

    internal async Task<TResult> Execute<TResult>(Func<T, Task<TResult>> action)
    {
        using var scope = _scopeFactory.CreateScope();

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

    internal Task<T> OnGet(Func<T, Task> page)
        => Execute(async p =>
        {
            await page(p);
            return p;
        });
}