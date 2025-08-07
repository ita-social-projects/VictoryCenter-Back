using dotenv.net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VictoryCenter.DbUpdate;

try
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

    var serviceProvider = services.BuildServiceProvider();

    using var scope = serviceProvider.CreateScope();

    var logger = scope.ServiceProvider.GetRequiredService<ILogger<DatabaseMigrator>>();

    var migrator = new DatabaseMigrator(configuration, services, logger);

    await migrator.MigrateAsync();
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
