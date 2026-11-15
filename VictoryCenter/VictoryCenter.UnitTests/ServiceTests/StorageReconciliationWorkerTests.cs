using System.Threading.Channels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using VictoryCenter.BLL.Services.StorageLifecycle;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.WebAPI.Extensions;
using VictoryCenter.WebAPI.Services;

namespace VictoryCenter.UnitTests.ServiceTests;

public partial class StorageOperationWorkerTests
{
    private readonly Mock<ILogger<StorageReconciliationWorker>> _reconciliationLogger = new();

    [Fact]
    public async Task Workers_BlockedScanDoesNotDelaySubsequentCleanupBatches()
    {
        var observer = new BlockingScanObserver();
        using var provider = Services(observer);
        await SeedMissingImageAsync(provider);
        using var reconciliation = ReconciliationWorker(provider);
        using var cleanup = Worker(provider);
        try
        {
            await reconciliation.StartAsync(CancellationToken.None);
            await observer.ScanStarted.Task.WaitAsync(TimeSpan.FromSeconds(15));

            var firstId = await QueueDeletionAsync(provider, "first.png");
            await cleanup.StartAsync(CancellationToken.None);
            Assert.Equal(firstId, await observer.Completed.Reader.ReadAsync().AsTask().WaitAsync(TimeSpan.FromSeconds(15)));
            var secondId = await QueueDeletionAsync(provider, "second.png");
            Assert.Equal(secondId, await observer.Completed.Reader.ReadAsync().AsTask().WaitAsync(TimeSpan.FromSeconds(15)));

            Assert.False(File.Exists(Path.Combine(_root, "images", "first.png")));
            Assert.False(File.Exists(Path.Combine(_root, "images", "second.png")));
            Assert.NotSame(observer.ScanContext, observer.OperationContext);
            Assert.Equal(1, observer.ScanCount);
            Assert.False(observer.ReleaseScan.Task.IsCompleted);
            await using var verification = provider.CreateAsyncScope();
            Assert.Empty(await verification.ServiceProvider.GetRequiredService<VictoryCenterDbContext>()
                .StorageReconciliationFindings.ToListAsync());
        }
        finally
        {
            await cleanup.StopAsync(CancellationToken.None).WaitAsync(TimeSpan.FromSeconds(5));
            await reconciliation.StopAsync(CancellationToken.None).WaitAsync(TimeSpan.FromSeconds(5));
        }

        Assert.True(cleanup.ExecuteTask!.IsCompletedSuccessfully);
        Assert.True(reconciliation.ExecuteTask!.IsCompletedSuccessfully);
    }

    [Fact]
    public async Task ReconciliationWorker_RetriesFailedScanWithoutStoppingCleanup()
    {
        var completed = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var recovered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        _reconciliationLogger.Setup(logger => logger.Log(
            LogLevel.Warning,
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception?>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()))
            .Callback(new InvocationAction(_ => recovered.TrySetResult()));
        using var provider = Services(new RejectFirstScan(), new CompletionObserver(completed, failFirstClaim: false));
        await SeedMissingImageAsync(provider);
        await QueueDeletionAsync(provider, "recover.png");
        using var reconciliation = ReconciliationWorker(provider);
        using var cleanup = Worker(provider);
        try
        {
            await reconciliation.StartAsync(CancellationToken.None);
            await cleanup.StartAsync(CancellationToken.None);
            await completed.Task.WaitAsync(TimeSpan.FromSeconds(15));
            await recovered.Task.WaitAsync(TimeSpan.FromSeconds(15));
        }
        finally
        {
            await cleanup.StopAsync(CancellationToken.None).WaitAsync(TimeSpan.FromSeconds(5));
            await reconciliation.StopAsync(CancellationToken.None).WaitAsync(TimeSpan.FromSeconds(5));
        }

        var error = Assert.Single(_reconciliationLogger.Invocations.Where(invocation => invocation.Method.Name == "Log"
            && (LogLevel)invocation.Arguments[0] == LogLevel.Error));
        Assert.DoesNotContain("sensitive failure detail", error.Arguments[2].ToString());
        Assert.Null(error.Arguments[3]);
        Assert.True(cleanup.ExecuteTask!.IsCompletedSuccessfully);
        Assert.True(reconciliation.ExecuteTask!.IsCompletedSuccessfully);
    }

    [Fact]
    public async Task ReconciliationWorker_UsesConfiguredIntervalAndCancelsLongWaitCleanly()
    {
        using var provider = Services();
        await SeedMissingImageAsync(provider);
        var clock = new ControlledScanClock();
        using var worker = ReconciliationWorker(provider, clock);
        try
        {
            await worker.StartAsync(CancellationToken.None);
            var firstWait = await clock.Waits.Reader.ReadAsync().AsTask().WaitAsync(TimeSpan.FromSeconds(15));
            Assert.Equal(TimeSpan.FromMinutes(60), firstWait.Delay);
            Assert.Single(_reconciliationLogger.Invocations.Where(invocation => invocation.Method.Name == "Log"));

            provider.GetRequiredService<IOptions<StorageLifecycleOptions>>().Value.ReconciliationIntervalMinutes = 120;
            firstWait.Callback(firstWait.State);
            var secondWait = await clock.Waits.Reader.ReadAsync().AsTask().WaitAsync(TimeSpan.FromSeconds(15));
            Assert.Equal(TimeSpan.FromMinutes(120), secondWait.Delay);
            Assert.Equal(2, _reconciliationLogger.Invocations.Count(invocation => invocation.Method.Name == "Log"));
        }
        finally
        {
            await worker.StopAsync(CancellationToken.None).WaitAsync(TimeSpan.FromSeconds(5));
        }

        Assert.True(worker.ExecuteTask!.IsCompletedSuccessfully);
    }

