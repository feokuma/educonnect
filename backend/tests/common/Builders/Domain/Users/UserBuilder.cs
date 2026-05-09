using Bogus;
using EduConnect.Domain.Users;

namespace EduConnect.Tests.Common.Builders.Domain.Users;

public sealed class UserBuilder : Faker<User>
{
    private DateTimeOffset? _createdAt;
    private string? _email;
    private Guid? _id;
    private string? _name;

    public UserBuilder()
    {
        CustomInstantiator(faker => User.Restore(
            _id ?? faker.Random.Guid(),
            _name ?? faker.Name.FullName(),
            _email ?? faker.Internet.Email(),
            _createdAt ?? faker.Date.PastOffset()));
    }

    public UserBuilder WithCreatedAt(DateTimeOffset createdAt)
    {
        _createdAt = createdAt;

        return this;
    }

    public UserBuilder WithEmail(string email)
    {
        _email = email;

        return this;
    }

    public UserBuilder WithId(Guid id)
    {
        _id = id;

        return this;
    }

    public UserBuilder WithName(string name)
    {
        _name = name;

        return this;
    }
}
