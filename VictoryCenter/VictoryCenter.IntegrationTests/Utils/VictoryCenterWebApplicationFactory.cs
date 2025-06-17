using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VictoryCenter.DAL.Data;

namespace VictoryCenter.IntegrationTests.Utils;

public class VictoryCenterWebApplicationFactory<T> : WebApplicationFactory<T>
    where T : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.Sources.Clear();
            config.AddJsonFile("appsettings.IntegrationTests.json", optional: false);
            config.AddEnvironmentVariables();
        });

        builder.ConfigureServices(services =>
        {
            RemoveExistingContext(services);
            AddTestDbContext(services);

            var provider = services.BuildServiceProvider();
            using var scope = provider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<VictoryCenterDbContext>();
            dbContext.Database.EnsureCreated();

            TestDatabaseSeeder.DeleteExistingData(dbContext);
            TestDatabaseSeeder.SeedData(dbContext);
            VictoryCenterDatabaseSeeder.DeleteExistingData(dbContext);
            VictoryCenterDatabaseSeeder.SeedData(dbContext);
        });
    }

    private static void RemoveExistingContext(IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(
            d => d.ServiceType == typeof(DbContextOptions<VictoryCenterDbContext>));
        if (descriptor != null)
        {
            services.Remove(descriptor);
        }
    }

    private static void AddTestDbContext(IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();

        services.AddDbContext<VictoryCenterDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            options.UseSqlServer(connectionString);
        });
    }
}
