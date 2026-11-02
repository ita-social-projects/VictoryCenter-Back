using System.Transactions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using VictoryCenter.BLL.Services.BlobStorage;
using VictoryCenter.BLL.Services.PdfStorage;
using VictoryCenter.BLL.Services.StorageLifecycle;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Migrations;

namespace VictoryCenter.IntegrationTests.StorageLifecycle;

public partial class StorageSqlServerTests : IAsyncLifetime
{
    private const string DatabasePrefix = "VictoryCenter_StorageTests_";
    private readonly string _databaseName = DatabasePrefix + Guid.NewGuid().ToString("N");
    private readonly string _root = Path.Combine(Path.GetTempPath(), $"vc-storage-sql-{Guid.NewGuid():N}");
    private readonly DateTimeOffset _now = DateTimeOffset.UtcNow;
    private readonly Mock<TimeProvider> _clock = new();
    private readonly StorageLifecycleOptions _options = new() { UploadReservationHours = 1 };
    private DbContextOptions<VictoryCenterDbContext> _databaseOptions = null!;

    public async Task InitializeAsync()
    {
        var connection = new SqlConnectionStringBuilder(Environment.GetEnvironmentVariable(StorageSqlServerFactAttribute.ConnectionVariable))
        {
            InitialCatalog = _databaseName,
            Pooling = true,
            ConnectTimeout = 10
        };
        _databaseOptions = new DbContextOptionsBuilder<VictoryCenterDbContext>()
            .UseSqlServer(connection.ConnectionString, sql => sql.MigrationsHistoryTable("__EFMigrationsHistory", "entity_framework")).Options;
        _clock.Setup(clock => clock.GetUtcNow()).Returns(_now);
        Directory.CreateDirectory(Path.Combine(_root, "images"));
        Directory.CreateDirectory(Path.Combine(_root, "pdfs"));
        await using var context = Context();
        await context.Database.EnsureCreatedAsync();
    }

    [StorageSqlServerFact]
    public async Task ReplacementRollback_PreservesOldMetadataAndIndependentUploadReservation()
    {
        var image = await SeedImageAsync("original");
        using var provider = new ServiceCollection().AddScoped(_ => Context()).BuildServiceProvider();
        var reservations = new StorageUploadReservations(provider.GetRequiredService<IServiceScopeFactory>(), Files(), Options.Create(_options), _clock.Object);
        var replacementPath = Files().GetPath(StorageFileKind.Image, "replacement.png");

        using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            await reservations.ReserveAsync(StorageFileKind.Image, "replacement.png");
            await File.WriteAllTextAsync(replacementPath, "replacement bytes");
            await using var context = Context();
            var replacement = await context.Images.SingleAsync(item => item.Id == image.Id);
            replacement.BlobName = "replacement";
            await context.SaveChangesAsync();

            // Deliberately do not complete the ambient metadata transaction.
        }

