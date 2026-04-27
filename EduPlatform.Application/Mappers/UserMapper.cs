using EduPlatform.Application.Contracts.Auth;
using EduPlatform.Application.Contracts.Defaults;
using EduPlatform.Domain;

namespace EduPlatform.Application.Mappers;

public static class UserMapper
{
    public static LoginResponse ToLoginResponse(this User user, string token) =>
        new LoginResponse(token, user.Name, user.Email);

    public static UserResponse ToUserResponse(this User user) =>
        new UserResponse(user.Id, user.Name, user.Email, user.Roles.Select(r => r.ToString()).ToList());
}