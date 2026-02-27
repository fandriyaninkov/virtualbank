using System.Net;

namespace IdentityService.Models.Auth;

public readonly record struct LoginModel(string Token, string Error);
