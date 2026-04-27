using CSharpFunctionalExtensions;
using EduPlatform.Application.Abstractions.Repositories;
using EduPlatform.Application.Contracts.Defaults;
using EduPlatform.Application.Mappers;

namespace EduPlatform.Application.Handlers.Users;

public class GetAllUsersHandler(IUsersRepository repository)
{
    private readonly IUsersRepository _repository = repository;

    public async Task<Result<IEnumerable<UserResponse>>> HandleAsync(CancellationToken cancellationToken)
    {
        var users = await _repository.GetAllAsync(cancellationToken);

        if (users.Count() == 0)
            return Result.Failure<IEnumerable<UserResponse>>("Users list not found");

        return users.Select(u => u.ToUserResponse()).ToList();
    }
}