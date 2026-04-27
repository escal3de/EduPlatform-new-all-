using CSharpFunctionalExtensions;

namespace EduPlatform.Domain;

public class Course
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }

    private readonly List<Lesson> _lessons = new();
    public IReadOnlyList<Lesson> Lessons => _lessons;

    private Course(Guid id, string name, string description)
    {
        Id = id;
        Name = name;
        Description = description;
    }

    private Course(string name, string description) : this(Guid.NewGuid(), name, description) {}

    public static Result<Course> Create(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(description))
            return Result.Failure<Course>("Invalid data");

        var course = new Course(name, description);

        return Result.Success(course);
    }

    public static Course Load(Guid id, string name, string description, IEnumerable<Lesson> lessons)
    {
        var course = new Course(id, name, description);
        foreach (var lesson in lessons)
        {
            course.AddLesson(lesson);
        }

        return course;
    }

    public void AddLesson(Lesson lesson)
    {
        _lessons.Add(lesson);
    }
}