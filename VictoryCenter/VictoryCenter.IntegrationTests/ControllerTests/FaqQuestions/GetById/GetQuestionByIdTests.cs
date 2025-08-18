using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.TeamMembers;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.FaqQuestions.GetById;

public class GetQuestionByIdTests : BaseTestClass
{
    public GetQuestionByIdTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task GetFaqQuestionById_ShouldReturnOk()
    {
        // Arrange
        var existingEntity = await Fixture.DbContext.FaqQuestions.FirstOrDefaultAsync()
            ?? throw new InvalidOperationException("Couldn't setup existing entity");

        // Act
        var response = await Fixture.HttpClient.GetAsync(new Uri($"api/faq/{existingEntity!.Id}", UriKind.Relative));
        var responseString = await response.Content.ReadAsStringAsync();

        var responseContent = JsonSerializer.Deserialize<TeamMemberDto>(responseString, JsonOptions);

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(responseContent);
    }

    [Fact]
    public async Task GetTeamMemberById_ShouldFail_NotFound()
    {
        var response = await Fixture.HttpClient.GetAsync(new Uri($"api/faq/{-1}", UriKind.Relative));

        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
