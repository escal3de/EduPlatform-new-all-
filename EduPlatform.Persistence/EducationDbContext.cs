using EduPlatform.Persistence.Configurations;
using EduPlatform.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace EduPlatform.Persistence;

public class EducationDbContext(DbContextOptions<EducationDbContext> options) : DbContext(options)
{
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<CourseEntity> Courses { get; set; }
    public DbSet<LessonEntity> Lessons { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new CourseConfiguration());
        modelBuilder.ApplyConfiguration(new LessonConfiguration());
        
        base.OnModelCreating(modelBuilder);
    }
}