using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EduPlatform.Application.Abstractions.Security;
using EduPlatform.Domain;
using EduPlatform.Domain.Permissions;
using EduPlatform.Infrastructure.Realisations.Security.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace EduPlatform.Infrastructure.Realisations.Security;

public class JwtProvider(IOptions<JwtOptions> options) : IJwtProvider
{
    private readonly JwtOptions _jwtOptions = options.Value;
    
    public string GenerateJwt(User user)
    {
        List<Claim> claims = new List<Claim>()
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Name, user.Name),
            new(JwtRegisteredClaimNames.Email, user.Email)
        };

        foreach (var role in user.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role.ToString()));
        }

        var allPermissions = user.Roles
            .SelectMany(role => RolePermissions.Map.TryGetValue(role, out var p) ? p : Array.Empty<string>())
            .Distinct();

        foreach (var permission in allPermissions)
        {
            claims.Add(new Claim("permissions", permission));
        }

        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            signingCredentials: signingCredentials,
            expires: DateTime.UtcNow.AddHours(_jwtOptions.ExpiresHours));
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}