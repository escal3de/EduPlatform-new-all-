namespace EduPlatform.Infrastructure.Realisations.Security.Options;

public class JwtOptions
{
    public string SecretKey { get; set; } = string.Empty;
    public int ExpiresHours { get; set; } = 12;
}