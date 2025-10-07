using System.Reflection;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using VictoryCenter.BLL;
using VictoryCenter.BLL.Commands.Public.Payment.Common;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Helpers;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.BLL.Interfaces.PaymentService;
using VictoryCenter.BLL.Interfaces.Search;
using VictoryCenter.BLL.Interfaces.TokenService;
using VictoryCenter.BLL.Options;
using VictoryCenter.BLL.Options.Payment;
using VictoryCenter.BLL.Services.BlobStorage;
using VictoryCenter.BLL.Services.PaymentService;
using VictoryCenter.BLL.Services.Search;
using VictoryCenter.BLL.Services.TokenService;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Realizations.Base;
using VictoryCenter.WebAPI.Factories;
using VictoryCenter.WebAPI.Utils;

namespace VictoryCenter.WebAPI.Extensions;

public static class ServicesConfiguration
{
    public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<VictoryCenterDbContext>(options =>
        {
            options.UseSqlServer(connectionString, opt =>
            {
                opt.MigrationsAssembly(typeof(VictoryCenterDbContext).Assembly.GetName().Name);
                opt.MigrationsHistoryTable("__EFMigrationsHistory", schema: "entity_framework");
            });
        });

        services.AddIdentity<AdminUser, IdentityRole<int>>()
            .AddEntityFrameworkStores<VictoryCenterDbContext>()
            .AddDefaultTokenProviders();

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options => { options.TokenValidationParameters = AuthHelper.GetTokenValidationParameters(configuration); });

        services.Configure<CookiePolicyOptions>(options =>
        {
            options.MinimumSameSitePolicy = SameSiteMode.Strict;
            options.Secure = CookieSecurePolicy.Always;
            options.HttpOnly = HttpOnlyPolicy.Always;
        });

        var corsSettings = SettingsExtractor.GetCorsSettings(configuration);
        services.AddCors(opt =>
        {
            opt.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins(corsSettings.AllowedOrigins)
                    .WithHeaders(corsSettings.AllowedHeaders)
                    .WithMethods(corsSettings.AllowedMethods)
                    .WithExposedHeaders(corsSettings.ExposedHeaders)
                    .AllowCredentials()
                    .SetPreflightMaxAge(TimeSpan.FromSeconds(corsSettings.PreflightMaxAge));
            });
        });

        ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;
    }

    public static void AddCustomServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddOpenApi();
        services.AddAutoMapper(typeof(BllAssemblyMarker).Assembly);

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(BllAssemblyMarker).Assembly));

        services.AddValidatorsFromAssemblyContaining<BllAssemblyMarker>();

        ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;
        ValidatorOptions.Global.DefaultClassLevelCascadeMode = CascadeMode.Continue;

        services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();
        services.AddSingleton<ProblemDetailsFactory, CustomProblemDetailsFactory>();
        services.ConfigureBlob(configuration);

        services.AddOptions<JwtOptions>()
            .BindConfiguration(JwtOptions.Position)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<WayForPayOptions>()
            .BindConfiguration(WayForPayOptions.Position)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddHttpClient("Way4PayClient")
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                AllowAutoRedirect = false
            });

        services.AddSingleton<ITokenService, TokenService>();

        services.AddScoped<IPaymentService, PaymentService>();

        services.AddScoped(typeof(ISearchService<>), typeof(SearchService<>));

        services.ScanInterfacesAndRegisterImplementations(typeof(BllAssemblyMarker).Assembly, typeof(IPaymentFactory), ServiceLifetime.Scoped);
        services.ScanInterfacesAndRegisterImplementations(typeof(BllAssemblyMarker).Assembly, typeof(IPaymentCommandHandler<,>), ServiceLifetime.Scoped);
    }

    public static void MapOpenApi(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "VictoryCenter API V1");
            c.RoutePrefix = "swagger";
        });
    }

    public static async Task ApplyMigrationsAsync(this WebApplication app)
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        try
        {
            using IServiceScope localScope = app.Services.CreateScope();
            var victoryCenterDbContext = localScope.ServiceProvider.GetRequiredService<VictoryCenterDbContext>();
            var pendingMigrations = await victoryCenterDbContext.Database.GetPendingMigrationsAsync();
            var migrations = pendingMigrations.ToList();

            if (migrations.Any())
            {
                logger.LogInformation("Pending migrations: {PendingMigrations}", string.Join(", ", migrations));
                var appliedMigrationsBefore = await victoryCenterDbContext.Database.GetAppliedMigrationsAsync();

                await victoryCenterDbContext.Database.MigrateAsync();

                logger.LogInformation("Migrations applied successfully.");
                var appliedMigrationsAfter = await victoryCenterDbContext.Database.GetAppliedMigrationsAsync();
                var newlyAppliedMigrations = appliedMigrationsAfter.Except(appliedMigrationsBefore);
                var appliedMigrations = newlyAppliedMigrations.ToList();

                if (appliedMigrations.Any())
                {
                    logger.LogInformation("Newly applied migrations: {NewlyAppliedMigrations}", string.Join(", ", appliedMigrations));
                }
                else
                {
                    logger.LogInformation("No new migrations were applied.");
                }
            }
            else
            {
                logger.LogInformation("No pending migrations.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred during startup migration");
        }
    }

    public static async Task CreateInitialDataAsync(this WebApplication app)
    {
        await app.CreateInitialAdminAsync();
        await app.CreateInitialCategoriesAsync();
        await app.SeedVisitorPagesAsync();
    }

    public static async Task SeedVisitorPagesAsync(this WebApplication app)
    {
        await using var asyncServiceScope = app.Services.CreateAsyncScope();
        var dbContext = asyncServiceScope.ServiceProvider.GetRequiredService<VictoryCenterDbContext>();

        var existing = await dbContext.VisitorPages.ToListAsync();
        var existingBySlug = existing.ToDictionary(p => p.Slug, StringComparer.OrdinalIgnoreCase);

        var toAdd = new List<VisitorPage>();

        foreach (var page in PageConstants.VisitorPages)
        {
            if (existingBySlug.TryGetValue(page.Slug, out VisitorPage? found))
            {
                found.Title = page.Title;
            }
            else
            {
                toAdd.Add(new() { Slug = page.Slug, Title = page.Title, CreatedAt = DateTime.UtcNow });
            }
        }

        dbContext.VisitorPages.AddRange(toAdd);
        await dbContext.SaveChangesAsync();
    }

    private static async Task CreateInitialAdminAsync(this WebApplication app)
    {
        await using var asyncServiceScope = app.Services.CreateAsyncScope();
        var userManager = asyncServiceScope.ServiceProvider.GetRequiredService<UserManager<AdminUser>>();
        var initialAdminEmail = Environment.GetEnvironmentVariable("INITIAL_ADMIN_EMAIL")
                                ?? throw new InvalidOperationException("INITIAL_ADMIN_EMAIL environment variable is required");
        if (!initialAdminEmail.Contains('@'))
        {
            throw new InvalidOperationException("INITIAL_ADMIN_EMAIL must be a valid email address");
        }

        if (await userManager.FindByEmailAsync(initialAdminEmail) is null)
        {
            var tokenService = asyncServiceScope.ServiceProvider.GetRequiredService<ITokenService>();
            var admin = new AdminUser()
            {
                UserName = initialAdminEmail,
                Email = initialAdminEmail,
                CreatedAt = DateTimeOffset.UtcNow,
                RefreshTokenValidTo = DateTimeOffset.UtcNow.AddDays(30),

                // just for initial admin during development, in future create separate endpoint/tool for creating admins with proper token operations
                RefreshToken = tokenService.CreateRefreshToken([])
            };

            var initialUserPassword = Environment.GetEnvironmentVariable("INITIAL_ADMIN_PASSWORD")
                                      ?? throw new InvalidOperationException("INITIAL_ADMIN_PASSWORD environment variable is required");
            var identityResult = await userManager.CreateAsync(admin, initialUserPassword);

            if (!identityResult.Succeeded)
            {
                var errors = string.Join(", ", identityResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to create initial admin: {errors}");
            }
        }
    }

    private static async Task CreateInitialCategoriesAsync(this WebApplication app)
    {
        await using var asyncServiceScope = app.Services.CreateAsyncScope();
        var dbContext = asyncServiceScope.ServiceProvider.GetRequiredService<VictoryCenterDbContext>();
        var categories = new List<TeamCategory>
        {
            new()
            {
                Name = "Основна команда",
                Description = "Люди, які щодня координують роботу програм, супроводжують учасників, будують логістику, фасилітують сесії.",
                CreatedAt = DateTimeOffset.UtcNow
            },
            new()
            {
                Name = "Наглядова рада",
                Description = "Люди, які щодня координують роботу програм, супроводжують учасників, будують логістику, фасилітують сесії.",
                CreatedAt = DateTimeOffset.UtcNow
            },
            new()
            {
                Name = "Радники",
                Description = "Фахівці, які консультують нас у ключових напрямах: психічне здоров’я, етика, безпека, комунікації, фандрейзинг.  Їхні поради — наш додатковий компас.",
                CreatedAt = DateTimeOffset.UtcNow
            }
        };

        foreach (var category in categories)
        {
            if (!await dbContext.TeamCategories.AnyAsync(c => c.Name == category.Name))
            {
                dbContext.TeamCategories.Add(category);
                await dbContext.SaveChangesAsync();
            }
        }
    }

    private static void AddOpenApi(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "VictoryCenter API",
                Version = "v1"
            });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    []
                }
            });
        });
    }

    private static IServiceCollection ConfigureBlob(this IServiceCollection services, IConfiguration configuration)
    {
        var blobSection = configuration.GetSection("BlobEnvironmentVariables");
        var serviceType = blobSection.GetValue<string>("ServiceType");

        switch (serviceType)
        {
            case "Local":
                services.AddOptions<BlobEnvironmentVariables>()
                    .Bind(blobSection.GetSection("Local"))
                    .ValidateDataAnnotations()
                    .PostConfigure<IWebHostEnvironment>((options, env) =>
                    {
                        options.RootPath = env.WebRootPath;
                    });
                services.AddScoped<IBlobService, BlobService>();
                break;

            case "Azure":
                services.AddOptions<BlobEnvironmentVariables>().Bind(blobSection.GetSection("Azure"))
                    .ValidateDataAnnotations();
                services.AddScoped<IBlobService, BlobService>();
                break;

            default:
                throw new InvalidOperationException($"Unsupported Blob Service Type: {serviceType}");
        }

        return services;
    }

    private static void ScanInterfacesAndRegisterImplementations(this IServiceCollection services, Assembly assembly, Type interfaceType, ServiceLifetime lifetime)
    {
        var types = assembly.GetTypes().Where(x => !x.IsAbstract && x.IsClass);

        foreach (var type in types)
        {
            var interfaces = type.GetInterfaces();

            if (interfaceType.IsGenericTypeDefinition)
            {
                var matchingInterfaces = interfaces.Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == interfaceType);
                foreach (var matchingInterface in matchingInterfaces)
                {
                    services.Add(new ServiceDescriptor(matchingInterface, type, lifetime));
                }
            }
            else
            {
                if (interfaces.Contains(interfaceType))
                {
                    services.Add(new ServiceDescriptor(interfaceType, type, lifetime));
                }
            }
        }
    }
}
