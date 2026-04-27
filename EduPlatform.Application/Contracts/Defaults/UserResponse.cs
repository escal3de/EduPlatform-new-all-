namespace EduPlatform.Application.Contracts.Defaults;

public record UserResponse(
    Guid Id,
    string Name,
    string Email,
    IEnumerable<string> Roles);