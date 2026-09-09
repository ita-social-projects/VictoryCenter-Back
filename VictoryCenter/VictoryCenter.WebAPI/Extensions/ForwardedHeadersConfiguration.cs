using Microsoft.AspNetCore.HttpOverrides;

namespace VictoryCenter.WebAPI.Extensions;

public static class ForwardedHeadersConfiguration
{
    private const string KnownNetworksConfigurationKey = "ReverseProxy:KnownNetworks";

    public static IServiceCollection AddForwardedHeadersConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        string[] knownNetworks = configuration
            .GetSection(KnownNetworksConfigurationKey)
            .Get<string[]>() ?? [];

        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;

            foreach (string knownNetwork in knownNetworks)
            {
                if (!System.Net.IPNetwork.TryParse(knownNetwork, out System.Net.IPNetwork network))
                {
                    throw new InvalidOperationException(
                        $"Invalid CIDR value '{knownNetwork}' in '{KnownNetworksConfigurationKey}'.");
                }

                options.KnownNetworks.Add(new IPNetwork(network.BaseAddress, network.PrefixLength));
            }
        });

        return services;
    }
}
