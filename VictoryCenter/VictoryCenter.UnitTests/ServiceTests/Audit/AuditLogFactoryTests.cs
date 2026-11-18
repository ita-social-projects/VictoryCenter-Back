using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Moq;
using VictoryCenter.BLL.Interfaces.Audit;
using VictoryCenter.BLL.Services.Audit;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.UnitTests.ServiceTests.Audit;

public class AuditLogFactoryTests
{
    private static readonly DateTimeOffset Now = new(2026, 11, 13, 10, 30, 0, TimeSpan.Zero);

    private readonly Mock<IHttpContextAccessor> _httpContextAccessor = new();
    private readonly AuditLogFactory _factory;

    public AuditLogFactoryTests()
    {
        var timeProvider = new Mock<TimeProvider>();
        timeProvider.Setup(t => t.GetUtcNow()).Returns(Now);
        _factory = new AuditLogFactory(_httpContextAccessor.Object, timeProvider.Object);
    }

    [Fact]
    public void Create_WithAuthenticatedRequest_ShouldSetActorCorrelationIdAndEntityData()
    {
        // Arrange
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, "7")])),
            TraceIdentifier = "trace-1",
        };
        _httpContextAccessor.Setup(a => a.HttpContext).Returns(httpContext);

        // Act
        var log = _factory.Create("FaqQuestion", "42", AuditOperation.Update, null);

        // Assert
        Assert.Equal("7", log.ActorId);
        Assert.Equal("trace-1", log.CorrelationId);
        Assert.Equal(AuditOperation.Update, log.Operation);
        Assert.Equal("FaqQuestion", log.EntityType);
        Assert.Equal("42", log.EntityId);
        Assert.Equal(Now, log.Timestamp);
    }

    [Fact]
    public void Create_WithoutNameIdentifierClaim_ShouldLeaveActorNull()
    {
        // Arrange
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.Email, "admin@gmail.com")])),
            TraceIdentifier = "trace-2",
        };
        _httpContextAccessor.Setup(a => a.HttpContext).Returns(httpContext);

        // Act
        var log = _factory.Create("FaqQuestion", "42", AuditOperation.Update, null);

        // Assert
        Assert.Null(log.ActorId);
        Assert.Equal("trace-2", log.CorrelationId);
    }

    [Fact]
    public void Create_WithoutHttpContext_ShouldLeaveActorAndCorrelationIdNull()
    {
        // Arrange
        _httpContextAccessor.Setup(a => a.HttpContext).Returns((HttpContext?)null);

        // Act
        var log = _factory.Create("FaqQuestion", "42", AuditOperation.Delete, null);

        // Assert
        Assert.Null(log.ActorId);
        Assert.Null(log.CorrelationId);
    }

    [Fact]
    public void Create_WithEmptyChanges_ShouldSetChangesNull()
    {
        // Act
        var log = _factory.Create("FaqQuestion", "42", AuditOperation.Update, new Dictionary<string, AuditChange>());

        // Assert
        Assert.Null(log.Changes);
    }

    [Fact]
    public void Create_WithChanges_ShouldWriteEnumNamesAndUnescapedText()
    {
        // Arrange
        var changes = new Dictionary<string, AuditChange>
        {
            ["Status"] = new AuditChange(Status.Draft, Status.Published),
            ["Title"] = new AuditChange("Старий", "<p>Новий</p>"),
        };

        // Act
        var log = _factory.Create("FaqQuestion", "42", AuditOperation.Update, changes);

        // Assert
        Assert.Equal(
            "{\"Status\":{\"Old\":\"Draft\",\"New\":\"Published\"},\"Title\":{\"Old\":\"Старий\",\"New\":\"<p>Новий</p>\"}}",
            log.Changes);
    }
}
