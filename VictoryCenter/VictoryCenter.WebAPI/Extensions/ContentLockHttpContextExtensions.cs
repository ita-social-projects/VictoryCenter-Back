using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Concurrency.WebSocket;

namespace VictoryCenter.WebAPI.Extensions;

public static class ContentLockHttpContextExtensions
{
    public static ContentLockSessionDto? GetContentLockSessionOrDefault(this HttpContext httpContext)
    {
        var session = httpContext.Items[ContentLockConstants.SessionItemKey];
        return session as ContentLockSessionDto;
    }
}
