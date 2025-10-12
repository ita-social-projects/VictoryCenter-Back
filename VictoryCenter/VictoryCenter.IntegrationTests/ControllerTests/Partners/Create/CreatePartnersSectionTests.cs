using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Partners;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.Partners.Create;

public class CreatePartnersSectionTests : BaseTestClass
{
    private readonly Uri _endpointUri = new("/api/Partners", UriKind.Relative);

    public CreatePartnersSectionTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task CreatePartnersSection_WithValidDto_ShouldReturnOk()
    {
        // Arrange
        var images = await Fixture.DbContext.Images.Take(2).ToListAsync();
        if (images.Count < 2)
        {
            throw new InvalidOperationException("Seeder must provide at least 2 images for this test.");
        }

        var createDto = new CreatePartnersSectionDto
        {
            Title = "Newly Created Partner Section",
            Description = "A valid description for our new partners.",
            Partners =
            [
                new()
                {
                    Description = "First Partner",
                    ImageId = images[0].Id
                },
                new()
                {
                    Description = "Second Partner",
                    ImageId = images[1].Id
                }

            ]
        };
        var serializedDto = JsonSerializer.Serialize(createDto);
        var content = new StringContent(serializedDto, Encoding.UTF8, "application/json");

        // Act
        var response = await Fixture.HttpClient.PostAsync(_endpointUri, content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var createdSectionInDb = await Fixture.DbContext.PartnersSections
            .AsNoTracking()
            .Include(s => s.Partners)
            .FirstOrDefaultAsync(s => s.Title == createDto.Title);

        Assert.NotNull(createdSectionInDb);
        Assert.Equal(2, createdSectionInDb.Partners.Count);
    }

    [Fact]
    public async Task CreatePartnersSection_WithInvalidDto_ShouldReturnBadRequest()
    {
        // Arrange
        // Invalid title
        var createDto = new CreatePartnersSectionDto
        {
            Title = new string('T', PartnerConstants.PartnersSectionTitleMaxLength + 1),
            Description = "A valid description.",
            Partners = []
        };
        var serializedDto = JsonSerializer.Serialize(createDto);
        var content = new StringContent(serializedDto, Encoding.UTF8, "application/json");

        // Act
        var response = await Fixture.HttpClient.PostAsync(_endpointUri, content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreatePartnersSection_WithNonExistentImageId_ShouldReturnNotFound()
    {
        // Arrange
        var createDto = new CreatePartnersSectionDto
        {
            Title = "Section with a Fake Image Partner",
            Description = "This section references an image that does not exist.",
            Partners =
            [
                new()
                {
                    Description = "Partner with non-existent image",
                    ImageId = long.MaxValue
                }

            ]
        };
        var serializedDto = JsonSerializer.Serialize(createDto);
        var content = new StringContent(serializedDto, Encoding.UTF8, "application/json");

        // Act
        var response = await Fixture.HttpClient.PostAsync(_endpointUri, content);
        var responseContent = await response.Content.ReadAsStringAsync();
        var problemDetails = JsonSerializer.Deserialize<ProblemDetails>(responseContent, JsonOptions);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(problemDetails);
    }
}