    [Fact]
    public async Task ReconciliationWorker_CancelsBlockedScanAndDisposesItsScope()
    {
        var observer = new BlockingScanObserver();
        using var provider = Services(observer);
        await SeedMissingImageAsync(provider);
        using var worker = ReconciliationWorker(provider);
        try
        {
            await worker.StartAsync(CancellationToken.None);
            await observer.ScanStarted.Task.WaitAsync(TimeSpan.FromSeconds(15));
        }
        finally
        {
            await worker.StopAsync(CancellationToken.None).WaitAsync(TimeSpan.FromSeconds(5));
        }

        Assert.True(worker.ExecuteTask!.IsCompletedSuccessfully);
        Assert.DoesNotContain(_reconciliationLogger.Invocations, invocation => invocation.Method.Name == "Log"
            && (LogLevel)invocation.Arguments[0] == LogLevel.Error);
        await Assert.ThrowsAsync<ObjectDisposedException>(() => observer.ScanContext!.Images.CountAsync());
    }

    [Fact]
    public void ApplicationServices_RegisterSeparateStorageWorkers()
    {
        Directory.CreateDirectory(_root);
        var services = new ServiceCollection();
        services.AddApplicationServices(new ConfigurationBuilder().Build());

        Assert.Single(services.Where(service => service.ServiceType == typeof(IHostedService)
            && service.ImplementationType == typeof(StorageOperationWorker)));
        Assert.Single(services.Where(service => service.ServiceType == typeof(IHostedService)
            && service.ImplementationType == typeof(StorageReconciliationWorker)));
    }

    private static async Task SeedMissingImageAsync(ServiceProvider provider)
    {
        await using var scope = provider.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<VictoryCenterDbContext>();
        context.Images.Add(new Image { BlobName = "missing", MimeType = "image/png" });
        await context.SaveChangesAsync();
    }

    private async Task<Guid> QueueDeletionAsync(ServiceProvider provider, string name)
    {
        await using var scope = provider.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<VictoryCenterDbContext>();
        var operation = new StorageOperation
        {
            FileName = name,
            FileKind = StorageFileKind.Image,
            Kind = StorageOperationKind.Delete,
            NextAttemptAt = _clock.GetUtcNow().AddSeconds(-1)
        };
        await File.WriteAllTextAsync(Path.Combine(_root, "images", name), "obsolete bytes");
        context.StorageOperations.Add(operation);
        await context.SaveChangesAsync();
        return operation.Id;
    }

    private StorageReconciliationWorker ReconciliationWorker(ServiceProvider provider, TimeProvider? clock = null) => new(
        provider.GetRequiredService<IServiceScopeFactory>(), provider.GetRequiredService<IOptions<StorageLifecycleOptions>>(),
        clock ?? _clock, _reconciliationLogger.Object);

    private sealed class BlockingScanObserver : SaveChangesInterceptor
    {
        public TaskCompletionSource ScanStarted { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public TaskCompletionSource ReleaseScan { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public Channel<Guid> Completed { get; } = Channel.CreateUnbounded<Guid>();
        public VictoryCenterDbContext? ScanContext { get; private set; }
        public DbContext? OperationContext { get; private set; }
        public int ScanCount { get; private set; }

        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            if (eventData.Context!.ChangeTracker.Entries<StorageReconciliationFinding>().Any())
            {
                ScanContext = (VictoryCenterDbContext)eventData.Context;
                ScanCount++;
                ScanStarted.TrySetResult();
                await ReleaseScan.Task.WaitAsync(cancellationToken);
            }

            return result;
        }

        public override ValueTask<int> SavedChangesAsync(
            SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
        {
            foreach (var entry in eventData.Context!.ChangeTracker.Entries<StorageOperation>()
                .Where(entry => entry.Entity.Status == StorageOperationStatus.Completed))
            {
                OperationContext = eventData.Context;
                Completed.Writer.TryWrite(entry.Entity.Id);
            }

            return ValueTask.FromResult(result);
        }
    }

    private sealed class RejectFirstScan : SaveChangesInterceptor
    {
        private int _failureRemaining = 1;

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            if (eventData.Context!.ChangeTracker.Entries<StorageReconciliationFinding>().Any()
                && Interlocked.Exchange(ref _failureRemaining, 0) == 1)
            {
                throw new DbUpdateException("sensitive failure detail");
            }

            return ValueTask.FromResult(result);
        }
    }

    private sealed class ControlledScanClock : TimeProvider
    {
        public Channel<(TimerCallback Callback, object? State, TimeSpan Delay)> Waits { get; } =
            Channel.CreateUnbounded<(TimerCallback Callback, object? State, TimeSpan Delay)>();

        public override ITimer CreateTimer(TimerCallback callback, object? state, TimeSpan dueTime, TimeSpan period)
        {
            Waits.Writer.TryWrite((callback, state, dueTime));
            return Mock.Of<ITimer>();
        }
    }
}
