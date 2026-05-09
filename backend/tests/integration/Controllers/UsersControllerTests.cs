using System.Net;
using System.Net.Http.Json;
using EduConnect.Application.DTOs;
using EduConnect.Integration.Setup;

namespace EduConnect.Integration.Controllers;

[Collection(nameof(IntegrationTestCollection))]
public class UsersControllerTests(IntegrationWebAppFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Post_Users_Returns201AndCreatedUserPayload()
    {
        var request = new CreateUserRequestDto("Jane Doe", "jane.doe@example.com");

        var response = await _client.PostAsJsonAsync("/users", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        Assert.StartsWith("/users/", response.Headers.Location!.OriginalString);

        var body = await response.Content.ReadFromJsonAsync<CreatedUserResponse>();

        Assert.NotNull(body);
        Assert.True(Guid.TryParse(body!.Id, out _));
        Assert.Equal(request.Name, body.Name);
        Assert.Equal(request.Email, body.Email);
        Assert.True(body.CreatedAt > DateTimeOffset.MinValue);
    }

    private sealed record CreatedUserResponse(string Id, string Name, string Email, DateTimeOffset CreatedAt);
}
