using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Interfaces.Concurrency;

namespace VictoryCenter.IntegrationTests.Utils;

public static class ContentLockTestHelper
{
    private const string TestAdminEmail = "testadmin@victorycenter.com";

    public static async Task<HttpResponseMessage> PutEventNewsAsync<T>(
        HttpClient client,
        IServiceProvider serviceProvider,
        long eventNewsId,
        string uri,
        T body)
    {
        var manager = serviceProvider.GetRequiredService<IContentLockManager>();
        var resource = new ContentLockResourceDto
        {
            ResourceType = ContentLockConstants.EventNewsResourceType,
            ResourceId = eventNewsId,
        };
        var connectionId = Guid.NewGuid().ToString("N");
        var acquisition = manager.Acquire(resource, TestAdminEmail, connectionId);
        if (!acquisition.Acquired || acquisition.LockId is null)
        {
            throw new InvalidOperationException("Unable to acquire a test content lock.");
        }

        using var request = new HttpRequestMessage(HttpMethod.Put, uri)
        {
            Content = JsonContent.Create(body)
        };
        request.Headers.Add(ContentLockConstants.LockIdHeader, acquisition.LockId);
        request.Headers.Add(ContentLockConstants.ConnectionIdHeader, connectionId);

        try
        {
            return await client.SendAsync(request);
        }
        finally
        {
            manager.Release(acquisition.LockId, resource, TestAdminEmail, connectionId);
        }
    }
}
