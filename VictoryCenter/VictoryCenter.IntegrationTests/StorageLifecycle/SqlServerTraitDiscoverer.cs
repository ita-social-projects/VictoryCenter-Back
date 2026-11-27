using Xunit.Abstractions;
using Xunit.Sdk;

namespace VictoryCenter.IntegrationTests.StorageLifecycle;

public sealed class SqlServerTraitDiscoverer : ITraitDiscoverer
{
    public IEnumerable<KeyValuePair<string, string>> GetTraits(IAttributeInfo traitAttribute)
    {
        yield return new KeyValuePair<string, string>("Category", "SqlServer");
    }
}
