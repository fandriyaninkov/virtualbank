using IdentityService.Services;

namespace IdentityService.Endpoints;

public static class Auth
{
    public static void RegisterAuthEndpoints(this IEndpointRouteBuilder routes)
    {
        var auth = routes.MapGroup("/auth");

        auth.MapPost("/register", async (string email, string password, IAuthService service) =>
        {
            var res = await service.RegisterAsync(email, password);
            if (!res.Succes)
                return Results.Problem(res.Error, statusCode: StatusCodes.Status400BadRequest);

            return Results.Ok();
        });

        auth.MapPost("/login", async (string email, string password, IAuthService service) =>
        {
            var res = await service.LoginAsync(email, password);
            if (!string.IsNullOrEmpty(res.Error))
                return TypedResults.Problem(res.Error, statusCode: StatusCodes.Status401Unauthorized);
            if (string.IsNullOrEmpty(res.Token))
                return TypedResults.Problem("Пустой токен", statusCode: StatusCodes.Status500InternalServerError);

            return Results.Ok(res.Token);
        });
    }
}
