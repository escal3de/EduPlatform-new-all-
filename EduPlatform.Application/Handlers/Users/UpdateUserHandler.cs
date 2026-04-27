using CSharpFunctionalExtensions;
using EduPlatform.Application.Abstractions.Repositories;
using EduPlatform.Application.Abstractions.Security;
using EduPlatform.Application.Contracts.Defaults;
using EduPlatform.Application.Mappers;
using EduPlatform.Domain;
using FluentValidation;

namespace EduPlatform.Application.Handlers.Users;

public class UpdateUserHandler(
    IUsersRepository repository,
    IPasswordHasher passwordHasher,
    IValidator<UpdateUserRequest> validator)
{
    private readonly IUsersRepository _repository = repository;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly IValidator<UpdateUserRequest> _validator = validator;

    public async Task<Result<UserResponse>> HandleAsync(
        UpdateUserRequest request,
        Guid id,
        Guid currentUserId,
        IReadOnlyList<UserRole> roles,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return Result.Failure<UserResponse>(string.Join(string.Empty,
                validationResult.Errors.Select(e => e.ErrorMessage)));

        var user = await _repository.GetByIdAsync(id, cancellationToken);

        if (user is null)
            return Result.Failure<UserResponse>("User is not found");

        if (!user.CanBeManagedByAdmin(currentUserId, roles))
            return Result.Failure<UserResponse>("You don not have permissions to update this user");

        var hashedPassword = request.Password is not null
            ? _passwordHasher.HashPassword(request.Password)
            : user.HashedPassword;

        IReadOnlyList<UserRole>? updatedRoles = null;
        if (request.UserRoles is not null)
        {
            var parsedRoles = new List<UserRole>();

            foreach (var role in request.UserRoles)
            {
                if (!Enum.TryParse<UserRole>(role, ignoreCase: true, out var parsedRole))
                    return Result.Failure<UserResponse>($"Invalid role value: {role}");

                parsedRoles.Add(parsedRole);
            }

            updatedRoles = parsedRoles;
        }

        user.UpdateInfo(
            request.Name != null ? request.Name : user.Name,
            request.Email != null ? request.Email : user.Email,
            hashedPassword,
            updatedRoles);

        await _repository.UpdateAsync(user, cancellationToken);

        return Result.Success(user.ToUserResponse());
    }
}
