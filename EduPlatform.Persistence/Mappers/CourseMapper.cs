using EduPlatform.Domain;
using EduPlatform.Persistence.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace EduPlatform.Persistence.Mappers;

public static class CourseMapper
{
    public static CourseEntity ToEntity(this Course course) => new()
    {
        Id = course.Id,
        Name = course.Name,
        Description = course.Description,
        Lessons = course.Lessons.Select(l => l.ToEntity()).ToList()
    };

    public static Course ToDomain(this CourseEntity entity) =>
        Course.Load(entity.Id, entity.Name, entity.Description, entity.Lessons.Select(l => l.ToDomain()));
}