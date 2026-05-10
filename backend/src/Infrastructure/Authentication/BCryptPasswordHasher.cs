using EduConnect.Application.Common;

namespace EduConnect.Infrastructure.Authentication;

public sealed class BCryptPasswordHasher : IPasswordHasher
{
    public string Hash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }
}
