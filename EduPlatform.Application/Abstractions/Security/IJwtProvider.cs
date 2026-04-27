using EduPlatform.Domain;

namespace EduPlatform.Application.Abstractions.Security;

public interface IJwtProvider
{
    string GenerateJwt(User user);
}