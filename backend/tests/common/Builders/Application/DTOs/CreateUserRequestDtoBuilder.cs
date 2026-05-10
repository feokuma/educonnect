using Bogus;
using EduConnect.Application.DTOs;

namespace EduConnect.Tests.Common.Builders.Application.DTOs;

public sealed class CreateUserRequestDtoBuilder : Faker<CreateUserRequestDto>
{
    private string? _email;
    private string? _name;
    private string? _passwordHash;
    private string? _username;

    public CreateUserRequestDtoBuilder()
    {
        CustomInstantiator(faker => new CreateUserRequestDto(
            _name ?? faker.Name.FullName(),
            _email ?? faker.Internet.Email(),
            _username ?? faker.Internet.UserName(),
            _passwordHash ?? faker.Random.Hash()));
    }

    public CreateUserRequestDtoBuilder WithEmail(string email)
    {
        _email = email;

        return this;
    }

    public CreateUserRequestDtoBuilder WithName(string name)
    {
        _name = name;

        return this;
    }

    public CreateUserRequestDtoBuilder WithPasswordHash(string passwordHash)
    {
        _passwordHash = passwordHash;

        return this;
    }

    public CreateUserRequestDtoBuilder WithUsername(string username)
    {
        _username = username;

        return this;
    }
}
