using System.Text;
using EduPlatform.Domain.Permissions;
using EduPlatform.Infrastructure.Realisations.Security.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace EduPlatform.Api.ServiceCollections;

public static class CustomAuth
{
    public static IServiceCollection AddCustomAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetRequiredSection("JwtOptions"));

        var jwtOptions = configuration.GetRequiredSection("JwtOptions").Get<JwtOptions>()
                         ?? throw new InvalidOperationException(nameof(JwtOptions));

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
                };

                options.Events = new JwtBearerEvents()
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Cookies["accessToken"];

                        if (!string.IsNullOrWhiteSpace(accessToken))
                            context.Token = accessToken;

                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("StudentOnly", p => p.RequireRole("Student"));
            options.AddPolicy("TeacherOnly", p => p.RequireRole("Teacher"));
            options.AddPolicy("AdminOnly", p => p.RequireRole("Admin"));
        });

        return services;
    }
}