using System.IdentityModel.Tokens.Jwt;
using System.Net;
using Xunit;

namespace IdentityService.Tests;

public class AuthEndpointsTests : TestBase
{
    [Fact]
    public async Task Register_ReturnsOk_ForNewUser()
    {
        var response = await Client.PostAsync($"/auth/register?email={Uri.EscapeDataString("newuser@example.com")}&password=Secret123!", null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Register_ReturnsBadRequest_ForDuplicateUser()
    {
        var email = Uri.EscapeDataString("duplicate@example.com");

        var firstResponse = await Client.PostAsync($"/auth/register?email={email}&password=Secret123!", null);
        var secondResponse = await Client.PostAsync($"/auth/register?email={email}&password=Secret123!", null);

        Assert.Equal(HttpStatusCode.OK, firstResponse.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, secondResponse.StatusCode);
    }

    [Fact]
    public async Task Login_ReturnsJwt_ForValidCredentials()
    {
        var email = Uri.EscapeDataString("login-ok@example.com");

        await Client.PostAsync($"/auth/register?email={email}&password=Secret123!", null);
        var loginResponse = await Client.PostAsync($"/auth/login?email={email}&password=Secret123!", null);

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var token = await loginResponse.Content.ReadAsStringAsync();
        token = token.Trim('"');
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        Assert.Equal("VirtualBankServer", jwt.Issuer);
        Assert.Contains("VirtualBankClient", jwt.Audiences);
    }

    [Fact]
    public async Task Login_ReturnsUnauthorized_ForWrongPassword()
    {
        var email = Uri.EscapeDataString("wrong-password@example.com");

        await Client.PostAsync($"/auth/register?email={email}&password=Secret123!", null);
        var loginResponse = await Client.PostAsync($"/auth/login?email={email}&password=WrongPass!", null);

        Assert.Equal(HttpStatusCode.Unauthorized, loginResponse.StatusCode);
    }
}
