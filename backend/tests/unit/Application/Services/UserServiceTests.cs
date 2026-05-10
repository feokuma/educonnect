using EduConnect.Application.Common;
using EduConnect.Application.DTOs;
using EduConnect.Application.Repositories;
using EduConnect.Application.Services;
using EduConnect.Domain.Users;
using EduConnect.Tests.Common.Builders.Application.DTOs;
using EduConnect.Tests.Common.Builders.Domain.Users;
using NSubstitute;
using Shouldly;

namespace EduConnect.Unit.Application.Services;

public class UserServiceTests
{
    private static readonly Guid GeneratedUserId = Guid.Parse("018f1f7e-6b5a-7f9b-9b6c-2b4c5d6e7f80");
    private const string GeneratedPasswordHash = "$2a$11$hashed-password";

    [Fact]
    public async Task CreateAsync_CreatesUserWithExpectedDataAndReturnsRepositoryResult()
    {
        var request = new CreateUserRequestDtoBuilder()
            .WithName("Jane Doe")
            .WithEmail("jane.doe@example.com")
            .WithUsername("jane.doe")
            .WithPassword("secret")
            .Generate();
        var repositoryResult = new UserBuilder()
            .WithId(Guid.Parse("018f1f7e-6b5a-7f9b-9b6c-2b4c5d6e7f81"))
            .WithName("Jane Doe Persisted")
            .WithEmail("persisted.jane.doe@example.com")
            .WithUsername("persisted.jane")
            .WithPasswordHash("persisted-hashed-secret")
            .WithCreatedAt(new DateTimeOffset(2026, 5, 9, 17, 30, 0, TimeSpan.Zero))
            .Generate();
        var (service, userRepository) = CreateService(repositoryResult);

        var response = await service.CreateAsync(request);

        await userRepository.Received(1).CreateAsync(
            Arg.Is<User>(user =>
                user.Id == GeneratedUserId &&
                user.Name == request.Name &&
                user.Email == request.Email &&
                user.Username == request.Username &&
                user.PasswordHash == GeneratedPasswordHash &&
                user.CreatedAt > DateTimeOffset.MinValue),
            Arg.Any<CancellationToken>());

        response.ShouldBe(new UserResponseDto(
            repositoryResult.Id,
            repositoryResult.Name,
            repositoryResult.Email,
            repositoryResult.Username,
            repositoryResult.CreatedAt));
    }

    private static (UserService Service, IUserRepository UserRepository) CreateService(User repositoryResult)
    {
        var idGenerator = Substitute.For<IIdGenerator>();
        var passwordHasher = Substitute.For<IPasswordHasher>();
        var userRepository = Substitute.For<IUserRepository>();

        idGenerator.NewId().Returns(GeneratedUserId);
        passwordHasher.Hash(Arg.Any<string>()).Returns(GeneratedPasswordHash);
        userRepository.CreateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(repositoryResult));

        return (new UserService(idGenerator, passwordHasher, userRepository), userRepository);
    }
}
