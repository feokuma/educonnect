namespace EduConnect.Domain.Users;

public sealed class User
{
    private User(Guid id, string name, string email, DateTimeOffset createdAt)
    {
        Id = id;
        Name = name;
        Email = email;
        CreatedAt = createdAt;
    }

    public Guid Id { get; }

    public string Name { get; }

    public string Email { get; }

    public DateTimeOffset CreatedAt { get; }

    public static User Create(Guid id, string name, string email)
    {
        return new User(id, name, email, DateTimeOffset.UtcNow);
    }

    public static User Restore(Guid id, string name, string email, DateTimeOffset createdAt)
    {
        return new User(id, name, email, createdAt);
    }
}
