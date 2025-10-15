using System.Net;
using System.Text;
using Newtonsoft.Json;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyProgramCategories;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.HippotherapyProgramCategories.Update;

public class UpdateHippotherapyProgramCategoryTests : BaseTestClass
{
    public UpdateHippotherapyProgramCategoryTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task UpdateProgramCategory_ShouldUpdateProgramCategory()
    {
        var updateProgramDto = new UpdateHippotherapyProgramCategoryDto
        {
            Name = "UpdatedName"
        };
        var serializedDto = JsonConvert.SerializeObject(updateProgramDto);

        HttpResponseMessage response = await Fixture.HttpClient.PutAsync("/api/HippotherapyProgramCategory/1", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();

        HippotherapyProgramCategoryDto? responseContent = JsonConvert.DeserializeObject<HippotherapyProgramCategoryDto>(responseString);

        Assert.NotNull(responseContent);
        Assert.Equal(updateProgramDto.Name, responseContent.Name);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task UpdateProgramCategory_ShouldNotUpdateProgramCategory_InvalidName(string? name)
    {
        var updateProgramCategoryDto = new UpdateHippotherapyProgramCategoryDto
        {
            Name = name!
        };

        var serializedDto = JsonConvert.SerializeObject(updateProgramCategoryDto);

        HttpResponseMessage response = await Fixture.HttpClient.PutAsync("/api/HippotherapyProgramCategory/1", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task ProgramCategory_ShouldNotUpdateProgramCategory_NotFound(int id)
    {
        var updateProgramCategoryDto = new UpdateHippotherapyProgramCategoryDto
        {
            Name = "UpdatedName"
        };
        var serializedDto = JsonConvert.SerializeObject(updateProgramCategoryDto);

        HttpResponseMessage response = await Fixture.HttpClient.PutAsync($"/api/HippotherapyProgramCategory/{id}", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
