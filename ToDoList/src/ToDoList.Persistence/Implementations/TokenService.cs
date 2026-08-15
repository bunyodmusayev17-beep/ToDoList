using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ToDoList.Application.Abstractions;
using ToDoList.Application.Dtos;
using ToDoList.Application.Settings;

namespace ToDoList.Infrastructure.Implementations;

public class TokenService : ITokenService
{
    private readonly JwtSettings JwtSettings;

    public TokenService(JwtSettings jwtSettings)
    {
        JwtSettings = jwtSettings;
    }

    public string GetToken(UserGetDto userGetDto)
    {
        var IdentityClaims = new Claim[]
        {
        new Claim("UserId",userGetDto.UserId.ToString()),
        new Claim("FirstName",userGetDto.FirstName.ToString()),
        new Claim("LastName",userGetDto.LastName.ToString()),
        new Claim("UserName",userGetDto.UserName.ToString()),
        new Claim(ClaimTypes.Role,userGetDto.Role.ToString()),
        new Claim(ClaimTypes.Email,userGetDto.Email.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSettings.SecretKey));
        var keyCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expiresMinutes = JwtSettings.Lifetime;
        var token = new JwtSecurityToken(
            issuer: JwtSettings.Issuer,
            audience: JwtSettings.Audience,
            claims: IdentityClaims,
            expires: DateTime.UtcNow.AddMinutes(expiresMinutes),
            signingCredentials: keyCredentials
            );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }
}
