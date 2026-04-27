using CSharpFunctionalExtensions;
using EduPlatform.Application.Abstractions.Repositories;
using EduPlatform.Domain;

namespace EduPlatform.Application.Handlers.Users;

public class DeleteUserHandler(IUsersRepository repository)
{
    private readonly IUsersRepository _usersRepository = repository;

    public async Task<Result> HandleAsync(
        Guid id,
        Guid currentUserId,
        IReadOnlyList<UserRole> roles,
        CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByIdAsync(id, cancellationToken);

        if (user is null)
            return Result.Failure("User not found");

        if (!user.CanBeManagedByAdmin(currentUserId, roles))
            return Result.Failure("You do not have permissions to delete this user");

        await _usersRepository.DeleteAsync(user, cancellationToken);

        return Result.Success();
    }
}
