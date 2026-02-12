using IdentityService.DB;
using IdentityService.Endpoints;
using IdentityService.Services;
using IdentityService.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddOptions<AppSettings>()
    .BindConfiguration("");

var connection = builder.Configuration.GetConnectionString("DefaultConnection");
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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/", (ApplicationDbContext db) => db.Database.CanConnect());
app.RegisterAuthEndpoints();
app.UseHttpsRedirection();

app.Run();

