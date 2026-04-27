using System.Collections.ObjectModel;

namespace EduPlatform.Domain.Permissions;

public static class RolePermissions
{
    public static readonly IReadOnlyDictionary<UserRole, IReadOnlyCollection<string>> Map =
        new ReadOnlyDictionary<UserRole, IReadOnlyCollection<string>>(
            new Dictionary<UserRole, IReadOnlyCollection<string>>
            {
                [UserRole.Student] =
                [
                    Permissions.CourseRead,
                    Permissions.LessonsRead
                ],
                [UserRole.Author] =
                [
                    Permissions.CourseCreate,
                    Permissions.CourseUpdate,
                    Permissions.CourseDelete,
                    Permissions.LessonsCreate,
                    Permissions.LessonsUpdate,
                    Permissions.LessonsDelete
                ],
                [UserRole.Admin] =
                [
                    Permissions.UsersRead,
                    Permissions.UsersDelete,
                    Permissions.UsersUpdate
                ]
            });
}