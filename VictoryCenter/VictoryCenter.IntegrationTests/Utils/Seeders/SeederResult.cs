namespace VictoryCenter.IntegrationTests.Utils.Seeders;

public class SeederResult
{
    public bool Success { get; set; }
    public int CreatedCount { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}
