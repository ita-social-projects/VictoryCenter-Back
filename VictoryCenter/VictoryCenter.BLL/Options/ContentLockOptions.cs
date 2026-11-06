using System.ComponentModel.DataAnnotations;

namespace VictoryCenter.BLL.Options;

public sealed class ContentLockOptions
{
    public const string Position = "ContentLock";

    [Range(1, 60)]
    public int TtlMinutes { get; init; } = 3;

    public bool EnforcementEnabled { get; init; }

    [Range(1, 300)]
    public int RenewalIntervalSeconds { get; init; } = 30;

    [Range(1, 300)]
    public int ExpirationScanIntervalSeconds { get; init; } = 30;

    public TimeSpan Ttl => TimeSpan.FromMinutes(TtlMinutes);
}
