using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.BLL.Services.BlobStorage;

namespace VictoryCenter.WebAPI.Extensions;

public class ConfigureHostBuilderExtensions
{
    public static void ConfigureBlob(IServiceCollection services, WebApplicationBuilder builder)
    {
        services.Configure<BlobEnvironmentVariables>(builder.Configuration.GetSection("BlobEnvironmentVariables"));

        builder.Services.AddScoped<IBlobService, BlobService>();
    }
}
