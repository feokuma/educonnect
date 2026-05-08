using educonnect.integration.Setup;

namespace educonnect.integration.Controllers;

[Collection(nameof(IntegrationTestCollection))]
public class HealthControllerTests(IntegrationWebAppFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Get_Health_Returns200()
    {
        var response = await _client.GetAsync("/health");

        response.EnsureSuccessStatusCode();
    }
}
