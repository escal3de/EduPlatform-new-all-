using EduPlatform.Application.Contracts.Auth;
using FluentValidation;

namespace EduPlatform.Application.Validators.Auth;

public class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequest>
{
    public RegisterUserRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .Length(3, 50);
        
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .Length(8, 255);
        
        RuleFor(x => x.Password)
            .NotEmpty()
            .Length(8, 255);
    }
}