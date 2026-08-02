using AIResumeAnalyzer.Application.Interfaces.Auth;
using AIResumeAnalyzer.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AIResumeAnalyzer.Domain.Common;

namespace AIResumeAnalyzer.Infrastructure.Services.Auth;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtOptions _jwtOptions;

    public JwtTokenGenerator(IOptions<JwtOptions> jwtOptions)
    {
        _jwtOptions = jwtOptions.Value;

        Console.WriteLine("======================================");
        Console.WriteLine("JWT FROM IOPTIONS");
        Console.WriteLine("======================================");
        Console.WriteLine($"Issuer     : '{_jwtOptions.Issuer}'");
        Console.WriteLine($"Audience   : '{_jwtOptions.Audience}'");
        Console.WriteLine($"Secret Key : '{_jwtOptions.SecretKey}'");
        Console.WriteLine($"Expiry     : {_jwtOptions.ExpiryMinutes}");
        Console.WriteLine("======================================");
    }

    public string GenerateToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
            new Claim(ClaimTypes.Role, user.Role.Name)
        };

        if (string.IsNullOrWhiteSpace(_jwtOptions.SecretKey))
        {
            throw new InvalidOperationException(
                "JWT SecretKey is empty. Check Jwt configuration binding.");
        }

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiryMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}