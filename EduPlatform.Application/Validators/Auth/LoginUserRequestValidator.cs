using EduPlatform.Application.Contracts.Auth;
using FluentValidation;

namespace EduPlatform.Application.Validators.Auth;

public class LoginUserRequestValidator : AbstractValidator<LoginUserRequest>
{
    public LoginUserRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .Length(8, 255);
        
        RuleFor(x => x.Password)
            .NotEmpty()
            .Length(8, 255);
    }
}