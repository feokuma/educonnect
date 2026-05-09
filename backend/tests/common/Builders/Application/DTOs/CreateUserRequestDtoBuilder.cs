using Bogus;
using EduConnect.Application.DTOs;

namespace EduConnect.Tests.Common.Builders.Application.DTOs;

public sealed class CreateUserRequestDtoBuilder : Faker<CreateUserRequestDto>
{
    private string? _email;
    private string? _name;

    public CreateUserRequestDtoBuilder()
    {
        CustomInstantiator(faker => new CreateUserRequestDto(
            _name ?? faker.Name.FullName(),
            _email ?? faker.Internet.Email()));
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
}
