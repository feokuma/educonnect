using EduConnect.Application.DTOs;
using EduConnect.Application.Common;
using EduConnect.Application.Repositories;
using EduConnect.Infrastructure.Authentication;
using EduConnect.Tests.Common.Builders.Domain.Users;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace EduConnect.Unit.Infrastructure.Authentication;

public class AuthServiceTests
{
    private const string Password = "secret123";
    private const string PasswordHash = "$2a$11$hashed-password";

    [Fact]
    public async Task Authenticate_WithValidEmailAndPassword_ReturnsTokensAndExpiration()
    {
        var service = CreateService();

        var response = await service.AuthenticateAsync(new LoginRequestDto("Jane.Doe@Example.com", Password));

        Assert.NotNull(response);
        Assert.False(string.IsNullOrWhiteSpace(response!.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(response.RefreshToken));
        Assert.True(response.ExpiresAt > DateTimeOffset.UtcNow);
        Assert.Equal(3, response.AccessToken.Split('.').Length);
    }

    [Fact]
    public async Task Authenticate_WithValidUsernameAndPassword_ReturnsTokensAndExpiration()
    {
        var service = CreateService();

        var response = await service.AuthenticateAsync(new LoginRequestDto("jane.doe", Password));

        Assert.NotNull(response);
        Assert.False(string.IsNullOrWhiteSpace(response!.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(response.RefreshToken));
        Assert.True(response.ExpiresAt > DateTimeOffset.UtcNow);
        Assert.Equal(3, response.AccessToken.Split('.').Length);
    }

    [Theory]
    [InlineData("", Password)]
    [InlineData(" ", Password)]
    [InlineData("jane.doe@example.com", "")]
    [InlineData("jane.doe@example.com", " ")]
    public async Task Authenticate_WithBlankIdentifierOrPassword_ReturnsNull(string identifier, string password)
    {
        var service = CreateService();

        var response = await service.AuthenticateAsync(new LoginRequestDto(identifier, password));

        Assert.Null(response);
    }

    [Fact]
    public async Task Authenticate_WithUnknownIdentifier_ReturnsNull()
    {
        var service = CreateService(userExists: false);

        var response = await service.AuthenticateAsync(new LoginRequestDto("unknown", Password));

        Assert.Null(response);
    }

    [Fact]
    public async Task Authenticate_CallsUserRepositoryWithExpectedIdentifierAndCancellationToken()
    {
        var (service, _, userRepository) = CreateServiceWithDependencies();
        using var cancellationTokenSource = new CancellationTokenSource();
        const string identifier = "Jane.Doe@Example.com";

        await service.AuthenticateAsync(new LoginRequestDto(identifier, Password), cancellationTokenSource.Token);

        await userRepository
            .Received(1)
            .GetByEmailOrUsernameAsync(identifier, cancellationTokenSource.Token);
    }

    [Fact]
    public async Task Authenticate_CallsPasswordHasherVerifyWithExpectedPasswordAndHash()
    {
        var (service, passwordHasher, _) = CreateServiceWithDependencies();

        await service.AuthenticateAsync(new LoginRequestDto("jane.doe@example.com", Password));

        passwordHasher.Received(1).Verify(Password, PasswordHash);
    }

    [Fact]
    public async Task Authenticate_WithWrongPassword_ReturnsNull()
    {
        var service = CreateService(passwordMatches: false);

        var response = await service.AuthenticateAsync(new LoginRequestDto("jane.doe@example.com", "wrong-secret"));

        Assert.Null(response);
    }

    [Fact]
    public async Task Refresh_WithValidRefreshToken_ReturnsNewTokens()
    {
        var service = CreateService();
        var auth = await service.AuthenticateAsync(new LoginRequestDto("jane.doe@example.com", Password));

        var refreshed = service.Refresh(new RefreshTokenRequestDto(auth!.RefreshToken));

        Assert.NotNull(refreshed);
        Assert.False(string.IsNullOrWhiteSpace(refreshed!.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(refreshed.RefreshToken));
        Assert.NotEqual(auth.RefreshToken, refreshed.RefreshToken);
    }

    [Fact]
    public void Refresh_WithUnknownRefreshToken_ReturnsNull()
    {
        var service = CreateService();

        var refreshed = service.Refresh(new RefreshTokenRequestDto("unknown-refresh-token"));

        Assert.Null(refreshed);
    }

    [Fact]
    public async Task Refresh_WithSameRefreshTokenTwice_ReturnsNullOnSecondAttempt()
    {
        var service = CreateService();
        var auth = await service.AuthenticateAsync(new LoginRequestDto("jane.doe@example.com", Password));

        var firstRefresh = service.Refresh(new RefreshTokenRequestDto(auth!.RefreshToken));
        var secondRefresh = service.Refresh(new RefreshTokenRequestDto(auth.RefreshToken));

        Assert.NotNull(firstRefresh);
        Assert.Null(secondRefresh);
    }

    [Fact]
    public async Task Refresh_WithExpiredRefreshToken_ReturnsNull()
    {
        var service = CreateService(new AuthSettings { RefreshTokenExpirationDays = -1 });
        var auth = await service.AuthenticateAsync(new LoginRequestDto("jane.doe@example.com", Password));

        var refreshed = service.Refresh(new RefreshTokenRequestDto(auth!.RefreshToken));

        Assert.Null(refreshed);
    }

    private static AuthService CreateService(
        AuthSettings? settings = null,
        bool userExists = true,
        bool passwordMatches = true)
    {
        return CreateServiceWithDependencies(settings, userExists, passwordMatches).Service;
    }

    private static (
        AuthService Service,
        IPasswordHasher PasswordHasher,
        IUserRepository UserRepository) CreateServiceWithDependencies(
            AuthSettings? settings = null,
            bool userExists = true,
            bool passwordMatches = true)
    {
        var passwordHasher = Substitute.For<IPasswordHasher>();
        var userRepository = Substitute.For<IUserRepository>();
        var user = new UserBuilder()
            .WithEmail("jane.doe@example.com")
            .WithUsername("jane.doe")
            .WithPasswordHash(PasswordHash)
            .Generate();

        userRepository
            .GetByEmailOrUsernameAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(userExists ? user : null);
        passwordHasher.Verify(Arg.Any<string>(), PasswordHash).Returns(passwordMatches);

        return (
            new AuthService(Options.Create(settings ?? new AuthSettings()), passwordHasher, userRepository),
            passwordHasher,
            userRepository);
    }
}
