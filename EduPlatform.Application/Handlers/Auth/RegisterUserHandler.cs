using CSharpFunctionalExtensions;
using EduPlatform.Application.Abstractions.Repositories;
using EduPlatform.Application.Abstractions.Security;
using EduPlatform.Application.Contracts.Auth;
using EduPlatform.Domain;
using FluentValidation;

namespace EduPlatform.Application.Handlers.Auth;

public class RegisterUserHandler(
    IUsersRepository repository,
    IPasswordHasher passwordHasher,
    IValidator<RegisterUserRequest> validator)
{
    private readonly IUsersRepository _repository = repository;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly IValidator<RegisterUserRequest> _validator = validator;

    public async Task<Result> HandleAsync(RegisterUserRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return Result.Failure(string.Join(string.Empty,
                validationResult.Errors.Select(e => e.ErrorMessage)));
        
        if (await _repository.GetByEmailAsync(request.Email, cancellationToken) != null)
            return Result.Failure("Email already exists");

        var hashedPassword = _passwordHasher.HashPassword(request.Password);
        var userResult = User.Create(
            request.Name,
            request.Email,
            hashedPassword,
            [UserRole.Student]);

        if (userResult.IsFailure)
            return Result.Failure(userResult.Error);

        await _repository.AddAsync(userResult.Value, cancellationToken);

        return Result.Success();
    }
}
