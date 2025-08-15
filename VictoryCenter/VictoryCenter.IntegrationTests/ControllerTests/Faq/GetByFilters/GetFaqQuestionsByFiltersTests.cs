using System.Text.Json;
using VictoryCenter.BLL.DTOs.Admin.FaqQuestions;
using VictoryCenter.IntegrationTests.ControllerTests.DbFixture;
using VictoryCenter.IntegrationTests.Utils;

namespace VictoryCenter.IntegrationTests.ControllerTests.Faq.GetByFilters;

public class GetFaqQuestionsByFiltersTests : BaseTestClass
{
    public GetFaqQuestionsByFiltersTests(IntegrationTestDbFixture Fixture)
        : base(Fixture)
    {
    }

    [Fact]
    public async Task GetFilteredQuestions_ShouldReturnFaqQuestions()
    {
        var response = await Fixture.HttpClient.GetAsync(new Uri("/api/faq/pages", UriKind.Relative));
        var responseString = await response.Content.ReadAsStringAsync();
        var responseContent = JsonSerializer.Deserialize<List<FaqQuestionDto>>(
            responseString,
            JsonOptions);

        response.EnsureSuccessStatusCode();
        Assert.NotNull(responseContent);
        Assert.IsType<List<FaqQuestionDto>>(responseContent);
        Assert.NotEmpty(responseContent);
    }
}
