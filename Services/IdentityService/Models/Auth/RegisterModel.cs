namespace IdentityService.Models.Auth;

public readonly record struct RegisterModel(bool Succes, string Error = null);
