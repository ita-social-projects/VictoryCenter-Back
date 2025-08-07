using dotenv.net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VictoryCenter.DAL.Data;
using VictoryCenter.DbUpdate.Helpers;

try
{
    await MainAsync();
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("Unhandled exception:");
    Console.ResetColor();

    Console.WriteLine(ex.Message);
    Console.WriteLine(ex);

    Environment.ExitCode = 1;
}

async Task MainAsync()
{
    var webApiProjectPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "VictoryCenter.WebApi"));

    DotEnv.Load(new DotEnvOptions(envFilePaths: new[] { Path.Combine(webApiProjectPath, ".env") }));

    var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

    var configuration = new ConfigurationBuilder()
        .SetBasePath(webApiProjectPath)
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{environment}.json", optional: true)
        .AddEnvironmentVariables()
        .Build();

    var rawConnectionString = configuration.GetConnectionString("DefaultConnection");

    if (string.IsNullOrEmpty(rawConnectionString))
    {
        throw new InvalidOperationException("Connection string 'DefaultConnection' is not set.");
    }

    var connectionString = EnvironmentVariablesResolver.GetEnvironmentVariable(rawConnectionString);

    var services = new ServiceCollection();

    services.AddLogging(builder =>
    {
        builder.AddSimpleConsole(options =>
        {
            options.IncludeScopes = true;
            options.SingleLine = true;
            options.TimestampFormat = "HH:mm:ss ";
        });
        builder.SetMinimumLevel(LogLevel.Information);
    });

    services.AddDbContext<VictoryCenterDbContext>(options =>
        options.UseSqlServer(connectionString));

    var serviceProvider = services.BuildServiceProvider();

    using var scope = serviceProvider.CreateScope();

    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Using connection string: {ConnectionString}", connectionString);

    var context = scope.ServiceProvider.GetRequiredService<VictoryCenterDbContext>();

    logger.LogInformation("Starting database migration...");

    await context.Database.MigrateAsync();

    logger.LogInformation("Database migration completed successfully.");
}
