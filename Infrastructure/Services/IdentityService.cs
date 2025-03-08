using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Core.Domain;
using Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services;

public class IdentityService : IIdentityService
{
    private readonly IConfiguration _configuration;
    private readonly ICryptoService _cryptoService;
    private readonly ILogger<IdentityService> _logger;

    public IdentityService(IConfiguration configuration, ICryptoService cryptoService, ILogger<IdentityService> logger)
    {
        _configuration = configuration;
        _cryptoService = cryptoService;
        _logger = logger;
    }

    public Task<string> GenerateAccessToken(User user)
    {
        var securityKey = _cryptoService.LoadRsaKey(_configuration.GetSection("Jwt")["PrivateKeyPath"]!);
        var credentials = new SigningCredentials(new RsaSecurityKey(securityKey), SecurityAlgorithms.RsaSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: [new Claim(JwtRegisteredClaimNames.Sub, user.UserName!)],
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: credentials
        );

        try
        {
            return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }

    public async Task<string> GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return await Task.FromResult(Convert.ToBase64String(randomNumber));
        }
    }

    public Task<ClaimsPrincipal> GetClaimsFromToken(string token)
    {
        var publicKey = _cryptoService.LoadRsaKey(_configuration.GetSection("Jwt")["PublicKeyPath"]!);

        TokenValidationParameters validationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new RsaSecurityKey(publicKey)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        ClaimsPrincipal principal = tokenHandler.ValidateToken(token, validationParameters, out var securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(
                SecurityAlgorithms.RsaSha256,
                StringComparison.InvariantCultureIgnoreCase)) throw new SecurityTokenException("Invalid token");
        return Task.FromResult(principal);
    }
}