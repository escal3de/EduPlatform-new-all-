using System.Globalization;
using CSharpFunctionalExtensions;
using EduPlatform.Application.Abstractions.Repositories;
using EduPlatform.Application.Contracts.Defaults;
using EduPlatform.Application.Mappers;

namespace EduPlatform.Application.Handlers.Users;

public class GetUserHandler(IUsersRepository repository)
{
    private readonly IUsersRepository _repository = repository;

    public async Task<Result<UserResponse>> HandleAsync(string data, CancellationToken cancellationToken)
    {
        var user = data switch
        {
            var s when s.Contains('@') => await _repository.GetByEmailAsync(s, cancellationToken),
            var s when Guid.TryParse(s, out var guid) => await _repository.GetByIdAsync(guid, cancellationToken),
            _ => null
        };

        return user is not null
            ? Result.Success(user.ToUserResponse())
            : Result.Failure<UserResponse>($"User with criteria {data} not found");
    }
}