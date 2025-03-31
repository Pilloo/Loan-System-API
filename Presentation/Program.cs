using System.Security.Cryptography;
using Core.Domain;
using Core.Extensions;
using Core.Interfaces;
using ErrorHandling.Extensions;
using Infrastructure.Data;
using Infrastructure.Extensions;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCoreServices();
builder.Services.AddErrorHandlingService(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddIdentity<User, IdentityRole>(config => config.SignIn.RequireConfirmedEmail = true)
    .AddDefaultTokenProviders().AddEntityFrameworkStores<AuthDbContext>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(
    options =>
    {
        var jwtOptions = builder.Configuration.GetSection("Jwt");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOptions.GetValue<string>("Issuer"),
            ValidAudience = jwtOptions.GetValue<string>("Audience"),
            IssuerSigningKeyResolver = (token, securityToken, kid, parameters) =>
            {
                ICryptoService cryptoService = new CryptoService();
                RSA key = cryptoService.LoadRsaKey(jwtOptions.GetValue<string>("PublicKeyPath")!);
                return [new RsaSecurityKey(key)];
            },
        };
    }
);

builder.Services.AddControllers();
builder.Services.AddRouting(config => config.LowercaseUrls = true);
builder.Services.AddOpenApi();

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSerilogRequestLogging();
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();