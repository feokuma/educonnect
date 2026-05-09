using EduConnect.Integration.Setup;

namespace EduConnect.Integration.Setup;

[CollectionDefinition(nameof(IntegrationTestCollection))]
public class IntegrationTestCollection : ICollectionFixture<IntegrationWebAppFactory> { }
