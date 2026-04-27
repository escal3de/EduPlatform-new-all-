namespace EduPlatform.Persistence.Entities;

public class LessonEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid CourseId { get; set; }
    public CourseEntity Course { get; set; } = null!;
}