using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VictoryCenter.DAL.Data;
using VictoryCenter.DbUpdate.Helpers;

namespace VictoryCenter.DbUpdate;

public class DatabaseMigrator
{
    private readonly IConfiguration _configuration;
    private readonly IServiceCollection _services;
    private readonly ILogger<DatabaseMigrator> _logger;

    public DatabaseMigrator(IConfiguration configuration, IServiceCollection services, ILogger<DatabaseMigrator> logger)
    {
        _configuration = configuration;
        _services = services;
        _logger = logger;
    }

    public async Task MigrateAsync()
    {
        var rawConnectionString = _configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(rawConnectionString))
        {
            throw new InvalidOperationException("Connection string 'DefaultConnection' is not set.");
        }

        var connectionString = EnvironmentVariablesResolver.GetEnvironmentVariable(rawConnectionString);

        _logger.LogInformation("Using connection string: {ConnectionString}", connectionString);

        _services.AddDbContext<VictoryCenterDbContext>(options =>
            options.UseSqlServer(connectionString));

        var provider = _services.BuildServiceProvider();
        using var scope = provider.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<VictoryCenterDbContext>();

        _logger.LogInformation("Starting database migration...");
        await context.Database.MigrateAsync();
        _logger.LogInformation("Database migration completed successfully.");
    }
}
