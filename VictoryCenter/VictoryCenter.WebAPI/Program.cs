using dotenv.net;
using Microsoft.AspNetCore.CookiePolicy;
using VictoryCenter.WebAPI.Extensions;

DotEnv.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Configuration["ConnectionStrings:DefaultConnection"] = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
                                                               ?? throw new InvalidOperationException("DB_CONNECTION_STRING is not set in configuration");
builder.Configuration["JwtOptions:SecretKey"] = Environment.GetEnvironmentVariable("JWTOPTIONS_SECRETKEY")
                                                ?? throw new InvalidOperationException("JWTOPTIONS_SECRETKEY is not set in configuration");

builder.Configuration["JwtOptions:RefreshTokenSecretKey"] = Environment.GetEnvironmentVariable("JWTOPTIONS_REFRESH_TOKEN_SECRETKEY")
                                                            ?? throw new InvalidOperationException("JWTOPTIONS_REFRESH_TOKEN_SECRETKEY is not set in configuration");

builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddCustomServices();
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.Strict;
    options.Secure = CookieSecurePolicy.Always;
    options.HttpOnly = HttpOnlyPolicy.Always;
});

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
