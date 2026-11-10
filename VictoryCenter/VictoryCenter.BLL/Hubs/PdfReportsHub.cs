using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace VictoryCenter.BLL.Hubs;

/// <summary>
/// Sends live PDF report updates to the public website, so the reports list refreshes
/// without a page reload when an admin creates, updates, deletes or reorders reports.
/// </summary>
/// <remarks>
/// Anonymous access is intentional: the audience is public site visitors, who have no accounts.
/// The hub is server-to-client only and sends only data that the public PdfReports API already
/// returns to anonymous users. Never broadcast admin-only data through this hub.
/// See docs/107-Realtime-Hubs.md.
/// </remarks>
[AllowAnonymous]
public class PdfReportsHub : Hub
{
}
