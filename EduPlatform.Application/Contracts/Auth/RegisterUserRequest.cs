namespace EduPlatform.Application.Contracts.Auth;

public record RegisterUserRequest(
    string Name,
    string Email,
    string Password);