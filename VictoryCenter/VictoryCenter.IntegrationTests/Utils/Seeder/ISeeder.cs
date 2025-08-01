namespace VictoryCenter.IntegrationTests.Utils.Seeder;

public interface ISeeder
{
    int Order { get; }
    string Name { get; }
    Task<SeederResult> SeedAsync();
    Task<bool> DisposeAsync();
    Task<bool> VerifyAsync();
}
