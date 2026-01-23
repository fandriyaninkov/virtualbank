using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace IdentityService.Settings;

public class AuthSettings
{
    /// <summary>Издатель токена</summary>
    public string Issuer { get; set; }
    /// <summary>Потребитель токена</summary>
    public string Audince { get; set; }
    /// <summary>Ключ для шифрования</summary>
    public string Key { get; set; }

    public SymmetricSecurityKey GetSymmetricSecurityKey
        => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
}
