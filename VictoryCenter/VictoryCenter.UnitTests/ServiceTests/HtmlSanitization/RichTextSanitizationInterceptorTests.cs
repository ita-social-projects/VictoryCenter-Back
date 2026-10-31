using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using VictoryCenter.BLL.Interceptors;
using VictoryCenter.BLL.Services.HtmlSanitization;
using VictoryCenter.DAL.Entities.Attributes;

namespace VictoryCenter.UnitTests.ServiceTests.HtmlSanitization;

public class RichTextSanitizationInterceptorTests
{
    private const string UnsafeHtml = "<p onclick=\"alert(1)\">Text<script>alert(1)</script></p>";
    private const string SanitizedHtml = "<p>Text</p>";

    private readonly Mock<ILogger<RichTextSanitizationInterceptor>> _mockLogger = new();
    private readonly string _databaseName = Guid.NewGuid().ToString();
    private readonly TestDbContext _context;

    public RichTextSanitizationInterceptorTests()
    {
        var interceptor = new RichTextSanitizationInterceptor(
            new RichTextSanitizer(),
            _mockLogger.Object);

        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(_databaseName)
            .AddInterceptors(interceptor)
            .Options;

        _context = new TestDbContext(options);
    }

    [Fact]
    public async Task SaveChangesAsync_AddedEntity_ShouldSanitizeRichTextProperties()
    {
        // Arrange
        var entity = new TestEntity { Content = UnsafeHtml, OptionalContent = UnsafeHtml, PlainText = UnsafeHtml };
        _context.TestEntities.Add(entity);

        // Act
        await _context.SaveChangesAsync();

        // Assert
        var stored = await GetStoredEntityAsync(entity.Id);
        Assert.Equal(SanitizedHtml, stored.Content);
        Assert.Equal(SanitizedHtml, stored.OptionalContent);
    }

    [Fact]
    public async Task SaveChangesAsync_PropertyWithoutRichTextAttribute_ShouldNotBeSanitized()
    {
        // Arrange
        var entity = new TestEntity { Content = SanitizedHtml, PlainText = UnsafeHtml };
        _context.TestEntities.Add(entity);

        // Act
        await _context.SaveChangesAsync();

        // Assert
        var stored = await GetStoredEntityAsync(entity.Id);
        Assert.Equal(UnsafeHtml, stored.PlainText);
    }

    [Fact]
    public async Task SaveChangesAsync_ModifiedEntity_ShouldSanitizeRichTextProperties()
    {
        // Arrange
        var entity = new TestEntity { Content = SanitizedHtml };
        _context.TestEntities.Add(entity);
        await _context.SaveChangesAsync();

        // Act
        entity.Content = UnsafeHtml;
        await _context.SaveChangesAsync();

        // Assert
        var stored = await GetStoredEntityAsync(entity.Id);
        Assert.Equal(SanitizedHtml, stored.Content);
    }

    [Fact]
    public async Task SaveChangesAsync_ModifiedEntity_ShouldAlsoSanitizeUnmodifiedRichTextProperties()
    {
        // Arrange: content stored before the sanitization policy existed
        var id = await StoreWithoutInterceptorAsync(new TestEntity { Content = UnsafeHtml, PlainText = "Old" });
        var entity = await _context.TestEntities.SingleAsync(e => e.Id == id);

        // Act
        entity.PlainText = "New";
        await _context.SaveChangesAsync();

        // Assert
        var stored = await GetStoredEntityAsync(id);
        Assert.Equal(SanitizedHtml, stored.Content);
        Assert.Equal("New", stored.PlainText);
    }

    [Fact]
    public async Task SaveChangesAsync_UnchangedEntity_ShouldNotBeTouched()
    {
        // Arrange
        var id = await StoreWithoutInterceptorAsync(new TestEntity { Content = UnsafeHtml });
        var entity = await _context.TestEntities.SingleAsync(e => e.Id == id);

        // Act
        await _context.SaveChangesAsync();

        // Assert
        Assert.Equal(EntityState.Unchanged, _context.Entry(entity).State);
        Assert.Equal(UnsafeHtml, entity.Content);
    }

    [Fact]
    public async Task SaveChangesAsync_NullRichTextProperty_ShouldBeSavedAsNull()
    {
        // Arrange
        var entity = new TestEntity { Content = SanitizedHtml, OptionalContent = null };
        _context.TestEntities.Add(entity);

        // Act
        await _context.SaveChangesAsync();

        // Assert
        var stored = await GetStoredEntityAsync(entity.Id);
        Assert.Null(stored.OptionalContent);
    }

    [Fact]
    public void SaveChanges_Synchronous_ShouldSanitizeRichTextProperties()
    {
        // Arrange
        var entity = new TestEntity { Content = UnsafeHtml };
        _context.TestEntities.Add(entity);

        // Act
        _context.SaveChanges();

        // Assert
        Assert.Equal(SanitizedHtml, entity.Content);
    }

    [Fact]
    public async Task SaveChangesAsync_RemovedContent_ShouldLogWarning()
    {
        // Arrange
        _context.TestEntities.Add(new TestEntity { Content = UnsafeHtml });

        // Act
        await _context.SaveChangesAsync();

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) =>
                    v.ToString()!.Contains("TestEntity.Content")
                    && v.ToString()!.Contains("<script>")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Theory]
    [InlineData("<p>Clean <strong>content</strong></p>")]
    [InlineData("<p>a<br/>b&#160;c</p>")]
    public async Task SaveChangesAsync_NothingRemoved_ShouldNotLogWarning(string html)
    {
        // Arrange
        _context.TestEntities.Add(new TestEntity { Content = html });

        // Act
        await _context.SaveChangesAsync();

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Never);
    }

    private async Task<TestEntity> GetStoredEntityAsync(long id)
    {
        _context.ChangeTracker.Clear();
        return await _context.TestEntities.SingleAsync(e => e.Id == id);
    }

    private async Task<long> StoreWithoutInterceptorAsync(TestEntity entity)
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(_databaseName)
            .Options;

        await using var context = new TestDbContext(options);
        context.TestEntities.Add(entity);
        await context.SaveChangesAsync();

        return entity.Id;
    }

    public class TestEntity
    {
        public long Id { get; set; }

        [RichText]
        public string Content { get; set; } = null!;

        [RichText]
        public string? OptionalContent { get; set; }

        public string? PlainText { get; set; }
    }

    public class TestDbContext(DbContextOptions<TestDbContext> options) : DbContext(options)
    {
        public DbSet<TestEntity> TestEntities => Set<TestEntity>();
    }
}
