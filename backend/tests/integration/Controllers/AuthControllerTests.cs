using System.Net;
using System.Net.Http.Json;
using EduConnect.Application.DTOs;
using EduConnect.Integration.Setup;

namespace EduConnect.Integration.Controllers;

[Collection(nameof(IntegrationTestCollection))]
public class AuthControllerTests(IntegrationWebAppFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Auth_Flow_ReturnsTokensAndValidatesAccessToken()
    {
        var loginRequest = new LoginRequestDto("jane.doe@example.com", "secret123");

        var loginResponse = await _client.PostAsJsonAsync("/auth/login", loginRequest);

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var loginBody = await loginResponse.Content.ReadFromJsonAsync<AuthResponseDto>();

        Assert.NotNull(loginBody);
        Assert.False(string.IsNullOrWhiteSpace(loginBody!.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(loginBody.RefreshToken));
        Assert.True(loginBody.ExpiresAt > DateTimeOffset.UtcNow);

        var validateResponse = await _client.PostAsJsonAsync(
            "/auth/validate",
            new ValidateTokenRequestDto(loginBody.AccessToken));

        Assert.Equal(HttpStatusCode.OK, validateResponse.StatusCode);

        var validationBody = await validateResponse.Content.ReadFromJsonAsync<TokenValidationResponseDto>();

        Assert.NotNull(validationBody);
        Assert.True(validationBody!.IsValid);
        Assert.Equal(loginRequest.Email, validationBody.Email);

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
}
