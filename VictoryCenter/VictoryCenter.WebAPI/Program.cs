using dotenv.net;
using VictoryCenter.WebAPI.Extensions;

DotEnv.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureApplication(builder);

builder.Configuration["ConnectionStrings:DefaultConnection"] = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
                                                               ?? throw new InvalidOperationException("DB_CONNECTION_STRING is not set in configuration");
builder.Configuration["JwtOptions:SecretKey"] = Environment.GetEnvironmentVariable("JWTOPTIONS_SECRETKEY")
                                                ?? throw new InvalidOperationException("JWTOPTIONS_SECRETKEY is not set in configuration");

builder.Configuration["JwtOptions:RefreshTokenSecretKey"] = Environment.GetEnvironmentVariable("JWTOPTIONS_REFRESH_TOKEN_SECRETKEY")
                                                            ?? throw new InvalidOperationException("JWTOPTIONS_REFRESH_TOKEN_SECRETKEY is not set in configuration");

builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddCustomServices();

builder.Services.AddOpenTelemetryTracing();
builder.Logging.AddOpenTelemetryLogging();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

await app.ApplyMigrations();
await app.CreateInitialAdmin();

app.UseRequestResponseLogging();
app.UseCors();
app.MapControllers();
app.UseHttpsRedirection();
app.UseCookiePolicy();
app.UseAuthentication();
app.UseAuthorization();

app.Run();

public partial class Program
{
}
