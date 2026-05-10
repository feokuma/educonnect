using EduConnect.Application.Repositories;
using EduConnect.Domain.Users;
using EduConnect.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace EduConnect.Infrastructure.Persistence.Repositories;

public sealed class EfCoreUserRepository(EduConnectDbContext dbContext) : IUserRepository
{
    public async Task<User> CreateAsync(User user, CancellationToken cancellationToken = default)
    {
        var entity = new UserEntity
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Username = user.Username,
            PasswordHash = user.PasswordHash,
            CreatedAt = user.CreatedAt
        };

        dbContext.Users.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return User.Restore(
            entity.Id,
            entity.Name,
            entity.Email,
            entity.Username,
            entity.PasswordHash,
            entity.CreatedAt);
    }

    public async Task<User?> GetByEmailOrUsernameAsync(string identifier, CancellationToken cancellationToken = default)
    {
        var normalizedIdentifier = identifier.Trim().ToLowerInvariant();
        var entity = await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(
                user => user.Email.ToLower() == normalizedIdentifier
                    || user.Username.ToLower() == normalizedIdentifier,
                cancellationToken);

        return entity is null
            ? null
            : User.Restore(
                entity.Id,
                entity.Name,
                entity.Email,
                entity.Username,
                entity.PasswordHash,
                entity.CreatedAt);
    }
}
