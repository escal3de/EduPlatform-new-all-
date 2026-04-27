using System.Threading.RateLimiting;

namespace EduPlatform.Api.ServiceCollections;

public static class GlobalRateLimiter
{
    public static IServiceCollection AddGlobalRateLimiter(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: context.User.Identity?.Name ??
                                  context.Connection.RemoteIpAddress?.ToString()
                                  ?? "anonymous",
                    factory: partition => new FixedWindowRateLimiterOptions
                    {
                        AutoReplenishment = true,
                        PermitLimit = 1000,
                        QueueLimit = 0,
                        Window = TimeSpan.FromMinutes(1)
                    }));

            options.OnRejected = async (context, token) =>
            {
                var response = context.HttpContext.Response;
                response.ContentType = "application/json";
                response.StatusCode = StatusCodes.Status429TooManyRequests;
                response.Headers["Retry-After"] = "60";

                response.WriteAsJsonAsync("""{"error": "Too many requests"}""", token);
            };
        });

        return services;
    }
}