using System.Net;
using System.Net.Http.Json;
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

        var body = await response.Content.ReadFromJsonAsync<CreatedUserResponse>();

        body.ShouldNotBeNull();
        Guid.TryParse(body!.Id, out _).ShouldBeTrue();
        body.Name.ShouldBe(request.Name);
        body.Email.ShouldBe(request.Email);
        body.CreatedAt.ShouldBeGreaterThan(DateTimeOffset.MinValue);
    }

    private sealed record CreatedUserResponse(string Id, string Name, string Email, DateTimeOffset CreatedAt);
}
