using System.ComponentModel.DataAnnotations;

namespace VictoryCenter.BLL.Options;

public class AccountLockoutOptions
{
    public static readonly string Position = "AccountLockout";

    [Range(1, int.MaxValue)]
    public int MaxFailedAccessAttempts { get; init; }

    [Range(1, int.MaxValue)]
    public int LockoutMinutes { get; init; }
}
