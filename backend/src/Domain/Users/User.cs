namespace EduConnect.Domain.Users;

public sealed class User
{
    private User(Guid id, string name, string email, string username, string passwordHash, DateTimeOffset createdAt)
    {
        Id = id;
        Name = name;
        Email = email;
        Username = username;
        PasswordHash = passwordHash;
        CreatedAt = createdAt;
    }

    public Guid Id { get; }

    public string Name { get; }

    public string Email { get; }

    public string Username { get; }

    public string PasswordHash { get; }

    public DateTimeOffset CreatedAt { get; }

    public static User Create(Guid id, string name, string email, string username, string passwordHash)
    {
        return new User(id, name, email, username, passwordHash, DateTimeOffset.UtcNow);
    }

    public static User Restore(
        Guid id,
        string name,
        string email,
        string username,
        string passwordHash,
        DateTimeOffset createdAt)
    {
        return new User(id, name, email, username, passwordHash, createdAt);
    }
}
