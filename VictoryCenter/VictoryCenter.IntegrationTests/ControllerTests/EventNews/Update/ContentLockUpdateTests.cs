using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.EventNews;
using VictoryCenter.BLL.Interfaces.Concurrency;
using VictoryCenter.DAL.Enums;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.EventNews.Update;

public class ContentLockUpdateTests : BaseTestClass
{
    private const string AdminEmail = "testadmin@victorycenter.com";

    public ContentLockUpdateTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task EventNewsUpdate_WithoutOrWithInvalidLock_ReturnsLockedAndDoesNotChangeEntity()
    {
        var originalStatus = await Fixture.DbContext.EventNews
            .Where(item => item.Id == 1)
            .Select(item => item.Status)
            .SingleAsync();
        var update = new UpdateEventNewsDto { Status = Status.Draft, CategoryId = 1 };

        var missingLockResponse = await Fixture.HttpClient.PutAsJsonAsync("/api/EventNews/1", update);
        using var invalidLockRequest = CreateLockedRequest("/api/EventNews/1", update, "not-a-lock", "missing-connection");
        var invalidLockResponse = await Fixture.HttpClient.SendAsync(invalidLockRequest);

        Assert.Equal(HttpStatusCode.Locked, missingLockResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Locked, invalidLockResponse.StatusCode);
        Fixture.DbContext.ChangeTracker.Clear();
        Assert.Equal(originalStatus, await Fixture.DbContext.EventNews.Where(item => item.Id == 1).Select(item => item.Status).SingleAsync());
    }

    [Fact]
    public async Task EventNewsUpdate_WithValidLock_SucceedsAndReleasesLock()
    {
        const string connectionId = "integration-content-lock-session";
        var resource = new ContentLockResourceDto
        {
            ResourceType = ContentLockConstants.EventNewsResourceType,
            ResourceId = 1,
        };
        var manager = Fixture.Factory.Services.GetRequiredService<IContentLockManager>();
        var acquisition = manager.Acquire(resource, AdminEmail, connectionId);
        Assert.True(acquisition.Acquired);

        using var request = CreateLockedRequest(
            "/api/EventNews/1",
            new UpdateEventNewsDto { Status = Status.Draft, CategoryId = 1 },
            acquisition.LockId!,
            connectionId);
        var response = await Fixture.HttpClient.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.False(manager.GetStatus(resource).IsLocked);
    }

    private static HttpRequestMessage CreateLockedRequest<T>(string uri, T body, string lockId, string connectionId)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, uri)
        {
            Content = JsonContent.Create(body)
        };
        request.Headers.Add(ContentLockConstants.LockIdHeader, lockId);
        request.Headers.Add(ContentLockConstants.ConnectionIdHeader, connectionId);
        return request;
    }
}
