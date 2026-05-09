using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using EduConnect.Application.DTOs;
using EduConnect.Integration.Setup;
using EduConnect.Tests.Common.Builders.Application.DTOs;
using Shouldly;

namespace EduConnect.Integration.Controllers;

[Collection(nameof(IntegrationTestCollection))]
public class UsersControllerTests(IntegrationWebAppFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Post_Users_Returns201AndCreatedUserPayload()
    {
        var request = new CreateUserRequestDtoBuilder()
            .WithName("Jane Doe")
            .WithEmail("jane.doe@example.com")
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
        body.CreatedAt.ShouldBeGreaterThan(DateTimeOffset.MinValue);
    }

    [Fact]
    public async Task Get_UsersMe_WithValidBearerToken_ReturnsCurrentUser()
    {
        var loginRequest = new LoginRequestDto("jane.doe@example.com", "secret123");
        var loginResponse = await _client.PostAsJsonAsync("/auth/login", loginRequest);
        var loginBody = await loginResponse.Content.ReadFromJsonAsync<AuthResponseDto>();

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginBody!.AccessToken);

        var response = await _client.GetAsync("/users/me");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<CurrentUserResponseDto>();

        body.ShouldNotBeNull();
        body!.Id.ShouldNotBeNullOrWhiteSpace();
        body.Email.ShouldBe(loginRequest.Email);
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
}
