using EduPlatform.Domain;
using EduPlatform.Persistence.Entities;

namespace EduPlatform.Persistence.Mappers;

public static class UserMapper
{
    public static UserEntity ToEntity(this User user) =>
        new()
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            HashedPassword = user.HashedPassword,
            Roles = user.Roles
        };

    public static User ToDomain(this UserEntity entity) =>
        User.Load(entity.Id, entity.Name, entity.Email, entity.HashedPassword, entity.Roles);
}
