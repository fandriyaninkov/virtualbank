using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace IdentityService.Auth;

public class AuthOptions
{
    /// <summary>Издатель токена</summary>
    public const string Issuer = "VirtualBankServer";
    /// <summary>Потребитель токена</summary>
    public const string Audince = "VirtualBankClient";
    /// <summary>Ключ для шифрования</summary>
    public const string Key = "s3cr3tv1rt83lb3nk";

    public static SymmetricSecurityKey GetSymmetricSecurityKey
        => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
}
