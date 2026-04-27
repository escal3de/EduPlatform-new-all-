using CSharpFunctionalExtensions;

namespace EduPlatform.Domain;

public class Lesson
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }

    private Lesson(Guid id, string name, string description)
    {
        Id = id;
        Name = name;
        Description = description;
    }
    
    private Lesson(string name, string description) : this(Guid.NewGuid(), name, description) {}

    public static Result<Lesson> Create(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(description))
            return Result.Failure<Lesson>("Invalid data");

        var lesson = new Lesson(name, description);

        return Result.Success(lesson);
    }
    
    public static Lesson Load(Guid id, string name, string description)
    {
        return new Lesson(id, name, description);
    }
}