using CSharpFunctionalExtensions;
using EduPlatform.Application.Abstractions.Repositories;
using EduPlatform.Application.Contracts.Defaults;
using EduPlatform.Application.Mappers;

namespace EduPlatform.Application.Handlers.Users;

public class GetMeHandler(IUsersRepository repository)
{
    private readonly IUsersRepository _repository = repository;

    public async Task<Result<UserResponse>> HandleAsync(Guid id, CancellationToken cancellationToken)
    {
        var user = await _repository.GetByIdAsync(id, cancellationToken);

        if (user is null)
            return Result.Failure<UserResponse>("User not found");

        return user.ToUserResponse();
    }
}