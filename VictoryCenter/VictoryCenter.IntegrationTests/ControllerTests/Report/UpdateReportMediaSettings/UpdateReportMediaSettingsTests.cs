using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.ReportMediaSettings;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.Report.UpdateReportMediaSettings;
public class UpdateReportMediaSettingsTests : BaseTestClass
{
    private readonly Uri _endpointUri = new("/api/Report/report", UriKind.Relative);

    public UpdateReportMediaSettingsTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task UpdateReportMediaSettings_WithValidData_ShouldReturnOkAndUpdateEntity()
    {
        // Arrange
        var image1 = await Fixture.DbContext.Images.OrderBy(i => i.Id).FirstAsync();
        var image2 = await Fixture.DbContext.Images.OrderByDescending(i => i.Id).FirstAsync();

        var updateDto = new UpdateReportMediaSettingsDto
        {
            CollectedFundsBlock = new UpdateCollectedFundsBlockDto
            {
                Title = "Updated Funds Title",
                ImageId = image1.Id
            },
            ChangedLivesBlock = new UpdateChangedLivesBlockDto
            {
                Title = "Updated Lives Title",
                ChangedLives = 2500,
                ImageId = image2.Id
            }
        };

        var serializedDto = JsonSerializer.Serialize(updateDto);
        var content = new StringContent(serializedDto, Encoding.UTF8, "application/json");

        // Act
        var response = await Fixture.HttpClient.PutAsync(_endpointUri, content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Fixture.DbContext.ChangeTracker.Clear();

        var collectedFundsEntity = await Fixture.DbContext.CollectedFundsBlocks.FirstAsync();
        var changedLivesEntity = await Fixture.DbContext.ChangedLivesBlocks.FirstAsync();

        Assert.Equal(updateDto.CollectedFundsBlock.Title, collectedFundsEntity.Title);
        Assert.Equal(updateDto.CollectedFundsBlock.ImageId, collectedFundsEntity.ImageId);

        Assert.Equal(updateDto.ChangedLivesBlock.Title, changedLivesEntity.Title);
        Assert.Equal(updateDto.ChangedLivesBlock.ChangedLives, changedLivesEntity.ChangedLivesCount);
        Assert.Equal(updateDto.ChangedLivesBlock.ImageId, changedLivesEntity.ImageId);
    }

    [Fact]
    public async Task UpdateReportMediaSettings_WithNonExistentImageId_ShouldReturnNotFound()
    {
        // Arrange
        // Get the maximum existing image ID and use a non-existent ID
        var maxImageId = await Fixture.DbContext.Images.MaxAsync(i => (long?)i.Id) ?? 0;
        var nonExistentImageId = maxImageId + 1000; // Use a large non-existent ID

        var updateDto = new UpdateReportMediaSettingsDto
        {
            CollectedFundsBlock = new UpdateCollectedFundsBlockDto
            {
                Title = "Funds Title",
                ImageId = nonExistentImageId
            },
            ChangedLivesBlock = new UpdateChangedLivesBlockDto
            {
                Title = "Lives Title",
                ChangedLives = 100,
                ImageId = nonExistentImageId
            }
        };

        var serializedDto = JsonSerializer.Serialize(updateDto);
        var content = new StringContent(serializedDto, Encoding.UTF8, "application/json");

        // Act
        var response = await Fixture.HttpClient.PutAsync(_endpointUri, content);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateReportMediaSettings_WithEmptyTitle_ShouldReturnBadRequest()
    {
        // Arrange
        var image = await Fixture.DbContext.Images.FirstAsync();

        var updateDto = new UpdateReportMediaSettingsDto
        {
            CollectedFundsBlock = new UpdateCollectedFundsBlockDto
            {
                Title = "",
                ImageId = image.Id
            },
            ChangedLivesBlock = new UpdateChangedLivesBlockDto
            {
                Title = "Valid",
                ChangedLives = 100,
                ImageId = image.Id
            }
        };

        var serializedDto = JsonSerializer.Serialize(updateDto);
        var content = new StringContent(serializedDto, Encoding.UTF8, "application/json");

        // Act
        var response = await Fixture.HttpClient.PutAsync(_endpointUri, content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
