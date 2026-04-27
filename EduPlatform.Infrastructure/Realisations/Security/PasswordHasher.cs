using Microsoft.AspNetCore.Identity;
using EduPlatform.Application.Abstractions.Security;
using EduPlatform.Domain;

namespace EduPlatform.Infrastructure.Realisations.Security;

public class PasswordHasher(PasswordHasher<User> passwordHasher) : IPasswordHasher
{
    private readonly IPasswordHasher<User> _passwordHasher = passwordHasher;

    public string HashPassword(string password)
    {
        return _passwordHasher.HashPassword(null!, password);
    }

    public bool VerifyHashedPassword(string hashedPassword, string providedPassword)
    {
        var result = _passwordHasher.VerifyHashedPassword(null!, hashedPassword, providedPassword);

        return result is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded;
    }
}