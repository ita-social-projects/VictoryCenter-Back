namespace VictoryCenter.DAL.Enums;

/// <summary>
/// Values of the legacy Metrics.Type column, kept for rollback to the previous backend; remove in the follow-up release.
/// </summary>
public enum LegacyMetricType
{
    Partners = 0,
    Programs = 1,
    Raised = 2,
    TherapyHours = 3
}
