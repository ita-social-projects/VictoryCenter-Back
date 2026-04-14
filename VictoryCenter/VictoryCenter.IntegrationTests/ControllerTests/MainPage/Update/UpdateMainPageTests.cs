using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics.Metrics;
using VictoryCenter.BLL.DTOs.Admin.MainAboutUs;
using VictoryCenter.BLL.DTOs.Admin.MainPages;
using VictoryCenter.BLL.DTOs.Admin.MainPartners;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.MainPage.Update;

public class UpdateMainPageTests : BaseTestClass
{
    private readonly Uri _endpointUri = new("/api/MainPage", UriKind.Relative);

    public UpdateMainPageTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task UpdateMainPage_WithValidData_ShouldReturnOkAndUpdateEntity()
    {
        // Arrange
        var mainPage = await Fixture.DbContext.MainPages
            .Include(m => m.ImpactStatistics)
                .ThenInclude(s => s.Metrics)
            .FirstAsync();

        var image1 = await Fixture.DbContext.Images.OrderBy(i => i.Id).FirstAsync();
        var image2 = await Fixture.DbContext.Images.OrderByDescending(i => i.Id).FirstAsync();

        var existingStat = mainPage.ImpactStatistics.FirstOrDefault();
        var existingMetric = existingStat?.Metrics.FirstOrDefault();

        var updateDto = new UpdateMainPageDto
        {
            Title = "Updated MainPage title",
            Description = "Updated MainPage description",
            ImageId = image1.Id,
            MainAboutUs = new UpdateMainAboutUsDto
            {
                Title = "Updated About Us title",
                Description = "Updated About Us description",
            },
            MainPartners = new UpdateMainPartnersDto
            {
                Title = "Updated Partners title",
                Description = "Updated Partners description",
            },
            ImpactStatistics =
            [
                new UpdateImpactStatisticDto
                {
                    Id = existingStat?.Id,
                    Description = "Updated existing stat",
                    ImageId = image2.Id,
                    Metrics =
                    [
                        new UpdateMetricDto
                        {
                            Id = existingMetric?.Id,
                            Value = "999",
                            Signature = "updated-signature",
                        },
                        new UpdateMetricDto
                        {
                            Value = "123",
                            Signature = "new-metric",
                        },
                    ],
                },
                new UpdateImpactStatisticDto
                {
                    Description = "New stat",
                    ImageId = image2.Id,
                    Metrics =
                    [
                        new UpdateMetricDto
                        {
                            Value = "321",
                            Signature = "new-stat-metric",
                        },
                    ],
                },
            ],
        };

        var content = new StringContent(
            JsonSerializer.Serialize(updateDto),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await Fixture.HttpClient.PutAsync(_endpointUri, content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Fixture.DbContext.ChangeTracker.Clear();

        var updatedMainPage = await Fixture.DbContext.MainPages
            .Include(m => m.MainAboutUs)
            .Include(m => m.MainPartners)
            .Include(m => m.ImpactStatistics)
                .ThenInclude(s => s.Metrics)
            .FirstAsync();

        Assert.Equal(updateDto.Title, updatedMainPage.Title);
        Assert.Equal(updateDto.Description, updatedMainPage.Description);
        Assert.Equal(updateDto.ImageId, updatedMainPage.ImageId);

        Assert.NotNull(updatedMainPage.MainAboutUs);
        Assert.Equal(updateDto.MainAboutUs!.Title, updatedMainPage.MainAboutUs!.Title);
        Assert.Equal(updateDto.MainAboutUs.Description, updatedMainPage.MainAboutUs.Description);

        Assert.NotNull(updatedMainPage.MainPartners);
        Assert.Equal(updateDto.MainPartners!.Title, updatedMainPage.MainPartners!.Title);
        Assert.Equal(updateDto.MainPartners.Description, updatedMainPage.MainPartners.Description);

        Assert.True(updatedMainPage.ImpactStatistics.Count >= 1);
    }

    [Fact]
    public async Task UpdateMainPage_WithEmptyTitle_ShouldReturnBadRequest()
    {
        // Arrange
        var updateDto = new UpdateMainPageDto
        {
            Title = string.Empty,
            Description = "Valid description",
            ImpactStatistics =
            [
                new UpdateImpactStatisticDto
                {
                    Description = "Valid stat description",
                    Metrics =
                    [
                        new UpdateMetricDto
                        {
                            Value = "100",
                            Signature = "children",
                        },
                    ],
                },
            ],
        };

        var content = new StringContent(
            JsonSerializer.Serialize(updateDto),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await Fixture.HttpClient.PutAsync(_endpointUri, content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateMainPage_WithNonExistentImageId_ShouldReturnNotFound()
    {
        // Arrange
        var mainPage = await Fixture.DbContext.MainPages
            .Include(m => m.ImpactStatistics)
                .ThenInclude(s => s.Metrics)
            .FirstAsync();

        var existingStat = mainPage.ImpactStatistics.FirstOrDefault();
        var existingMetric = existingStat?.Metrics.FirstOrDefault();

        var maxImageId = await Fixture.DbContext.Images.MaxAsync(i => (long?)i.Id) ?? 0;
        var nonExistentImageId = maxImageId + 1000;

        var updateDto = new UpdateMainPageDto
        {
            Title = "Updated title",
            Description = "Updated description",
            ImageId = nonExistentImageId,
            ImpactStatistics =
            [
                new UpdateImpactStatisticDto
                {
                    Id = existingStat?.Id,
                    Description = "Updated stat",
                    ImageId = nonExistentImageId,
                    Metrics =
                    [
                        new UpdateMetricDto
                        {
                            Id = existingMetric?.Id,
                            Value = "100",
                            Signature = "children",
                        },
                    ],
                },
            ],
        };

        var content = new StringContent(
            JsonSerializer.Serialize(updateDto),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await Fixture.HttpClient.PutAsync(_endpointUri, content);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}