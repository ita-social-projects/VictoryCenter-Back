using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using VictoryCenter.BLL.Interceptors;
using VictoryCenter.BLL.Services.Audit;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.Attributes;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.UnitTests.ServiceTests.Audit;

public class AuditInterceptorTests
{
    private readonly DbContextOptions<TestDbContext> _options;
    private readonly TestDbContext _context;

    public AuditInterceptorTests()
    {
        var timeProvider = new Mock<TimeProvider>();
        timeProvider.Setup(t => t.GetUtcNow()).Returns(new DateTimeOffset(2026, 11, 13, 0, 0, 0, TimeSpan.Zero));
        var interceptor = new AuditInterceptor(
            new AuditLogFactory(new Mock<IHttpContextAccessor>().Object, timeProvider.Object));

        _options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .AddInterceptors(interceptor)
            .Options;

        _context = new TestDbContext(_options);
    }

    [Fact]
    public async Task SaveChangesAsync_AddedAuditableEntity_ShouldWriteCreateLogWithoutUnauditedFields()
    {
        // Arrange
        var entity = new AuditedTestEntity { Name = "Name", Status = Status.Published, Secret = "secret-value" };
        _context.AuditedEntities.Add(entity);

        // Act
        await _context.SaveChangesAsync();

        // Assert
        var log = await _context.AuditLogs.SingleAsync();
        Assert.Equal(AuditOperation.Create, log.Operation);
        Assert.Equal(nameof(AuditedTestEntity), log.EntityType);
        Assert.Equal(entity.Id.ToString(), log.EntityId);
        Assert.Contains("\"Status\":{\"Old\":null,\"New\":\"Published\"}", log.Changes);
        Assert.DoesNotContain("secret-value", log.Changes);
    }

    [Fact]
    public async Task SaveChangesAsync_AuditedPropertyChanged_ShouldWriteUpdateLogWithOldAndNewValues()
    {
        // Arrange
        var entity = await CreateEntityAsync();
        entity.Name = "New name";

        // Act
        await _context.SaveChangesAsync();

        // Assert
        var log = Assert.Single(await _context.AuditLogs.ToListAsync(), l => l.Operation == AuditOperation.Update);
        Assert.Equal(entity.Id.ToString(), log.EntityId);
        Assert.Equal("{\"Name\":{\"Old\":\"Old name\",\"New\":\"New name\"}}", log.Changes);
    }

    [Fact]
    public async Task SaveChangesAsync_OnlyUnauditedPropertyChanged_ShouldWriteUpdateLogWithoutChanges()
    {
        // Arrange
        var entity = await CreateEntityAsync();
        entity.Secret = "new-secret";

        // Act
        await _context.SaveChangesAsync();

        // Assert
        var log = Assert.Single(await _context.AuditLogs.ToListAsync(), l => l.Operation == AuditOperation.Update);
        Assert.Null(log.Changes);
    }

    [Fact]
    public async Task SaveChangesAsync_DetachedEntityUpdatedWithSameValues_ShouldNotWriteUpdateLog()
    {
        // Arrange
        var entity = await CreateEntityAsync();
        _context.ChangeTracker.Clear();
        _context.AuditedEntities.Update(new AuditedTestEntity
        {
            Id = entity.Id,
            Name = entity.Name,
            Status = entity.Status,
            Secret = entity.Secret,
        });

        // Act
        await _context.SaveChangesAsync();

        // Assert
        Assert.DoesNotContain(await _context.AuditLogs.ToListAsync(), l => l.Operation == AuditOperation.Update);
    }

    [Fact]
    public async Task SaveChangesAsync_ConcurrentUpdatesOfDifferentFields_ShouldRecordOnlyModifiedField()
    {
        // Arrange
        var entity = await CreateEntityAsync();
        await using var otherContext = new TestDbContext(_options);
        var otherEntity = await otherContext.AuditedEntities.SingleAsync(e => e.Id == entity.Id);
        otherEntity.Status = Status.Published;
        await otherContext.SaveChangesAsync();
        entity.Name = "New name";

        // Act
        await _context.SaveChangesAsync();

        // Assert
        var updates = await _context.AuditLogs
            .Where(l => l.Operation == AuditOperation.Update)
            .OrderBy(l => l.Id)
            .ToListAsync();
        Assert.Equal(2, updates.Count);
        Assert.Equal("{\"Name\":{\"Old\":\"Old name\",\"New\":\"New name\"}}", updates[1].Changes);
    }

    [Fact]
    public async Task SaveChangesAsync_DeletedAuditableEntity_ShouldWriteDeleteLogWithOldValues()
    {
        // Arrange
        var entity = await CreateEntityAsync();
        _context.AuditedEntities.Remove(entity);

        // Act
        await _context.SaveChangesAsync();

        // Assert
        var log = Assert.Single(await _context.AuditLogs.ToListAsync(), l => l.Operation == AuditOperation.Delete);
        Assert.Equal(entity.Id.ToString(), log.EntityId);
        Assert.Contains("\"Name\":{\"Old\":\"Old name\",\"New\":null}", log.Changes);
        Assert.DoesNotContain("old-secret", log.Changes);
    }

