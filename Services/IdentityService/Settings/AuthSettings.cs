using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace IdentityService.Settings;

public class AuthSettings
{
    /// <summary>Издатель токена</summary>
    public required string Issuer { get; set; }

    /// <summary>Потребитель токена</summary>
    public required string Audience { get; set; }

    /// <summary>Ключ для шифрования</summary>
    public required string Key { get; set; }

    public SymmetricSecurityKey GetSymmetricSecurityKey
        => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
}
