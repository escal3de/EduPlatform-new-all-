using System.Security.Claims;
using EduPlatform.Api.Extensions;
using EduPlatform.Application.Contracts.Defaults;
using EduPlatform.Application.Handlers.Users;
using EduPlatform.Domain;
using Microsoft.AspNetCore.Mvc;

namespace EduPlatform.Api.Endpoints;

public static class UsersEndpoints
{
    public static IEndpointRouteBuilder MapUsersEndpoint(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/users")
            .RequireAuthorization();

        group.MapGet("/me",
            async (ClaimsPrincipal user, [FromServices] GetMeHandler handler,
                CancellationToken cancellationToken = default) =>
            {
                var currentId = user.GetRequiredUserId();

                var result = await handler.HandleAsync(currentId, cancellationToken);

                return result.IsFailure
                    ? Results.BadRequest(result.Error)
                    : Results.Ok(result.Value);
            });

        group.MapGet("/",
            async ([FromServices] GetAllUsersHandler handler, CancellationToken cancellationToken = default) =>
            {
                var result = await handler.HandleAsync(cancellationToken);

                return result.IsFailure
                    ? Results.BadRequest(result.Error)
                    : Results.Ok(result.Value);
            });

        group.MapGet("/{data}",
            async (string data, [FromServices] GetUserHandler handler, CancellationToken cancellationToken = default) =>
            {
                var result = await handler.HandleAsync(data, cancellationToken);

                return result.IsFailure
                    ? Results.BadRequest(result.Error)
                    : Results.Ok(result.Value);
            });

        group.MapPatch("/{id}",
            async (
                Guid id,
                UpdateUserRequest request,
                [FromServices] UpdateUserHandler handler,
                ClaimsPrincipal user,
                CancellationToken cancellationToken = default) =>
            {
                var currentId = user.GetRequiredUserId();
                var claims = user.FindAll(ClaimTypes.Role)
                    .Select(c => Enum.Parse<UserRole>(c.Value));

                var result = await handler.HandleAsync(request, id, currentId, claims.ToList(), cancellationToken);

                return result.IsFailure
                    ? Results.BadRequest(result.Error)
                    : Results.Ok(result.Value);
            });

        group.MapDelete("/{id}",
            async (
                Guid id,
                ClaimsPrincipal user,
                [FromServices] DeleteUserHandler handler,
                CancellationToken cancellationToken = default) =>
            {
                var currentId = user.GetRequiredUserId();
                var claims = user.FindAll(ClaimTypes.Role)
                    .Select(c => Enum.Parse<UserRole>(c.Value));

                var result = await handler.HandleAsync(id, currentId, claims.ToList(), cancellationToken);

                return result.IsFailure
                    ? Results.BadRequest(result.Error)
                    : Results.NoContent();
            });

        return group;
    }
}
