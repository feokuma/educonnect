using EduConnect.Domain.Users;

namespace EduConnect.Application.Repositories;

public interface IUserRepository
{
    Task<User> CreateAsync(User user, CancellationToken cancellationToken = default);

    Task<User?> GetByEmailOrUsernameAsync(string identifier, CancellationToken cancellationToken = default);
}
