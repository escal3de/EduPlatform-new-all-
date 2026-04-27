namespace EduPlatform.Persistence.Entities;

public class CourseEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid AuthorId { get; set; }
    public UserEntity Author { get; set; } = null!;
    public ICollection<LessonEntity> Lessons { get; set; } = new List<LessonEntity>();
}