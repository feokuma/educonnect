using EduConnect.Application.Repositories;
using EduConnect.Domain.Users;
using EduConnect.Infrastructure.Persistence.Entities;

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
            CreatedAt = user.CreatedAt
        };

        dbContext.Users.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return User.Restore(entity.Id, entity.Name, entity.Email, entity.CreatedAt);
    }
}
