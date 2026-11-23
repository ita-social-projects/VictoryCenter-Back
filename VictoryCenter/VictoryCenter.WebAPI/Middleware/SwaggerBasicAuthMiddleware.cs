using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Identity;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.WebAPI.Middleware;

public class SwaggerBasicAuthMiddleware(
    RequestDelegate next,
    ILogger<SwaggerBasicAuthMiddleware> logger)
{
    private static readonly AdminUser DummyAdmin = new();
    private static readonly PasswordHasher<AdminUser> DummyPasswordHasher = new();
    private static readonly string DummyPasswordHash = DummyPasswordHasher.HashPassword(
        DummyAdmin,
        Guid.NewGuid().ToString());

    public async Task InvokeAsync(
        HttpContext context,
        UserManager<AdminUser> userManager)
    {
        if (!context.Request.Path.StartsWithSegments("/swagger"))
        {
            await next(context);
            return;
        }

        if (!context.Request.IsHttps)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return;
        }

        var authorization = context.Request.Headers.Authorization.ToString();

        if (AuthenticationHeaderValue.TryParse(authorization, out var header) &&
            header.Scheme.Equals("Basic", StringComparison.OrdinalIgnoreCase) &&
            !string.IsNullOrEmpty(header.Parameter))
        {
            try
            {
                var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(header.Parameter));

                var separatorIndex = credentials.IndexOf(':');

                if (separatorIndex >= 0)
                {
                    var email = credentials[..separatorIndex];
                    var password = credentials[(separatorIndex + 1)..];

                    var admin = await userManager.FindByEmailAsync(email);

                    if (admin is null)
                    {
                        PerformDummyPasswordCheck(password);
                    }
                    else if (await userManager.IsLockedOutAsync(admin))
                    {
                        PerformDummyPasswordCheck(password);
                        LogLockedOutAttempt(admin.Id, context);
                    }
                    else if (await userManager.CheckPasswordAsync(admin, password))
                    {
                        var resetResult = await userManager.ResetAccessFailedCountAsync(admin);
                        if (resetResult.Succeeded)
                        {
                            await next(context);
                            return;
                        }

                        LogIdentityFailure("Failed access count was not reset", admin.Id, resetResult);
                    }
                    else
                    {
                        var accessFailedResult = await userManager.AccessFailedAsync(admin);
                        if (!accessFailedResult.Succeeded)
                        {
                            LogIdentityFailure("Failed Swagger access attempt was not recorded", admin.Id, accessFailedResult);
                        }
                    }
                }
            }
            catch (FormatException ex)
            {
                logger.LogWarning(ex, "Swagger authentication failed: invalid Base64 credentials.");
            }
        }

        logger.LogWarning(
            "Swagger authentication failed from client IP {ClientIpAddress}",
            context.Connection.RemoteIpAddress);

        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.Headers.WWWAuthenticate =
            "Basic realm=\"Swagger Documentation\"";
    }

    private static void PerformDummyPasswordCheck(string password)
    {
        _ = DummyPasswordHasher.VerifyHashedPassword(
            DummyAdmin,
            DummyPasswordHash,
            password);
    }

    private void LogLockedOutAttempt(int adminId, HttpContext context)
    {
        logger.LogWarning(
            "Swagger access rejected for locked out account {AdminId} from client IP {ClientIpAddress}",
            adminId,
            context.Connection.RemoteIpAddress);
    }

    private void LogIdentityFailure(string message, int adminId, IdentityResult identityResult)
    {
        logger.LogError(
            "{Message} for account {AdminId}: {Errors}",
            message,
            adminId,
            string.Join(", ", identityResult.Errors.Select(e => e.Description)));
    }
}
