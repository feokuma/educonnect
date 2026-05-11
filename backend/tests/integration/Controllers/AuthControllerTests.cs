using System.Net;
using System.Net.Http.Json;
using EduConnect.Application.DTOs;
using EduConnect.Integration.Setup;
using EduConnect.Tests.Common.Builders.Application.DTOs;

namespace EduConnect.Integration.Controllers;

[Collection(nameof(IntegrationTestCollection))]
public class AuthControllerTests(IntegrationWebAppFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Auth_Flow_ReturnsTokensAndRefreshesAccessToken()
    {
        var createUserRequest = new CreateUserRequestDtoBuilder()
            .WithName("Auth Jane")
            .WithEmail("auth.jane@example.com")
            .WithUsername("auth.jane")
            .WithPassword("secret123")
            .Generate();
        await factory.SeedUserAsync(createUserRequest);
        var loginRequest = new LoginRequestDto(createUserRequest.Email, createUserRequest.Password);

        var loginResponse = await _client.PostAsJsonAsync("/auth/login", loginRequest);

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var loginBody = await loginResponse.Content.ReadFromJsonAsync<AuthResponseDto>();

        Assert.NotNull(loginBody);
        Assert.False(string.IsNullOrWhiteSpace(loginBody!.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(loginBody.RefreshToken));
        Assert.True(loginBody.ExpiresAt > DateTimeOffset.UtcNow);

        var refreshResponse = await _client.PostAsJsonAsync(
            "/auth/refresh",
            new RefreshTokenRequestDto(loginBody.RefreshToken));

        Assert.Equal(HttpStatusCode.OK, refreshResponse.StatusCode);

        var refreshBody = await refreshResponse.Content.ReadFromJsonAsync<AuthResponseDto>();

        Assert.NotNull(refreshBody);
        Assert.False(string.IsNullOrWhiteSpace(refreshBody!.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(refreshBody.RefreshToken));
        Assert.NotEqual(loginBody.RefreshToken, refreshBody.RefreshToken);
    }

    [Fact]
    public async Task Login_WithUsername_ReturnsTokens()
    {
        var createUserRequest = new CreateUserRequestDtoBuilder()
            .WithName("Username Jane")
            .WithEmail("username.jane@example.com")
            .WithUsername("username.jane")
            .WithPassword("secret123")
            .Generate();
        await factory.SeedUserAsync(createUserRequest);
        var loginRequest = new LoginRequestDto(createUserRequest.Username, createUserRequest.Password);

        var loginResponse = await _client.PostAsJsonAsync("/auth/login", loginRequest);

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var loginBody = await loginResponse.Content.ReadFromJsonAsync<AuthResponseDto>();

        Assert.NotNull(loginBody);
        Assert.False(string.IsNullOrWhiteSpace(loginBody!.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(loginBody.RefreshToken));
    }

    [Fact]
    public async Task Login_WithWrongPassword_Returns401()
    {
        var createUserRequest = new CreateUserRequestDtoBuilder()
            .WithName("Wrong Password Jane")
            .WithEmail("wrong.password.jane@example.com")
            .WithUsername("wrong.password.jane")
            .WithPassword("secret123")
            .Generate();
        await factory.SeedUserAsync(createUserRequest);
        var loginRequest = new LoginRequestDto(createUserRequest.Email, "wrong-secret");

        var loginResponse = await _client.PostAsJsonAsync("/auth/login", loginRequest);

        Assert.Equal(HttpStatusCode.Unauthorized, loginResponse.StatusCode);
    }
}
