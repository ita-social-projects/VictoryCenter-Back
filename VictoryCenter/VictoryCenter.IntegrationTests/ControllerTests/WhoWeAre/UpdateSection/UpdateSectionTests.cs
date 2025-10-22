using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.WhoWeAreContent;
using VictoryCenter.BLL.DTOs.Admin.WhoWeAreSection;
using VictoryCenter.BLL.DTOs.Common.WhoWeAreContent;
using VictoryCenter.DAL.Enums;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.WhoWeAre.UpdateSection;

public class UpdateSectionTests : BaseTestClass
{
    public UpdateSectionTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task UpdateSection_ValidRequest_ShouldUpdateSection()
    {
        var section = await Fixture.DbContext.WhoWeAreSections
                          .Include(s => s.Contents)
                          .FirstOrDefaultAsync(s => s.SectionType == SectionType.Main) ??
                      throw new InvalidOperationException("Main section does not exist in the database.");

        var contentId = section.Contents.FirstOrDefault(c => c.ContentType == ContentType.Description)?.Id ??
                        throw new InvalidOperationException("Description content does not exist in the database.");

        var contentToUpdate = new List<UpdateWhoWeAreContentDto>
        {
            new()
            {
                Id = contentId,
                ContentType = ContentType.Description,
                Description = "Updated description",
            }
        };

        var serializedDto = JsonSerializer.Serialize(contentToUpdate);

        var response = await Fixture.HttpClient.PutAsync(
            $"api/WhoWeAre/{(int)SectionType.Main}",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        var responseString = await response.Content.ReadAsStringAsync();
        var responseContent = JsonSerializer.Deserialize<WhoWeAreSectionDto>(responseString, JsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(responseContent);
        var updatedDescription =
            (responseContent.Contents.FirstOrDefault(x => x.ContentType == ContentType.Description) as
                DescriptionContentDto)?.Description;
        Assert.Equal(contentToUpdate.First().Description, updatedDescription);
    }

    [Fact]
    public async Task UpdateSection_ContentNotFound_ShouldReturnNotFoundRequest()
    {
        var section = await Fixture.DbContext.WhoWeAreSections
                          .FirstOrDefaultAsync(s => s.SectionType == SectionType.Main)
                      ?? throw new InvalidOperationException("Main section does not exist in the database.");

        var fakeContentId = -1;

        var contentToUpdate = new List<UpdateWhoWeAreContentDto>
        {
            new()
            {
                Id = fakeContentId,
                ContentType = ContentType.Description,
                Description = "Invalid update"
            }
        };

        var serializedDto = JsonSerializer.Serialize(contentToUpdate, JsonOptions);

        var response = await Fixture.HttpClient.PutAsync(
            $"api/WhoWeAre/{(int)section.SectionType}",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateSection_WrongSection_ShouldReturnBadRequest()
    {
        var section = await Fixture.DbContext.WhoWeAreSections
                          .Include(s => s.Contents)
                          .FirstOrDefaultAsync(s => s.SectionType == SectionType.Main)
                      ?? throw new InvalidOperationException("Main section does not exist in the database.");

        var otherSection = await Fixture.DbContext.WhoWeAreSections
                               .FirstOrDefaultAsync(s => s.SectionType != SectionType.Main)
                           ?? throw new InvalidOperationException("Other section does not exist in the database.");

        var contentId = section.Contents.FirstOrDefault()?.Id
                        ?? throw new InvalidOperationException("Section does not contain any content.");

        var contentToUpdate = new List<UpdateWhoWeAreContentDto>
        {
            new()
            {
                Id = contentId,
                ContentType = ContentType.Description,
                Description = "Try assign to wrong section"
            }
        };

        var serializedDto = JsonSerializer.Serialize(contentToUpdate, JsonOptions);

        var response = await Fixture.HttpClient.PutAsync(
            $"api/WhoWeAre/{(int)otherSection.SectionType}",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateSection_WrongContentType_ShouldReturnBadRequest()
    {
        var section = await Fixture.DbContext.WhoWeAreSections
                          .Include(s => s.Contents)
                          .FirstOrDefaultAsync(s => s.SectionType == SectionType.Main)
                      ?? throw new InvalidOperationException("Main section does not exist in the database.");

        var content = section.Contents.FirstOrDefault()
                      ?? throw new InvalidOperationException("Section does not contain any content.");

        var contentToUpdate = new List<UpdateWhoWeAreContentDto>
        {
            new()
            {
                Id = content.Id,
                ContentType = ContentType.Image,
                Description = "Wrong type"
            }
        };

        var serializedDto = JsonSerializer.Serialize(contentToUpdate, JsonOptions);

        var response = await Fixture.HttpClient.PutAsync(
            $"api/WhoWeAre/{(int)section.SectionType}",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateSection_InvalidSectionType_ShouldReturnBadRequest()
    {
        var section = await Fixture.DbContext.WhoWeAreSections
                          .Include(whoWeAreSection => whoWeAreSection.Contents)
                          .FirstOrDefaultAsync(s => s.SectionType == SectionType.Main)
                      ?? throw new InvalidOperationException("Main section does not exist in the database.");

        var invalidSectionType = 999;

        var contentId = section.Contents.FirstOrDefault()?.Id
                        ?? throw new InvalidOperationException("Section does not contain any content.");

        var contentToUpdate = new List<UpdateWhoWeAreContentDto>
        {
            new()
            {
                Id = contentId,
                ContentType = ContentType.Description,
                Description = "Invalid section"
            }
        };

        var serializedDto = JsonSerializer.Serialize(contentToUpdate, JsonOptions);

        var response = await Fixture.HttpClient.PutAsync(
            $"api/WhoWeAre/{invalidSectionType}",
            new StringContent(serializedDto, Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
