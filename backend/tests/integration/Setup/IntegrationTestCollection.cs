using educonnect.integration.Setup;

namespace educonnect.integration.Setup;

[CollectionDefinition(nameof(IntegrationTestCollection))]
public class IntegrationTestCollection : ICollectionFixture<IntegrationWebAppFactory> { }
