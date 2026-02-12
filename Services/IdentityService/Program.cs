using IdentityService.DB;
using IdentityService.Endpoints;
using IdentityService.Services;
using IdentityService.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddOptions<AppSettings>()
    .BindConfiguration("")
    .Validate(settings =>
    {
        var jwt = settings.Jwt;

        return jwt != null
               && !string.IsNullOrWhiteSpace(jwt.Issuer)
               && !string.IsNullOrWhiteSpace(jwt.Audience)
               && !string.IsNullOrWhiteSpace(jwt.Key)
               && jwt.Key.Length >= 16;
    }, "Jwt settings are invalid. Issuer, Audience and Key are required, and Key must be at least 16 chars.")
    .ValidateOnStart();

var connection = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection is not configured.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connection));

builder.Services.AddTransient<IAuthService, AuthService>();

var serviceProvider = builder.Services.BuildServiceProvider();
var settings = serviceProvider.GetRequiredService<IOptions<AppSettings>>();
var jwt = settings.Value.Jwt;
if (jwt == null)
    throw new ArgumentNullException(nameof(jwt));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt.Issuer,
            ValidateAudience = true,
            ValidAudience = jwt.Audience,
            ValidateLifetime = true,
            IssuerSigningKey = jwt.GetSymmetricSecurityKey,
            ValidateIssuerSigningKey = true
        });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", (ApplicationDbContext db) => db.Database.CanConnect());
app.RegisterAuthEndpoints();
app.UseHttpsRedirection();

app.Run();

public partial class Program;
