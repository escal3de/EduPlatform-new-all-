using CSharpFunctionalExtensions;
using EduPlatform.Application.Abstractions.Repositories;
using EduPlatform.Application.Abstractions.Security;
using EduPlatform.Application.Contracts.Auth;
using EduPlatform.Application.Mappers;
using FluentValidation;

namespace EduPlatform.Application.Handlers.Auth;

public class LoginUserHandler(
    IUsersRepository repository,
    IValidator<LoginUserRequest> validator,
    IPasswordHasher passwordHasher,
    IJwtProvider jwtProvider)
{
    private readonly IUsersRepository _repository = repository;
    private readonly IValidator<LoginUserRequest> _validator = validator;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly IJwtProvider _jwtProvider = jwtProvider;

    public async Task<Result<LoginResponse>> HandleAsync(LoginUserRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return Result.Failure<LoginResponse>(string.Join(string.Empty,
                validationResult.Errors.Select(e => e.ErrorMessage)));

        var user = await _repository.GetByEmailAsync(request.Email, cancellationToken);

        if (user is null)
            return Result.Failure<LoginResponse>("Invalid credentials");

        var passwordCheck = _passwordHasher.VerifyHashedPassword(user.HashedPassword, request.Password);

        if (!passwordCheck)
            return Result.Failure<LoginResponse>("Invalid credentials");

        var token = _jwtProvider.GenerateJwt(user);

        return Result.Success(user.ToLoginResponse(token));
    }
}