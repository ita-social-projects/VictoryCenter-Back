using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Partners;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.Partners.UpdateBanner;

public class UpdatePartnersBannerTests : BaseTestClass
{
    private readonly Uri _endpointUri = new("/api/Partners/banner", UriKind.Relative);

    public UpdatePartnersBannerTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task UpdateBanner_WithValidData_ShouldReturnOkAndUpdateEntity()
    {
        // Arrange
        var newImage = await Fixture.DbContext.Images.OrderBy(i => i.Id).LastOrDefaultAsync()
            ?? throw new InvalidOperationException("Not enough images seeded for the test.");

        var updateDto = new UpdatePartnersPageBannerDto
        {
            Title = "Updated Banner Title",
            Description = "This is the new and updated description.",
            ImageId = newImage.Id
        };
        var serializedDto = JsonSerializer.Serialize(updateDto);
        var content = new StringContent(serializedDto, Encoding.UTF8, "application/json");

        // Act
        var response = await Fixture.HttpClient.PutAsync(_endpointUri, content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Fixture.DbContext.ChangeTracker.Clear();
        var updatedBannerInDb = await Fixture.DbContext.PartnersPageBanners.FirstAsync();
        Assert.Equal(updateDto.Title, updatedBannerInDb.Title);
        Assert.Equal(updateDto.Description, updatedBannerInDb.Description);
        Assert.Equal(updateDto.ImageId, updatedBannerInDb.ImageId);
    }

    [Fact]
    public async Task UpdateBanner_WithInvalidData_ShouldReturnBadRequest()
    {
        // Arrange
        var updateDto = new UpdatePartnersPageBannerDto
        {
            Title = new string('T', PartnerConstants.PartnersPageBannerTitleMaxLength + 1), // Invalid title
            Description = "Valid Description",
            ImageId = 1
        };
        var serializedDto = JsonSerializer.Serialize(updateDto);
        var content = new StringContent(serializedDto, Encoding.UTF8, "application/json");

        // Act
        var response = await Fixture.HttpClient.PutAsync(_endpointUri, content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateBanner_WithNonExistentImageId_ShouldReturnNotFound()
    {
        // Arrange
        var updateDto = new UpdatePartnersPageBannerDto
        {
            Title = "Valid Title",
            Description = "Valid Description",
            ImageId = long.MaxValue // Non-existent Image ID
        };
        var serializedDto = JsonSerializer.Serialize(updateDto);
        var content = new StringContent(serializedDto, Encoding.UTF8, "application/json");

        // Act
        var response = await Fixture.HttpClient.PutAsync(_endpointUri, content);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