        await using var verification = Context();
        Assert.Equal("original", (await verification.Images.SingleAsync()).BlobName);
        var intent = await verification.StorageOperations.SingleAsync();
        Assert.Equal(StorageOperationKind.UploadCleanup, intent.Kind);
        Assert.Equal(StorageOperationStatus.Pending, intent.Status);
        _clock.Setup(clock => clock.GetUtcNow()).Returns(_now.AddHours(2));
        await Processor(verification).ProcessBatchAsync(CancellationToken.None);
        Assert.False(File.Exists(replacementPath));
        Assert.True(File.Exists(Files().GetPath(StorageFileKind.Image, "original.png")));
    }

    [StorageSqlServerFact]
    public async Task DeleteForeignKeyFailure_RollsBackMetadataAndOutboxTogether()
    {
        var image = await SeedImageAsync("referenced");
        await using var context = Context();
        await context.Database.ExecuteSqlRawAsync(
            "CREATE TABLE dbo.StorageTestReferences (ImageId bigint NOT NULL REFERENCES media.Images(Id));");
        await context.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO dbo.StorageTestReferences (ImageId) VALUES ({image.Id});");
        context.Images.Remove(await context.Images.SingleAsync());

        await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());

        await using var verification = Context();
        Assert.True(await verification.Images.AnyAsync(item => item.Id == image.Id));
        Assert.Empty(await verification.StorageOperations.ToListAsync());
        Assert.True(File.Exists(Files().GetPath(StorageFileKind.Image, "referenced.png")));
    }

    [StorageSqlServerFact]
    public async Task CompetingReplacements_RollBackLosingOutboxAndPreserveWinnerFile()
    {
        var image = await SeedImageAsync("original");
        await using (var seed = Context())
        {
            seed.StorageOperations.AddRange(Upload("winner.png"), Upload("loser.png"));
            await seed.SaveChangesAsync();
        }

        await File.WriteAllTextAsync(Files().GetPath(StorageFileKind.Image, "winner.png"), "winning bytes");
        await File.WriteAllTextAsync(Files().GetPath(StorageFileKind.Image, "loser.png"), "losing bytes");
        await using var first = Context();
        await using var second = Context();
        var winner = await first.Images.SingleAsync(item => item.Id == image.Id);
        var loser = await second.Images.SingleAsync(item => item.Id == image.Id);
        winner.BlobName = "winner";
        loser.BlobName = "loser";
        await first.SaveChangesAsync();

        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => second.SaveChangesAsync());

        await using var verification = Context();
        Assert.Equal("winner", (await verification.Images.SingleAsync()).BlobName);
        Assert.Single(await verification.StorageOperations.Where(operation => operation.Kind == StorageOperationKind.Delete).ToListAsync());
        Assert.Equal(StorageOperationStatus.Pending, (await verification.StorageOperations.SingleAsync(operation => operation.FileName == "loser.png")).Status);
        _clock.Setup(clock => clock.GetUtcNow()).Returns(_now.AddHours(2));
        await Processor(verification).ProcessBatchAsync(CancellationToken.None);
        Assert.True(File.Exists(Files().GetPath(StorageFileKind.Image, "winner.png")));
        Assert.False(File.Exists(Files().GetPath(StorageFileKind.Image, "loser.png")));
        Assert.False(File.Exists(Files().GetPath(StorageFileKind.Image, "original.png")));
    }

    [StorageSqlServerFact]
    public async Task UniqueUploadIndex_RejectsDuplicateReservation()
    {
        await using var first = Context();
        first.StorageOperations.Add(Upload("reserved.png"));
        await first.SaveChangesAsync();
        await using var second = Context();
        second.StorageOperations.Add(Upload("reserved.png"));

        await Assert.ThrowsAsync<DbUpdateException>(() => second.SaveChangesAsync());

        await using var verification = Context();
        Assert.Single(await verification.StorageOperations.ToListAsync());
    }

    [StorageSqlServerFact]
    public async Task TwoWorkers_OnlyOneClaimCanDeleteAndAcknowledge()
    {
        await using (var seed = Context())
        {
            seed.StorageOperations.Add(new StorageOperation
            {
                FileKind = StorageFileKind.Image, FileName = "queued.png", Kind = StorageOperationKind.Delete,
                NextAttemptAt = _now.AddMinutes(-1)
            });
            await seed.SaveChangesAsync();
        }

        var path = Files().GetPath(StorageFileKind.Image, "queued.png");
        await File.WriteAllTextAsync(path, "queued bytes");
        var barrier = new SaveBarrier(context => context.ChangeTracker.Entries<StorageOperation>()
            .Any(entry => entry.Entity.Status == StorageOperationStatus.Processing));
        await using var first = Context(barrier);
        await using var second = Context(barrier);
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(30));

        var results = await Task.WhenAll(
            Processor(first).ProcessBatchAsync(timeout.Token), Processor(second).ProcessBatchAsync(timeout.Token));

        Assert.Equal(1, results.Sum());
        await using var verification = Context();
        var operation = await verification.StorageOperations.SingleAsync();
        Assert.Equal(StorageOperationStatus.Completed, operation.Status);
        Assert.Equal(1, operation.Attempts);
        Assert.False(File.Exists(path));
    }

    [StorageSqlServerFact]
    public async Task ReclaimedUpload_RejectsStaleMetadataCommitAtomically()
    {
        await using (var seed = Context())
        {
            var intent = Upload("expired.png");
            intent.NextAttemptAt = _now.AddMinutes(-1);
            seed.StorageOperations.Add(intent);
            await seed.SaveChangesAsync();
        }

        await File.WriteAllTextAsync(Files().GetPath(StorageFileKind.Image, "expired.png"), "expired bytes");
        await using var metadata = Context();
        await metadata.StorageOperations.SingleAsync();
        metadata.Images.Add(new Image { BlobName = "expired", MimeType = "image/png" });
        await using (var worker = Context())
        {
            await Processor(worker).ProcessBatchAsync(CancellationToken.None);
        }

        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => metadata.SaveChangesAsync());

        await using var verification = Context();
        Assert.Empty(await verification.Images.ToListAsync());
        Assert.Equal(StorageOperationStatus.Completed, (await verification.StorageOperations.SingleAsync()).Status);
    }

    [StorageSqlServerFact]
    public async Task Migration_DownAndUp_PreserveExistingImageMetadata()
    {
        var image = await SeedImageAsync("legacy");
        await using var context = Context();
        var migration = new AddStorageLifecycle();
        var generator = context.GetService<IMigrationsSqlGenerator>();
        foreach (var command in generator.Generate(migration.DownOperations, context.Model))
        {
            await context.Database.ExecuteSqlRawAsync(command.CommandText);
        }

        foreach (var command in generator.Generate(migration.UpOperations, context.Model))
        {
            await context.Database.ExecuteSqlRawAsync(command.CommandText);
        }

        var restored = await context.Images.AsNoTracking().SingleAsync();
        Assert.Equal(image.Id, restored.Id);
        Assert.Equal(image.BlobName, restored.BlobName);
        Assert.Equal(image.MimeType, restored.MimeType);
        Assert.Equal(Guid.Empty, restored.StorageVersion);
        Assert.Empty(await context.StorageOperations.ToListAsync());
        Assert.Empty(await context.StorageReconciliationFindings.ToListAsync());
        Assert.True(File.Exists(Files().GetPath(StorageFileKind.Image, "legacy.png")));
    }

    [StorageSqlServerFact]
    public async Task Migration_FullChainUpgradesPreviousSchemaAndRecordsHistory()
    {
        await using var context = Context();
        var migrations = context.Database.GetMigrations().ToArray();
        var current = Array.FindIndex(migrations, migration => migration.EndsWith("_AddStorageLifecycle", StringComparison.Ordinal));
        Assert.True(current > 0);
        await context.Database.EnsureDeletedAsync();
        var migrator = context.GetService<IMigrator>();
        await migrator.MigrateAsync(migrations[current - 1]);
        await context.Database.ExecuteSqlInterpolatedAsync(
            $"INSERT INTO media.Images (BlobName, MimeType, CreatedAt, UpdatedAt) VALUES ({"legacy"}, {"image/png"}, {_now}, {_now});");

        await migrator.MigrateAsync();

        var restored = await context.Images.AsNoTracking().SingleAsync();
        Assert.Equal("legacy", restored.BlobName);
        Assert.Equal(Guid.Empty, restored.StorageVersion);
        Assert.Contains(migrations[current], await context.Database.GetAppliedMigrationsAsync());
        Assert.Empty(await context.Database.GetPendingMigrationsAsync());
        Assert.Empty(await context.StorageOperations.ToListAsync());
        Assert.Empty(await context.StorageReconciliationFindings.ToListAsync());
    }

    public async Task DisposeAsync()
    {
        if (_databaseOptions != null)
        {
            await using var context = Context();
            var target = new SqlConnectionStringBuilder(context.Database.GetConnectionString());
            if (target.InitialCatalog != _databaseName || !_databaseName.StartsWith(DatabasePrefix, StringComparison.Ordinal))
            {
                throw new InvalidOperationException("Refusing to delete a database outside the generated test database name.");
            }

            await context.Database.EnsureDeletedAsync();
        }

        if (Directory.Exists(_root))
        {
            Directory.Delete(_root, recursive: true);
        }
    }

    private VictoryCenterDbContext Context(SaveChangesInterceptor? interceptor = null)
    {
        var options = new DbContextOptionsBuilder<VictoryCenterDbContext>(_databaseOptions);
        if (interceptor != null)
        {
            options.AddInterceptors(interceptor);
        }

        return new VictoryCenterDbContext(options.Options);
    }

    private async Task<Image> SeedImageAsync(string name)
    {
        await using var context = Context();
        var image = new Image { BlobName = name, MimeType = "image/png", CreatedAt = _now, UpdatedAt = _now };
        context.Images.Add(image);
        await context.SaveChangesAsync();
        await File.WriteAllTextAsync(Files().GetPath(StorageFileKind.Image, name + ".png"), "original bytes");
        return image;
    }

    private StorageOperation Upload(string name) => new()
    {
        FileKind = StorageFileKind.Image, FileName = name, Kind = StorageOperationKind.UploadCleanup,
        NextAttemptAt = _now.AddHours(_options.UploadReservationHours)
    };

    private LocalStorageFiles Files() => new(
        Options.Create(new BlobEnvironmentVariables { RootPath = _root, ImagesSubPath = "images" }),
        Options.Create(new PdfEnvironmentVariables { RootPath = _root, PdfSubPath = "pdfs" }));

    private StorageOperationProcessor Processor(VictoryCenterDbContext context) => new(
        context, Files(), Options.Create(_options), _clock.Object, NullLogger<StorageOperationProcessor>.Instance);

    private sealed class SaveBarrier(Func<DbContext, bool> shouldWait) : SaveChangesInterceptor
    {
        private readonly TaskCompletionSource _ready = new(TaskCreationOptions.RunContinuationsAsynchronously);
        private int _arrivals;

        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            if (shouldWait(eventData.Context!))
            {
                if (Interlocked.Increment(ref _arrivals) == 2)
                {
                    _ready.TrySetResult();
                }

                await _ready.Task.WaitAsync(cancellationToken);
            }

            return result;
        }
    }
}
