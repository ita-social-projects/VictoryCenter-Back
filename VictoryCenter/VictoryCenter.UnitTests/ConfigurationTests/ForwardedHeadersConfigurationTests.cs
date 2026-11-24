using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using VictoryCenter.WebAPI.Extensions;

namespace VictoryCenter.UnitTests.ConfigurationTests;

public class ForwardedHeadersConfigurationTests
{
    [Theory]
    [InlineData("172.16.0.0/12", "172.16.8.1", "192.168.1.1")]
    [InlineData("2001:db8::/32", "2001:db8::1", "2001:db9::1")]
    public void ConfiguredNetwork_ShouldTrustOnlyAddressesInItsRange(
        string cidr, string trustedAddress, string untrustedAddress)
    {
        using var provider = CreateProvider(cidr);
        var options = provider.GetRequiredService<IOptions<ForwardedHeadersOptions>>().Value;

        Assert.Equal(ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto, options.ForwardedHeaders);
        Assert.Contains(options.KnownIPNetworks, network => network.Contains(IPAddress.Parse(trustedAddress)));
        Assert.DoesNotContain(options.KnownIPNetworks, network => network.Contains(IPAddress.Parse(untrustedAddress)));
    }

    [Theory]
    [InlineData("not-a-network")]
    [InlineData("172.16.0.0/33")]
    public void InvalidNetwork_ShouldFailConfiguration(string cidr)
    {
        using var provider = CreateProvider(cidr);

        Assert.Throws<InvalidOperationException>(
            () => provider.GetRequiredService<IOptions<ForwardedHeadersOptions>>().Value);
    }

    [Fact]
    public void MissingNetworkConfiguration_ShouldRetainRestrictedDefaults()
    {
        using var provider = CreateProvider(null);
        var options = provider.GetRequiredService<IOptions<ForwardedHeadersOptions>>().Value;

        Assert.NotEmpty(options.KnownIPNetworks);
        Assert.DoesNotContain(options.KnownIPNetworks, network => network.Contains(IPAddress.Parse("203.0.113.1")));
        Assert.NotNull(options.ForwardLimit);
    }

    private static ServiceProvider CreateProvider(string? cidr)
    {
        var values = new Dictionary<string, string?>();
        if (cidr is not null)
        {
            values["ReverseProxy:KnownNetworks:0"] = cidr;
        }

        var configuration = new ConfigurationBuilder().AddInMemoryCollection(values).Build();
        var services = new ServiceCollection();
        services.AddForwardedHeadersConfiguration(configuration);

        return services.BuildServiceProvider();
    }
}
