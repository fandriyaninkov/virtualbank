using IdentityService.Services;

namespace IdentityService.Endpoints;

public static class Auth
{
    public static void RegisterAuthEndpoints(this IEndpointRouteBuilder routes)
    {
        var auth = routes.MapGroup("/auth");

        auth.MapPost("/register", async (string email, string password, IAuthService service) =>
        {
            var error = await service.RegisterAsync(email, password);
            if (error != null)
                return Results.Problem(error, statusCode: StatusCodes.Status400BadRequest);

            return Results.Ok();
        });

        auth.MapPost("/login", async (string email, string password, IAuthService service) =>
        {
            var (error, jwt) = await service.LoginAsync(email, password);
            if (error != null)
                return TypedResults.Problem("Пользователя с таким email не существует", statusCode: StatusCodes.Status401Unauthorized);

            return Results.Ok(jwt);
        });
    }
}
