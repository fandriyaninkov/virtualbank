using IdentityService.DB;

namespace IdentityService.Endpoints;

public static class Auth
{
    public static void RegisterAuthEndpoints(this IEndpointRouteBuilder routes)
    {
        var auth = routes.MapGroup("/auth");

        auth.MapPost("/register", (string email, string password, ApplicationDbContext db) =>
        {
            
        });
    }
}
