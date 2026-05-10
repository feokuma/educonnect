using Bogus;
using EduConnect.Application.DTOs;

namespace EduConnect.Tests.Common.Builders.Application.DTOs;

public sealed class CreateUserRequestDtoBuilder : Faker<CreateUserRequestDto>
{
    private string? _email;
    private string? _name;
    private string? _password;
    private string? _username;

    public CreateUserRequestDtoBuilder()
    {
        CustomInstantiator(faker => new CreateUserRequestDto(
            _name ?? faker.Name.FullName(),
            _email ?? faker.Internet.Email(),
            _username ?? faker.Internet.UserName(),
            _password ?? faker.Internet.Password()));
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

    public CreateUserRequestDtoBuilder WithPassword(string password)
    {
        _password = password;

        return this;
    }

    public CreateUserRequestDtoBuilder WithUsername(string username)
    {
        _username = username;

        return this;
    }
}
