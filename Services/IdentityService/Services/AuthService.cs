using IdentityService.DB;
using IdentityService.Models.Auth;
using IdentityService.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;

namespace IdentityService.Services;

public interface IAuthService
{
    /// <summary>Регистрация</summary>
    Task<RegisterModel> RegisterAsync(string email, string password);
    /// <summary>Авторизация</summary>
    Task<LoginModel> LoginAsync(string email, string password);
}

public class AuthService(ApplicationDbContext db, IOptions<AppSettings> options) : IAuthService
{
    public async Task<RegisterModel> RegisterAsync(string email, string password)
    {
        var already = await db.Users
            .Where(u => u.Email == email)
            .FirstOrDefaultAsync();
        if (already != null)
            return new(false, "Пользователь с таким email уже существует");

        var user = new User
        {
            Email = email,
            CreatedAt = DateTime.UtcNow,
            PasswordHash = CrypterService.Crypt(password),
        };
        db.Users.Add(user);
        await db.SaveChangesAsync();

        return new(true);
    }

    public async Task<LoginModel> LoginAsync(string email, string password)
    {
        var user = await db.Users
                .Where(x => x.Email == email)
                .FirstOrDefaultAsync();

        if (user == null)
            return new(null, "Неверный логин и/или пароль");

        if (!CrypterService.Verify(user.PasswordHash, password))
            return new(null, "Неверный логин и/или пароль");

        var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };
        var settings = options.Value.Jwt;
        var jwt = new JwtSecurityToken(
            issuer: settings.Issuer,
            audience: settings.Audience,
            claims: claims,
            expires: DateTime.Now.Add(TimeSpan.FromMinutes(20)),
            signingCredentials: new SigningCredentials(settings.GetSymmetricSecurityKey, SecurityAlgorithms.HmacSha256));

        return new(new JwtSecurityTokenHandler().WriteToken(jwt), null);
    }
}
