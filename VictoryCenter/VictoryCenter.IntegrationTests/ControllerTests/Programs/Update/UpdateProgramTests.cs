using System.Text;
using Newtonsoft.Json;
using VictoryCenter.BLL.DTOs.Programs;
using VictoryCenter.IntegrationTests.ControllerTests.Base;
using VictoryCenter.IntegrationTests.Utils.Seeder;

namespace VictoryCenter.IntegrationTests.ControllerTests.Programs.Update;

[Collection("SharedIntegrationTests")]
public class UpdateProgramTests : IAsyncLifetime
{
    private readonly HttpClient _httpClient;
    private readonly SeederManager _seederManager;

    public UpdateProgramTests(IntegrationTestDbFixture fixture)
    {
        _httpClient = fixture.HttpClient;
        _seederManager = fixture.SeederManager ?? throw new InvalidOperationException(
            "SeederManager is not registered in the service collection.");
    }

    public async Task InitializeAsync()
    {
        await _seederManager.DisposeAllAsync();
        await _seederManager.SeedAllAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task UpdatePublishedProgram_ShouldUpdateProgram()
    {
        var updateProgramDto = new UpdateProgramDto
        {
            Id = 1,
            Name = "UpdatedName",
            Description = "UpdatedDescription",
            ImageId = 1,
            CategoriesId = [1, 4]
        };

        var serializedDto = JsonConvert.SerializeObject(updateProgramDto);

        var response = await _httpClient.PutAsync("/api/Program/", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        var responseString = await response.Content.ReadAsStringAsync();

        var responseContent = JsonConvert.DeserializeObject<ProgramDto>(responseString);
        response.EnsureSuccessStatusCode();

        Assert.NotNull(responseContent);
        Assert.Equal(updateProgramDto.Name, responseContent.Name);
        Assert.Equal(updateProgramDto.Description, responseContent.Description);
    }

    // [Theory]
    // [InlineData(null)]
    // [InlineData("")]
    // [InlineData(" ")]
    // public async Task UpdateProgramWithInvalidName_ShouldReturnBadRequest(string? invalidName)
    // {
    //     var updateProgramDto = new UpdateProgramDto
    //     {
    //         Id = 1,
    //         Name = invalidName,
    //         Description = "UpdatedDescription",
    //         Status = Status.Published,
    //         ImageId = 2,
    //         CategoriesId = [1, 4]
    //     };
    // }
}
