using EduPlatform.Application.Contracts.Defaults;
using FluentValidation;

namespace EduPlatform.Application.Validators.Defaults;

public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        RuleFor(request => request.Name)
            .Length(3, 50);
        
        RuleFor(request => request.Email)
            .EmailAddress()
            .Length(8, 255);
        
        RuleFor(request => request.Password)
            .Length(8, 255);
    }
}