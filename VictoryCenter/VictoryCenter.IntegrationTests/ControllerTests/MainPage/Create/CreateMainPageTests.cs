using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics.Metrics;
using VictoryCenter.BLL.DTOs.Admin.MainAboutUs;
using VictoryCenter.BLL.DTOs.Admin.MainPages;
using VictoryCenter.BLL.DTOs.Admin.MainPartners;
using VictoryCenter.DAL.Entities;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.MainPage.Create;

public class CreateMainPageTests : BaseTestClass
{
    private readonly Uri _endpointUri = new("/api/MainPage", UriKind.Relative);

    public CreateMainPageTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task CreateMainPage_WithValidData_ShouldReturnOkAndCreateEntity()
    {
        Fixture.DbContext.MainPages.RemoveRange(Fixture.DbContext.MainPages);
        await Fixture.DbContext.SaveChangesAsync();

        var image = await EnsureImageExistsAsync();

        var createDto = new CreateMainPageDto
        {
            Title = "New MainPage title",
            Description = "New MainPage description",
            ImageId = image.Id,
            MainAboutUs = new CreateMainAboutUsDto
            {
                Title = "New About Us title",
                Description = "New About Us description",
            },
            MainPartners = new CreateMainPartnersDto
            {
                Title = "New Partners title",
                Description = "New Partners description",
            },
            ImpactStatistics =
            [
                new CreateImpactStatisticDto
                {
                    Description = "New stat",
                    ImageId = image.Id,
                    Metrics =
                    [
                        new CreateMetricDto
                        {
                            Value = "500",
                            Signature = "new-metric",
                        }

                    ]
                }

            ]
        };

        var content = new StringContent(
            JsonSerializer.Serialize(createDto),
            Encoding.UTF8,
            "application/json");

        var response = await Fixture.HttpClient.PostAsync(_endpointUri, content);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var createdMainPage = await Fixture.DbContext.MainPages
            .Include(m => m.MainAboutUs)
            .Include(m => m.MainPartners)
            .Include(m => m.ImpactStatistics)
            .FirstOrDefaultAsync(m => m.Title == createDto.Title);

        Assert.NotNull(createdMainPage);
        Assert.Equal(createDto.Title, createdMainPage.Title);
        Assert.Equal(createDto.ImageId, createdMainPage.ImageId);
        Assert.NotNull(createdMainPage.MainAboutUs);
        Assert.NotNull(createdMainPage.MainPartners);
        Assert.Single(createdMainPage.ImpactStatistics);
    }

    [Fact]
    public async Task CreateMainPage_WithEmptyTitle_ShouldReturnBadRequest()
    {
        var createDto = new CreateMainPageDto
        {
            Title = string.Empty,
            Description = "Valid description",
        };

        var content = new StringContent(
            JsonSerializer.Serialize(createDto),
            Encoding.UTF8,
            "application/json");

        var response = await Fixture.HttpClient.PostAsync(_endpointUri, content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateMainPage_WithNonExistentImageId_ShouldReturnNotFound()
    {
        var maxImageId = await Fixture.DbContext.Images.MaxAsync(i => (long?)i.Id) ?? 0;
        var nonExistentImageId = maxImageId + 1000;

        var createDto = new CreateMainPageDto
        {
            Title = "Valid title",
            Description = "Valid description",
            ImageId = nonExistentImageId
        };

        var content = new StringContent(
            JsonSerializer.Serialize(createDto),
            Encoding.UTF8,
            "application/json");

        var response = await Fixture.HttpClient.PostAsync(_endpointUri, content);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<Image> EnsureImageExistsAsync()
    {
        var image = await Fixture.DbContext.Images.OrderBy(i => i.Id).FirstOrDefaultAsync();
        if (image is not null)
        {
            return image;
        }

        image = new Image
        {
            Url = "https://example.com/seed-image-1.jpg",
            CreatedAt = DateTimeOffset.UtcNow,
        };

        await Fixture.DbContext.Images.AddAsync(image);
        await Fixture.DbContext.SaveChangesAsync();

        return image;
    }
}