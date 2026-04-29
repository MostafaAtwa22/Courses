using Xunit;

namespace API.Tests
{
    [CollectionDefinition("Integration Tests")]
    public class SharedTestCollection : ICollectionFixture<IntegrationTestFactory<Program>>
    {
    }
}
