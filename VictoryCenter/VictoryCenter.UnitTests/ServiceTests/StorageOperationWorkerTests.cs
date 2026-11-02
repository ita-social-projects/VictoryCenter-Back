using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using VictoryCenter.BLL.Services.BlobStorage;
using VictoryCenter.BLL.Services.PdfStorage;
using VictoryCenter.BLL.Services.StorageLifecycle;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.WebAPI.Services;

namespace VictoryCenter.UnitTests.ServiceTests;

public class StorageOperationWorkerTests : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), $"vc-worker-{Guid.NewGuid():N}");
    private readonly Mock<ILogger<StorageOperationWorker>> _logger = new();
    private readonly Mock<ILogger<StorageOperationProcessor>> _operationLogger = new();
    private readonly AdjustableClock _clock = new();

    [Fact]
    public async Task Worker_UnavailableRootDoesNotRecreateDirectoriesOrConsumeAttemptsAndRecoversAfterRestore()
    {
        var completed = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var unavailable = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        using var provider = Services(new CompletionObserver(completed, failFirstClaim: false));
        await using (var scope = provider.CreateAsyncScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<VictoryCenterDbContext>();
            context.StorageOperations.Add(new StorageOperation
            {
                FileName = "absent.png", FileKind = StorageFileKind.Image, Kind = StorageOperationKind.Delete,
                NextAttemptAt = _clock.GetUtcNow().AddSeconds(-1)
            });
            await context.SaveChangesAsync();
        }

        var imageRoot = Path.Combine(_root, "images");
        Directory.Delete(imageRoot);
        _logger.Setup(logger => logger.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception?>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()))
            .Callback(new InvocationAction(_ => unavailable.TrySetResult()));
        using var worker = Worker(provider);
        try
        {
            await worker.StartAsync(CancellationToken.None);
            await unavailable.Task.WaitAsync(TimeSpan.FromSeconds(15));
            Assert.False(Directory.Exists(imageRoot));
            await using (var scope = provider.CreateAsyncScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<VictoryCenterDbContext>();
                var operation = await context.StorageOperations.SingleAsync();
                Assert.Equal(StorageOperationStatus.Pending, operation.Status);
                Assert.Equal(0, operation.Attempts);
                Assert.Empty(await context.StorageReconciliationFindings.ToListAsync());
            }

            Directory.CreateDirectory(imageRoot);
            await completed.Task.WaitAsync(TimeSpan.FromSeconds(15));
        }
        finally
        {
            await worker.StopAsync(CancellationToken.None).WaitAsync(TimeSpan.FromSeconds(5));
        }

        Assert.True(worker.ExecuteTask!.IsCompletedSuccessfully);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Worker_ProcessesPendingWorkAndSurvivesTransientBatchFailure(bool failFirstClaim)
    {
        var completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        using var provider = Services(new CompletionObserver(completion, failFirstClaim));
        await using (var scope = provider.CreateAsyncScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<VictoryCenterDbContext>();
            context.StorageOperations.Add(new StorageOperation
            {
                FileKind = StorageFileKind.Image, FileName = "absent.png", Kind = StorageOperationKind.Delete,
                NextAttemptAt = _clock.GetUtcNow().AddSeconds(-1)
            });
            await context.SaveChangesAsync();
        }

        using var worker = Worker(provider);
        try
        {
            await worker.StartAsync(CancellationToken.None);
            await completion.Task.WaitAsync(TimeSpan.FromSeconds(15));
        }
        finally
        {
            await worker.StopAsync(CancellationToken.None).WaitAsync(TimeSpan.FromSeconds(5));
        }

        await using var verification = provider.CreateAsyncScope();
        var saved = await verification.ServiceProvider.GetRequiredService<VictoryCenterDbContext>().StorageOperations.SingleAsync();
        Assert.Equal(StorageOperationStatus.Completed, saved.Status);
        Assert.Equal(1, saved.Attempts);
        Assert.DoesNotContain(_logger.Invocations, invocation => invocation.Method.Name == "Log"
            && (LogLevel)invocation.Arguments[0] == LogLevel.Error);
        var errors = _operationLogger.Invocations.Where(invocation => invocation.Method.Name == "Log"
            && (LogLevel)invocation.Arguments[0] == LogLevel.Error).ToList();
        Assert.Equal(failFirstClaim ? 1 : 0, errors.Count);
        Assert.All(errors, invocation => Assert.DoesNotContain("sensitive failure detail", invocation.Arguments[2].ToString()));
        Assert.True(worker.ExecuteTask!.IsCompletedSuccessfully);
    }

    [Fact]
    public async Task Worker_RescansAfterConfiguredIntervalAndCancelsLongWaitCleanly()
    {
        var firstWait = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var longWait = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var secondScan = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var waits = 0;
        _clock.TimerCreated = () =>
        {
            if (Interlocked.Increment(ref waits) == 1)
            {
                firstWait.TrySetResult();
            }
            else
            {
                longWait.TrySetResult();
            }
        };
        var scans = 0;
        _logger.Setup(logger => logger.Log(
            LogLevel.Warning,
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception?>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()))
            .Callback(new InvocationAction(_ =>
            {
                if (Interlocked.Increment(ref scans) == 2)
                {
                    secondScan.TrySetResult();
                }
            }));
        using var provider = Services();
        await using (var scope = provider.CreateAsyncScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<VictoryCenterDbContext>();
            context.Images.Add(new Image { BlobName = "missing", MimeType = "image/png" });
            await context.SaveChangesAsync();
        }

        using var worker = Worker(provider);
        try
        {
            await worker.StartAsync(CancellationToken.None);
            await firstWait.Task.WaitAsync(TimeSpan.FromSeconds(15));
            Assert.Equal(1, Volatile.Read(ref scans));
            provider.GetRequiredService<IOptions<StorageLifecycleOptions>>().Value.PollIntervalSeconds = 3600;
            _clock.Advance(TimeSpan.FromMinutes(61));
            await secondScan.Task.WaitAsync(TimeSpan.FromSeconds(15));
            await longWait.Task.WaitAsync(TimeSpan.FromSeconds(15));
        }
        finally
        {
            await worker.StopAsync(CancellationToken.None).WaitAsync(TimeSpan.FromSeconds(5));
        }

        Assert.True(worker.ExecuteTask!.IsCompletedSuccessfully);
    }

    public void Dispose()
    {
        Directory.Delete(_root, recursive: true);
        GC.SuppressFinalize(this);
    }

    private ServiceProvider Services(SaveChangesInterceptor? interceptor = null)
    {
        Directory.CreateDirectory(Path.Combine(_root, "images"));
        Directory.CreateDirectory(Path.Combine(_root, "pdfs"));
        var database = Guid.NewGuid().ToString();
        return new ServiceCollection()
            .AddLogging()
            .AddSingleton(_operationLogger.Object)
            .AddSingleton<TimeProvider>(_clock)
            .AddSingleton(Options.Create(new StorageLifecycleOptions { PollIntervalSeconds = 1 }))
            .AddSingleton(Options.Create(new BlobEnvironmentVariables { RootPath = _root, ImagesSubPath = "images" }))
            .AddSingleton(Options.Create(new PdfEnvironmentVariables { RootPath = _root, PdfSubPath = "pdfs" }))
            .AddDbContext<VictoryCenterDbContext>(options =>
            {
                options.UseInMemoryDatabase(database);
                if (interceptor != null)
                {
                    options.AddInterceptors(interceptor);
                }
            })
            .AddScoped<LocalStorageFiles>()
            .AddScoped<StorageOperationProcessor>()
            .AddScoped<StorageReconciliation>()
            .BuildServiceProvider(validateScopes: true);
    }

    private StorageOperationWorker Worker(ServiceProvider provider) => new(
        provider.GetRequiredService<IServiceScopeFactory>(), provider.GetRequiredService<IOptions<StorageLifecycleOptions>>(), _clock, _logger.Object);

    private sealed class CompletionObserver(TaskCompletionSource completion, bool failFirstClaim) : SaveChangesInterceptor
    {
        private int _failureRemaining = failFirstClaim ? 1 : 0;

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            if (eventData.Context!.ChangeTracker.Entries<StorageOperation>().Any(entry => entry.Entity.Status == StorageOperationStatus.Processing)
                && Interlocked.Exchange(ref _failureRemaining, 0) == 1)
            {
                throw new DbUpdateException("sensitive failure detail");
            }

            return ValueTask.FromResult(result);
        }

        public override ValueTask<int> SavedChangesAsync(
            SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
        {
            if (eventData.Context!.ChangeTracker.Entries<StorageOperation>().Any(entry => entry.Entity.Status == StorageOperationStatus.Completed))
            {
                completion.TrySetResult();
            }

            return ValueTask.FromResult(result);
        }
    }

    private sealed class AdjustableClock : TimeProvider
    {
        private long _offset;

        public Action? TimerCreated { get; set; }

        public override DateTimeOffset GetUtcNow() => DateTimeOffset.UtcNow.AddTicks(Interlocked.Read(ref _offset));

        public void Advance(TimeSpan duration) => Interlocked.Add(ref _offset, duration.Ticks);

        public override ITimer CreateTimer(TimerCallback callback, object? state, TimeSpan dueTime, TimeSpan period)
        {
            var timer = base.CreateTimer(callback, state, dueTime, period);
            TimerCreated?.Invoke();
            return timer;
        }
    }
}
