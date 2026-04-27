using EduPlatform.Domain;

namespace EduPlatform.Application.Contracts.Defaults;

public record UpdateUserRequest(
    string? Name,
    string? Email,
    string? Password,
    IReadOnlyList<string>? UserRoles);