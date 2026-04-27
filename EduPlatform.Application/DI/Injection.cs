using EduPlatform.Application.Contracts.Auth;
using EduPlatform.Application.Contracts.Defaults;
using EduPlatform.Application.Handlers.Auth;
using EduPlatform.Application.Handlers.Users;
using EduPlatform.Application.Validators.Auth;
using EduPlatform.Application.Validators.Defaults;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EduPlatform.Application.DI;

public static class Injection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // validators
        services.AddScoped<IValidator<RegisterUserRequest>, RegisterUserRequestValidator>();
        services.AddScoped<IValidator<LoginUserRequest>, LoginUserRequestValidator>();
        
        // another validators
        services.AddScoped<IValidator<UpdateUserRequest>, UpdateUserRequestValidator>();
        
        // handlers
        services.AddScoped<RegisterUserHandler>();
        services.AddScoped<LoginUserHandler>();
        
        // another handlers
        services.AddScoped<UpdateUserHandler>();
        services.AddScoped<GetUserHandler>();
        services.AddScoped<GetAllUsersHandler>();
        services.AddScoped<GetMeHandler>();
        services.AddScoped<DeleteUserHandler>();

        return services;
    }
}