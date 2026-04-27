using EduPlatform.Application.Abstractions.Repositories;
using EduPlatform.Domain;
using EduPlatform.Persistence.Mappers;
using Microsoft.EntityFrameworkCore;

namespace EduPlatform.Persistence.Realisations.Repositories;

public class UsersRepository(EducationDbContext dbContext) : IUsersRepository
{
    private readonly EducationDbContext _dbContext = dbContext;

    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        await _dbContext.Users.AddAsync(user.ToEntity(), cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        return entity?.ToDomain();
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

        return entity?.ToDomain();
    }

    public async Task<IEnumerable<User?>> GetAllAsync(CancellationToken cancellationToken)
    {
        var entities = await _dbContext.Users
            .AsNoTracking()
            .OrderBy(e => e.Name)
            .ToListAsync(cancellationToken);

        return entities.Select(e => e.ToDomain());
    }

    public async Task UpdateAsync(User user, CancellationToken cancellationToken)
    {
        var existingUser = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == user.Id, cancellationToken);

        if (existingUser is null)
            return;

        existingUser.Name = user.Name;
        existingUser.Email = user.Email;
        existingUser.HashedPassword = user.HashedPassword;
        existingUser.Roles = user.Roles;

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(User user, CancellationToken cancellationToken)
    {
        await  _dbContext.Users
            .Where(u => u.Id == user.Id)
            .ExecuteDeleteAsync(cancellationToken);
    }
}
