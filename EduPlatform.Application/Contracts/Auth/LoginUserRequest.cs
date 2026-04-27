namespace EduPlatform.Application.Contracts.Auth;

public record LoginUserRequest(
    string Email,
    string Password);