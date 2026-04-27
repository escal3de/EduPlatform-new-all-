using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace EduPlatform.Api.Extensions;

public static class GetUserIdExtension
{
    public static Guid? GetUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirst(JwtRegisteredClaimNames.Sub) 
                    ?? user.FindFirst(JwtRegisteredClaimNames.UniqueName)
                    ?? user.FindFirst(ClaimTypes.NameIdentifier);
        
        if (value != null && Guid.TryParse(value.Value, out Guid guid))
        {
            return guid;
        }

        return null;
    }

    public static Guid GetRequiredUserId(this ClaimsPrincipal user)
    {
        var id = user.GetUserId();

        if (id is null)
            throw new InvalidOperationException("User id claim is missing or invalid");

        return id.Value;
    }
}
