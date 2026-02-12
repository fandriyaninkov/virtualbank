using System.IdentityModel.Tokens.Jwt;
using System.Net;

namespace IdentityService.Tests;

public class AuthEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthEndpointsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_ReturnsOk_ForNewUser()
    {
        var response = await _client.PostAsync($"/auth/register?email={Uri.EscapeDataString("newuser@example.com")}&password=Secret123!", null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Register_ReturnsBadRequest_ForDuplicateUser()
    {
        var email = Uri.EscapeDataString("duplicate@example.com");

        var firstResponse = await _client.PostAsync($"/auth/register?email={email}&password=Secret123!", null);
        var secondResponse = await _client.PostAsync($"/auth/register?email={email}&password=Secret123!", null);

        Assert.Equal(HttpStatusCode.OK, firstResponse.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, secondResponse.StatusCode);
    }

    [Fact]
    public async Task Login_ReturnsJwt_ForValidCredentials()
    {
        var email = Uri.EscapeDataString("login-ok@example.com");

        await _client.PostAsync($"/auth/register?email={email}&password=Secret123!", null);
        var loginResponse = await _client.PostAsync($"/auth/login?email={email}&password=Secret123!", null);

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

        await _client.PostAsync($"/auth/register?email={email}&password=Secret123!", null);
        var loginResponse = await _client.PostAsync($"/auth/login?email={email}&password=WrongPass!", null);

        Assert.Equal(HttpStatusCode.Unauthorized, loginResponse.StatusCode);
    }
}
