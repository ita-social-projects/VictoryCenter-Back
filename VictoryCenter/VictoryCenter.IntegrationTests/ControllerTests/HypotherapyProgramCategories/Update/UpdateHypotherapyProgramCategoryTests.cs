using System.Net;
using System.Text;
using Newtonsoft.Json;
using VictoryCenter.BLL.DTOs.Admin.HypotherapyProgramCategories;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.HypotherapyProgramCategories.Update;

public class UpdateHypotherapyProgramCategoryTests : BaseTestClass
{
    public UpdateHypotherapyProgramCategoryTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task UpdateProgramCategory_ShouldUpdateProgramCategory()
    {
        var updateProgramDto = new UpdateHypotherapyProgramCategoryDto
        {
            Name = "UpdatedName"
        };
        var serializedDto = JsonConvert.SerializeObject(updateProgramDto);

        HttpResponseMessage response = await Fixture.HttpClient.PutAsync("/api/HypotherapyProgramCategory/1", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();

        HypotherapyProgramCategoryDto? responseContent = JsonConvert.DeserializeObject<HypotherapyProgramCategoryDto>(responseString);

        Assert.NotNull(responseContent);
        Assert.Equal(updateProgramDto.Name, responseContent.Name);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task UpdateProgramCategory_ShouldNotUpdateProgramCategory_InvalidName(string? name)
    {
        var updateProgramCategoryDto = new UpdateHypotherapyProgramCategoryDto
        {
            Name = name!
        };

        var serializedDto = JsonConvert.SerializeObject(updateProgramCategoryDto);

        HttpResponseMessage response = await Fixture.HttpClient.PutAsync("/api/HypotherapyProgramCategory/1", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task ProgramCategory_ShouldNotUpdateProgramCategory_NotFound(int id)
    {
        var updateProgramCategoryDto = new UpdateHypotherapyProgramCategoryDto
        {
            Name = "UpdatedName"
        };
        var serializedDto = JsonConvert.SerializeObject(updateProgramCategoryDto);

        HttpResponseMessage response = await Fixture.HttpClient.PutAsync($"/api/HypotherapyProgramCategory/{id}", new StringContent(
            serializedDto, Encoding.UTF8, "application/json"));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
