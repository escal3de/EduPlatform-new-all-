namespace EduPlatform.Application.Contracts.Auth;

public record LoginResponse(
    string Token, 
    string Name,
    string Email);