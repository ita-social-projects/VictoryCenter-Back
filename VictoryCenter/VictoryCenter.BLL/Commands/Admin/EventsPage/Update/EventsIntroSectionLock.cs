namespace VictoryCenter.BLL.Commands.Admin.EventsPage.Update;

internal static class EventsIntroSectionLock
{
    internal static readonly SemaphoreSlim Semaphore = new(1, 1);
}
