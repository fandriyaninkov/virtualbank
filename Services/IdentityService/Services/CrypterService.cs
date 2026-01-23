namespace IdentityService.Services;

public static class CrypterService
{
    /// <summary>Зашифровать пароль</summary>
    public static string Crypt(string password)
        => BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt());

    /// <summary>Проверить пароль по хэшу</summary>
    public static bool Verify(string hash, string password)
        => BCrypt.Net.BCrypt.Verify(password, hash);
}
