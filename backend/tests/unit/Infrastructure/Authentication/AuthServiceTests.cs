using EduConnect.Application.DTOs;
using EduConnect.Infrastructure.Authentication;
using Microsoft.Extensions.Options;

namespace EduConnect.Unit.Infrastructure.Authentication;

public class AuthServiceTests
{
    [Fact]
    public void Authenticate_WithValidCredentials_ReturnsTokensAndExpiration()
    {
        var service = CreateService();

        var response = service.Authenticate(new LoginRequestDto("Jane.Doe@Example.com", "secret123"));

        Assert.NotNull(response);
        Assert.False(string.IsNullOrWhiteSpace(response!.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(response.RefreshToken));
        Assert.True(response.ExpiresAt > DateTimeOffset.UtcNow);
        Assert.Equal(3, response.AccessToken.Split('.').Length);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Authenticate_WithInvalidEmail_ReturnsNull(string email)
    {
        var service = CreateService();

        var response = service.Authenticate(new LoginRequestDto(email, "secret123"));

        Assert.Null(response);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Authenticate_WithInvalidPassword_ReturnsNull(string password)
    {
        var service = CreateService();

        var response = service.Authenticate(new LoginRequestDto("jane.doe@example.com", password));

        Assert.Null(response);
    }

    [Fact]
    public void Refresh_WithValidRefreshToken_ReturnsNewTokens()
    {
        var service = CreateService();
        var auth = service.Authenticate(new LoginRequestDto("jane.doe@example.com", "secret123"));

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
    public void Refresh_WithSameRefreshTokenTwice_ReturnsNullOnSecondAttempt()
    {
        var service = CreateService();
        var auth = service.Authenticate(new LoginRequestDto("jane.doe@example.com", "secret123"));

        var firstRefresh = service.Refresh(new RefreshTokenRequestDto(auth!.RefreshToken));
        var secondRefresh = service.Refresh(new RefreshTokenRequestDto(auth.RefreshToken));

        Assert.NotNull(firstRefresh);
        Assert.Null(secondRefresh);
    }

    [Fact]
    public void Refresh_WithExpiredRefreshToken_ReturnsNull()
    {
        var service = CreateService(new AuthSettings { RefreshTokenExpirationDays = -1 });
        var auth = service.Authenticate(new LoginRequestDto("jane.doe@example.com", "secret123"));

        var refreshed = service.Refresh(new RefreshTokenRequestDto(auth!.RefreshToken));

        Assert.Null(refreshed);
    }

    private static AuthService CreateService(AuthSettings? settings = null)
    {
        return new AuthService(Options.Create(settings ?? new AuthSettings()));
    }
}
