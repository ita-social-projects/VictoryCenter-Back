using Microsoft.Extensions.Options;
using VictoryCenter.BLL.Services.BlobStorage;
using VictoryCenter.BLL.Services.PdfStorage;
using VictoryCenter.WebAPI.Middleware;

namespace VictoryCenter.WebAPI.Extensions;

public static class ApplicationConfiguration
{
    public static void InitializeStorage(this IApplicationBuilder app)
    {
        Directory.CreateDirectory(app.ApplicationServices.GetRequiredService<IOptions<BlobEnvironmentVariables>>().Value.FullPath);
        Directory.CreateDirectory(app.ApplicationServices.GetRequiredService<IOptions<PdfEnvironmentVariables>>().Value.FullPath);
    }

    public static void UseExceptionHandling(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();
    }

    public static void UseRequestResponseLogging(this IApplicationBuilder app)
    {
        app.UseMiddleware<RequestResponseLoggingMiddleware>();
    }
}
