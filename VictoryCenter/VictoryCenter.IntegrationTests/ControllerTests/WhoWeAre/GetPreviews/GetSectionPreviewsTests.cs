using System.Net;
using System.Text.Json;
using VictoryCenter.BLL.DTOs.Admin.WhoWeAreSection;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.WhoWeAre.GetPreviews;

public class GetSectionPreviewsTests : BaseTestClass
{
    public GetSectionPreviewsTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task GetPreviews_ShouldReturnSectionPreviews()
    {
        var response = await Fixture.HttpClient.GetAsync("/api/WhoWeAre/previews");
        var responseString = await response.Content.ReadAsStringAsync();

        var responseContent = JsonSerializer.Deserialize<List<WhoWeAreSectionInfoDto>>(responseString, JsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(responseContent);
        Assert.True(responseContent.Count > 0);
        var sections = responseContent.Select(x => x.SectionType).ToList();
        Assert.True(sections.Any());
        Assert.Contains("Main", sections);
        Assert.Contains("WhatWeDo", sections);
        Assert.Contains("WhoWeSupport", sections);
        Assert.Contains("People", sections);
        Assert.Contains("Team", sections);
    }
}