    [Fact]
    public async Task SaveChangesAsync_EntityWithoutAuditableAttribute_ShouldNotWriteLog()
    {
        // Arrange
        _context.NotAuditedEntities.Add(new NotAuditedTestEntity { Name = "Name" });

        // Act
        await _context.SaveChangesAsync();

        // Assert
        Assert.Empty(await _context.AuditLogs.ToListAsync());
    }

    [Fact]
    public async Task SaveChangesAsync_ModifiedAuditLog_ShouldThrow()
    {
        // Arrange
        await CreateEntityAsync();
        var log = await _context.AuditLogs.SingleAsync();
        log.Changes = "tampered";

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _context.SaveChangesAsync());
    }

    [Fact]
    public async Task SaveChangesAsync_DeletedAuditLog_ShouldThrow()
    {
        // Arrange
        await CreateEntityAsync();
        var log = await _context.AuditLogs.SingleAsync();
        _context.AuditLogs.Remove(log);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _context.SaveChangesAsync());
    }

    [Fact]
    public void SaveChanges_SynchronousCreateUpdateAndDelete_ShouldWriteAllLogs()
    {
        // Arrange
        var entity = new AuditedTestEntity { Name = "Old name", Status = Status.Draft };
        _context.AuditedEntities.Add(entity);
        _context.SaveChanges();

        // Act
        entity.Name = "New name";
        _context.SaveChanges();
        _context.AuditedEntities.Remove(entity);
        _context.SaveChanges();

        // Assert
        var operations = _context.AuditLogs.OrderBy(l => l.Id).Select(l => l.Operation).ToList();
        Assert.Equal(new[] { AuditOperation.Create, AuditOperation.Update, AuditOperation.Delete }, operations);
    }

    [Fact]
    public async Task SaveChangesAsync_FailedCreate_ShouldNotWriteCreateLog()
    {
        // Arrange
        var existing = await CreateEntityAsync();
        _context.ChangeTracker.Clear();
        _context.AuditedEntities.Add(new AuditedTestEntity { Id = existing.Id, Name = "Duplicate" });

        // Act
        await Assert.ThrowsAnyAsync<Exception>(() => _context.SaveChangesAsync());

        // Assert
        _context.ChangeTracker.Clear();
        Assert.Single(await _context.AuditLogs.ToListAsync(), l => l.Operation == AuditOperation.Create);
    }

    [Fact]
    public void SaveChanges_FailedCreate_ShouldNotWriteCreateLog()
    {
        // Arrange
        var existing = new AuditedTestEntity { Name = "Old name" };
        _context.AuditedEntities.Add(existing);
        _context.SaveChanges();
        _context.ChangeTracker.Clear();
        _context.AuditedEntities.Add(new AuditedTestEntity { Id = existing.Id, Name = "Duplicate" });

        // Act
        Assert.ThrowsAny<Exception>(() => _context.SaveChanges());

        // Assert
        _context.ChangeTracker.Clear();
        Assert.Single(_context.AuditLogs.ToList(), l => l.Operation == AuditOperation.Create);
    }

    [Fact]
    public async Task SaveChangesAsync_RepeatedReportPublish_ShouldWriteCreateLogForEachSnapshot()
    {
        // Arrange
        var publishedAt = new DateTimeOffset(2026, 11, 1, 0, 0, 0, TimeSpan.Zero);
        _context.PublishedSnapshots.Add(new PublishedReportFundsExpendituresSnapshot { DisclaimerTitle = "Report", PublishedAt = publishedAt });
        await _context.SaveChangesAsync();
        _context.PublishedSnapshots.Add(new PublishedReportFundsExpendituresSnapshot { DisclaimerTitle = "Report", PublishedAt = publishedAt.AddDays(1) });

        // Act
        await _context.SaveChangesAsync();

        // Assert
        var logs = await _context.AuditLogs
            .Where(l => l.EntityType == nameof(PublishedReportFundsExpendituresSnapshot))
            .OrderBy(l => l.Id)
            .ToListAsync();
        Assert.Equal(2, logs.Count);
        Assert.All(logs, l => Assert.Equal(AuditOperation.Create, l.Operation));
        Assert.Contains("\"PublishedAt\":{\"Old\":null,\"New\":\"2026-11-02T00:00:00+00:00\"}", logs[1].Changes);
    }

    private async Task<AuditedTestEntity> CreateEntityAsync()
    {
        var entity = new AuditedTestEntity { Name = "Old name", Status = Status.Draft, Secret = "old-secret" };
        _context.AuditedEntities.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    [Auditable]
    public class AuditedTestEntity
    {
        public long Id { get; set; }

        [Audited]
        public string Name { get; set; } = null!;

        [Audited]
        public Status Status { get; set; }

        public string? Secret { get; set; }
    }

    public class NotAuditedTestEntity
    {
        public long Id { get; set; }

        public string Name { get; set; } = null!;
    }

    public class TestDbContext(DbContextOptions<TestDbContext> options) : DbContext(options)
    {
        public DbSet<AuditedTestEntity> AuditedEntities => Set<AuditedTestEntity>();

        public DbSet<NotAuditedTestEntity> NotAuditedEntities => Set<NotAuditedTestEntity>();

        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

        public DbSet<PublishedReportFundsExpendituresSnapshot> PublishedSnapshots => Set<PublishedReportFundsExpendituresSnapshot>();
    }
}
