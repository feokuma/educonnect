using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using EduConnect.Application.DTOs;
using EduConnect.Infrastructure.Persistence;
using EduConnect.Integration.Setup;
using EduConnect.Tests.Common.Builders.Application.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace EduConnect.Integration.Controllers;

[Collection(nameof(IntegrationTestCollection))]
public class UsersControllerTests(IntegrationWebAppFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Post_Users_WithValidBearerToken_Returns201AndCreatedUserPayload()
    {
        await AuthenticateClientAsync();
        var request = new CreateUserRequestDtoBuilder()
            .WithName("Jane Doe")
            .WithEmail("jane.doe@example.com")
            .WithUsername("jane.doe")
            .WithPassword("secret")
            .Generate();

        var response = await _client.PostAsJsonAsync("/users", request);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        response.Headers.Location.ShouldNotBeNull();
        response.Headers.Location!.OriginalString.ShouldStartWith("/users/");

        var body = await response.Content.ReadFromJsonAsync<UserResponseDto>();

        body.ShouldNotBeNull();
        body!.Id.ShouldNotBe(Guid.Empty);
        body.Name.ShouldBe(request.Name);
        body.Email.ShouldBe(request.Email);
        body.Username.ShouldBe(request.Username);
        body.CreatedAt.ShouldBeGreaterThan(DateTimeOffset.MinValue);

        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EduConnectDbContext>();
        var persistedUser = await dbContext.Users.SingleAsync(user => user.Id == body.Id);

        persistedUser.PasswordHash.ShouldNotBe(request.Password);
        BCrypt.Net.BCrypt.Verify(request.Password, persistedUser.PasswordHash).ShouldBeTrue();
    }

    [Fact]
    public async Task Post_Users_WithoutBearerToken_Returns401()
    {
        _client.DefaultRequestHeaders.Authorization = null;
        var request = new CreateUserRequestDtoBuilder()
            .WithName("Unauthorized Jane")
            .WithEmail("unauthorized.jane@example.com")
            .WithUsername("unauthorized.jane")
            .WithPassword("secret")
            .Generate();

        var response = await _client.PostAsJsonAsync("/users", request);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Post_Users_WithInvalidBearerToken_Returns401()
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "invalid-token");
        var request = new CreateUserRequestDtoBuilder()
            .WithName("Invalid Token Jane")
            .WithEmail("invalid.token.jane@example.com")
            .WithUsername("invalid.token.jane")
            .WithPassword("secret")
            .Generate();

        var response = await _client.PostAsJsonAsync("/users", request);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Get_UsersMe_WithValidBearerToken_ReturnsCurrentUser()
    {
        var createUserRequest = new CreateUserRequestDtoBuilder()
            .WithName("Current User Jane")
            .WithEmail("current.user.jane@example.com")
            .WithUsername("current.user.jane")
            .WithPassword("secret123")
            .Generate();
        await factory.SeedUserAsync(createUserRequest);
        var loginRequest = new LoginRequestDto(createUserRequest.Email, createUserRequest.Password);
        var loginResponse = await _client.PostAsJsonAsync("/auth/login", loginRequest);
        var loginBody = await loginResponse.Content.ReadFromJsonAsync<AuthResponseDto>();

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginBody!.AccessToken);

        var response = await _client.GetAsync("/users/me");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<CurrentUserResponseDto>();

        body.ShouldNotBeNull();
        body!.Id.ShouldNotBeNullOrWhiteSpace();
        body.Email.ShouldBe(createUserRequest.Email);
        body.TokenExpiresAt.ShouldNotBeNull();
        body.TokenExpiresAt!.Value.ShouldBeGreaterThan(DateTimeOffset.UtcNow);
    }

    [Fact]
    public async Task Get_UsersMe_WithoutBearerToken_Returns401()
    {
        _client.DefaultRequestHeaders.Authorization = null;

        var response = await _client.GetAsync("/users/me");

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    private async Task AuthenticateClientAsync()
    {
        var adminRequest = new CreateUserRequestDtoBuilder()
            .WithName("Admin Jane")
            .WithEmail("admin.jane@example.com")
            .WithUsername("admin.jane")
            .WithPassword("secret123")
            .Generate();

        await factory.SeedUserAsync(adminRequest);

        var loginResponse = await _client.PostAsJsonAsync(
            "/auth/login",
            new LoginRequestDto(adminRequest.Email, adminRequest.Password));
        var loginBody = await loginResponse.Content.ReadFromJsonAsync<AuthResponseDto>();

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            loginBody!.AccessToken);
    }
}
