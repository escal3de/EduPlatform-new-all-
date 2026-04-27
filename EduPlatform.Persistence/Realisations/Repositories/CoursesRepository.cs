using EduPlatform.Application.Abstractions.Repositories;
using EduPlatform.Domain;
using EduPlatform.Persistence.Mappers;
using Microsoft.EntityFrameworkCore;

namespace EduPlatform.Persistence.Realisations.Repositories;

public class CoursesRepository(EducationDbContext dbContext) : ICoursesRepository
{
    private readonly EducationDbContext _dbContext = dbContext;

    public async Task AddCourseAsync(Course course, CancellationToken cancellationToken)
    {
        await _dbContext.Courses.AddAsync(course.ToEntity(), cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Course?> GetCourseByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Courses
            .Include(c => c.Lessons)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        return entity?.ToDomain();
    }

    public async Task<IEnumerable<Course?>> GetAllCoursesAsync(CancellationToken cancellationToken)
    {
        var entities = await _dbContext.Courses
            .Include(c => c.Lessons)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return entities.Select(c => c.ToDomain());
    }

    public async Task UpdateCourseAsync(Course course, CancellationToken cancellationToken)
    {
        var existingCourse = await _dbContext.Courses
            .Include(c => c.Lessons)
            .FirstOrDefaultAsync(c => c.Id == course.Id, cancellationToken);

        if (existingCourse == null) return;

        _dbContext.Entry(existingCourse).CurrentValues.SetValues(course.ToEntity());

        foreach (var existingLesson in existingCourse.Lessons.ToList())
        {
            if (course.Lessons.All(l => l.Id != existingLesson.Id))
                _dbContext.Lessons.Remove(existingLesson);
        }

        foreach (var lessonModel in course.Lessons)
        {
            var existingLesson = existingCourse.Lessons
                .FirstOrDefault(l => l.Id == lessonModel.Id);

            if (existingLesson != null)
            {
                _dbContext.Entry(existingLesson).CurrentValues.SetValues(lessonModel.ToEntity());
            }
            else
            {
                existingCourse.Lessons.Add(lessonModel.ToEntity());
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteCourseAsync(Guid id, CancellationToken cancellationToken)
    {
        await _dbContext.Courses
            .Where(c => c.Id == id)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<IEnumerable<Lesson>> GetLessonsByCourseIdAsync(Guid courseId, CancellationToken cancellationToken)
    {
        var entity = await GetCourseByIdAsync(courseId, cancellationToken);
        return entity?.Lessons ?? Enumerable.Empty<Lesson>();
    }
}