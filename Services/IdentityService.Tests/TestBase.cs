using IdentityService.DB;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace IdentityService.Tests;

public abstract class TestBase : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    protected readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    protected TestBase(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    public async Task DisposeAsync()
    {
        using var scope = _factory.Services.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await db.Database.EnsureDeletedAsync();
        await db.Database.EnsureCreatedAsync();
    }

    public Task InitializeAsync()
        => Task.CompletedTask;
}
