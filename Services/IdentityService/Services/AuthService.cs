using IdentityService.DB;
using IdentityService.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace IdentityService.Services;

public interface IAuthService
{
    /// <summary>Регистрация</summary>
    Task<string> RegisterAsync(string email, string password);
    /// <summary>Авторизация</summary>
    Task<(string Error, string Token)> LoginAsync(string email, string password);
}

public class AuthService(ApplicationDbContext db, IOptions<AppSettings> options) : IAuthService
{
    public async Task<string> RegisterAsync(string email, string password)
    {
        var already = await db.Users
            .Where(u => u.Email == email)
            .FirstOrDefaultAsync();
        if (already != null)
            return "Пользователь с таким email уже существует";

        var user = new User
        {
            Email = email,
            CreatedAt = DateTime.UtcNow,
            PasswordHash = CrypterService.Crypt(password),
        };
        db.Users.Add(user);
        await db.SaveChangesAsync();

        return null;
    }

    public async Task<(string Error, string Token)> LoginAsync(string email, string password)
    {
        var user = await db.Users
                .Where(x => x.Email == email)
                .FirstOrDefaultAsync();

        if (user == null)
            return ("Пользователя с таким email не существует", null);

        if (!CrypterService.Verify(user.PasswordHash, password))
            return ("Пользователя с таким email не существует", null);

        var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };
        var settings = options.Value.Jwt;
        var jwt = new JwtSecurityToken(
            issuer: settings.Issuer,
            audience: settings.Audince,
            claims: claims,
            expires: DateTime.Now.Add(TimeSpan.FromMinutes(20)),
            signingCredentials: new SigningCredentials(settings.GetSymmetricSecurityKey, SecurityAlgorithms.HmacSha256));

        return (null, new JwtSecurityTokenHandler().WriteToken(jwt));
    }
}
