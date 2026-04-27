using EduPlatform.Domain;

namespace EduPlatform.Application.Abstractions.Repositories;

public interface ICoursesRepository
{
    Task AddCourseAsync(Course course, CancellationToken cancellationToken);
    Task<Course?> GetCourseByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<Course?>> GetAllCoursesAsync(CancellationToken cancellationToken);
    Task UpdateCourseAsync(Course course, CancellationToken cancellationToken);
    Task DeleteCourseAsync(Guid id, CancellationToken cancellationToken);
    
    Task<IEnumerable<Lesson>> GetLessonsByCourseIdAsync(Guid courseId, CancellationToken cancellationToken);
}