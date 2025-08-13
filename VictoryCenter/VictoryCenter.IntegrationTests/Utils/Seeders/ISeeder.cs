namespace VictoryCenter.IntegrationTests.Utils.Seeders;

public interface ISeeder
{
    int Order { get; }
    string Name { get; }
    Task<SeederResult> SeedAsync();
    Task<bool> DisposeAsync();
    Task<bool> VerifyAsync();
}
