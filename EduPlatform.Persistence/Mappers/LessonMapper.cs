using EduPlatform.Domain;
using EduPlatform.Persistence.Entities;

namespace EduPlatform.Persistence.Mappers;

public static class LessonMapper
{
    public static LessonEntity ToEntity(this Lesson lesson) => new()
    {
        Id = lesson.Id,
        Name = lesson.Name,
        Description = lesson.Description
    };

    public static Lesson ToDomain(this LessonEntity entity) => Lesson.Load(entity.Id, entity.Name, entity.Description);
}