namespace VictoryCenter.BLL.Interfaces.MainPage;

public interface IMetricVisibilityService
{
    Task ToggleMetricVisibilityAsync(long id, bool isHidden);
}
