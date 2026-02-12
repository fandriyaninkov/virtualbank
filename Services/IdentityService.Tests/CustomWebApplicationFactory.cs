using IdentityService.DB;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IdentityService.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly static string DataBaseName = $"identity-tests-{Guid.NewGuid()}";
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
                options.UseInMemoryDatabase(DataBaseName);
            });
        });
    }
}
