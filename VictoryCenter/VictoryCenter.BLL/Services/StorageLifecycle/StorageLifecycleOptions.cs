using System.ComponentModel.DataAnnotations;

namespace VictoryCenter.BLL.Services.StorageLifecycle;

public class StorageLifecycleOptions
{
    [Range(1, 3600)]
    public int PollIntervalSeconds { get; set; } = 15;

    [Range(1, 200)]
    public int BatchSize { get; set; } = 25;

    [Range(1, 100)]
    public int MaxAttempts { get; set; } = 8;

    [Range(30, 3600)]
    public int LeaseSeconds { get; set; } = 300;

    [Range(1, 168)]
    public int UploadReservationHours { get; set; } = 24;

    [Range(1, 168)]
    public int OrphanMinimumAgeHours { get; set; } = 24;

    [Range(1, 1440)]
    public int ReconciliationIntervalMinutes { get; set; } = 60;

    [Range(1, 3600)]
    public int RetryDelaySeconds { get; set; } = 5;

    [Range(1, 86400)]
    public int MaxRetryDelaySeconds { get; set; } = 3600;
}
