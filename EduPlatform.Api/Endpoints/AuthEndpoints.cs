using EduPlatform.Application.Contracts.Auth;
using EduPlatform.Application.Handlers.Auth;
using Microsoft.AspNetCore.Mvc;

namespace EduPlatform.Api.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/auth");

        group.MapPost("/register",
            async (RegisterUserRequest req, [FromServices] RegisterUserHandler handler,
                CancellationToken cancellationToken = default) =>
            {
                var result = await handler.HandleAsync(req, cancellationToken);

                return result.IsFailure
                    ? Results.BadRequest(result.Error)
                    : Results.Ok();
            });

        group.MapPost("/login",
            async (HttpContext context, LoginUserRequest req, [FromServices] LoginUserHandler handler,
                CancellationToken cancellationToken = default) =>
            {
                var result = await handler.HandleAsync(req, cancellationToken);

                if (result.IsFailure)
                    return Results.BadRequest(result.Error);

                context.Response.Cookies.Append("accessToken", result.Value.Token, new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTimeOffset.UtcNow.AddHours(12)
                });

                return Results.Ok();
            });

        group.MapGet("/logout", (HttpContext context) =>
        {
            context.Response.Cookies.Delete("accessToken", new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddHours(12)
            });

            return Results.Unauthorized();
        }).RequireAuthorization();

        return group;
    }
}