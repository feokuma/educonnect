namespace EduConnect.Application.Common;

public interface IPasswordHasher
{
    string Hash(string password);
}
