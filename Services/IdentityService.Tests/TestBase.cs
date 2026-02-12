using IdentityService.DB;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IdentityService.Tests;

public abstract class TestBase : IDisposable
{
    protected readonly HttpClient Client;
    private readonly string _databaseName;
    private readonly CustomWebApplicationFactory _factory;

    protected TestBase()
    {
        _databaseName = $"identity-tests-{Guid.NewGuid()}";
        _factory = new CustomWebApplicationFactory(_databaseName);
        Client = _factory.CreateClient();
    }

    public void Dispose()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            context.Database.EnsureDeleted();
        }
        _factory.Dispose();
    }

    private class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        public CustomWebApplicationFactory(string databaseName)
            => DatabaseName = databaseName;

        private string DatabaseName { get; }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Development");

            builder.ConfigureServices(services =>
            {
                services.RemoveAll<ApplicationDbContext>();
                services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
                services.RemoveAll<IDbContextOptionsConfiguration<ApplicationDbContext>>();

                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase(DatabaseName);
                });
            });
        }
    }
}

