using IdentityService.DB;
using IdentityService.Services;
using IdentityService.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace IdentityService.Endpoints;

public static class Auth
{
    public static void RegisterAuthEndpoints(this IEndpointRouteBuilder routes)
    {
        var auth = routes.MapGroup("/auth");

        auth.MapPost("/register", async (string email, string password, ApplicationDbContext db) =>
        {
            var already = await db.Users
                .Where(u => u.Email == email)
                .FirstOrDefaultAsync();
            if (already != null)
                return Results.Problem("Пользователь с таким email уже существует", statusCode: StatusCodes.Status400BadRequest);

            var user = new User
            {
                Email = email,
                CreatedAt = DateTime.Now,
                PasswordHash = CrypterService.Crypt(password),
            };
            db.Users.Add(user);
            await db.SaveChangesAsync();

            return Results.Ok();
        });

        auth.MapPost("/login", async (string email, string password, ApplicationDbContext db, IOptions<AppSettings> options) =>
        {
            var user = await db.Users
                .Where(x => x.Email == email)
                .FirstOrDefaultAsync();

            if (user == null)
                return TypedResults.Problem("Пользователя с таким email не существует", statusCode: StatusCodes.Status401Unauthorized);

            if (!CrypterService.Verify(user.PasswordHash, password))
                return TypedResults.Problem("Неверный пароль", statusCode: StatusCodes.Status401Unauthorized);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email)
            };
            var settings = options.Value.Jwt;
            var jwt = new JwtSecurityToken(
                issuer: settings.Issuer,
                audience: settings.Audince,
                claims: claims,
                expires: DateTime.Now.Add(TimeSpan.FromMinutes(20)),
                signingCredentials: new SigningCredentials(settings.GetSymmetricSecurityKey, SecurityAlgorithms.HmacSha256));

            return Results.Ok(new JwtSecurityTokenHandler().WriteToken(jwt));
        });
    }
}
