using EduPlatform.Application.Abstractions.Repositories;
using EduPlatform.Persistence.Realisations.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EduPlatform.Persistence.DI;

public static class Injection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<EducationDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DbConnection"));
        });
        
        services.AddScoped<IUsersRepository, UsersRepository>();
        services.AddScoped<ICoursesRepository, CoursesRepository>();

        return services;
    }
}