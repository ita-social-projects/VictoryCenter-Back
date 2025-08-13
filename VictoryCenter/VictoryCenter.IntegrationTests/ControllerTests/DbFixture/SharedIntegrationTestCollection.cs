using VictoryCenter.IntegrationTests.ControllerTests.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.Base;

[CollectionDefinition("SharedIntegrationTests")]
public class SharedIntegrationTestCollection : ICollectionFixture<IntegrationTestDbFixture>
{
}
