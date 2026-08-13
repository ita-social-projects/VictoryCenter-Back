using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics.Metrics;
using VictoryCenter.BLL.DTOs.Admin.MainAboutUs;
using VictoryCenter.BLL.DTOs.Admin.MainDonations;
using VictoryCenter.BLL.DTOs.Admin.MainPages;
using VictoryCenter.BLL.DTOs.Admin.MainPartners;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

namespace VictoryCenter.IntegrationTests.ControllerTests.MainPage.Create;

public class CreateMainPageTests : BaseTestClass
{
    private IDbContextTransaction? _transaction;
    private readonly Uri _endpointUri = new("/api/MainPage", UriKind.Relative);

    public CreateMainPageTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _transaction = await Fixture.DbContext.Database.BeginTransactionAsync();
    }

    public override async Task DisposeAsync()
    {
        if (_transaction is not null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
        }

        await base.DisposeAsync();
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
            MainDonations = new CreateMainDonationsDto
            {
                Title = "New Donations title",
                Description = "New Donations description",
                ImageId = image.Id,
            },
            ImpactStatistics = new CreateImpactStatisticDto
            {
                Title = "New stat",
                ImageId = image.Id,
                Metrics =
                [
                    new CreateMetricDto { Value = 120,  Name = "Партнерів",      Type = MetricType.Partners },
                    new CreateMetricDto { Value = 45,   Name = "Програм",        Type = MetricType.Programs },
                    new CreateMetricDto { Value = 2500, Name = "Зібрано",        Type = MetricType.Raised },
                    new CreateMetricDto { Value = 8400, Name = "Годин терапії",  Type = MetricType.TherapyHours },
                ],
            },
        };

        var content = new StringContent(
            JsonSerializer.Serialize(createDto),
            Encoding.UTF8,
            "application/json");

        var response = await Fixture.HttpClient.PostAsync(_endpointUri, content);

        var responseBody = await response.Content.ReadAsStringAsync();
        Assert.True(HttpStatusCode.OK == response.StatusCode, $"Status code: {response.StatusCode}. Body: {responseBody}");

        var createdMainPage = await Fixture.DbContext.MainPages
            .Include(m => m.MainAboutUs)
            .Include(m => m.MainPartners)
            .Include(m => m.MainDonations)
            .Include(m => m.ImpactStatistics)
            .FirstOrDefaultAsync(m => m.Title == createDto.Title);

        Assert.NotNull(createdMainPage);
        Assert.Equal(createDto.Title, createdMainPage.Title);
        Assert.Equal(createDto.ImageId, createdMainPage.ImageId);
        Assert.NotNull(createdMainPage.MainAboutUs);
        Assert.NotNull(createdMainPage.MainPartners);
        Assert.NotNull(createdMainPage.MainDonations);
        Assert.Equal(createDto.MainDonations.ImageId, createdMainPage.MainDonations.ImageId);
        Assert.NotNull(createdMainPage.ImpactStatistics);
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
    public async Task CreateMainPage_WithNonExistentImageId_ShouldReturnBadRequest()
    {
        var maxImageId = await Fixture.DbContext.Images.MaxAsync(i => (long?)i.Id) ?? 0;
        var nonExistentImageId = maxImageId + 1000;

        var createDto = new CreateMainPageDto
        {
            Title = "Valid title",
            Description = "Valid description",
            ImageId = nonExistentImageId,
        };

        var content = new StringContent(
            JsonSerializer.Serialize(createDto),
            Encoding.UTF8,
            "application/json");

        var response = await Fixture.HttpClient.PostAsync(_endpointUri, content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateMainPage_WithInvalidMainDonations_ShouldReturnBadRequest()
    {
        var createDto = new CreateMainPageDto
        {
            Title = "Valid title",
            Description = "Valid description",
            MainDonations = new CreateMainDonationsDto
            {
                Title = string.Empty,
                Description = "Valid donations description",
            },
        };

        var content = new StringContent(
            JsonSerializer.Serialize(createDto),
            Encoding.UTF8,
            "application/json");

        var response = await Fixture.HttpClient.PostAsync(_endpointUri, content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateMainPage_WithNonExistentMainDonationsImageId_ShouldReturnBadRequest()
    {
        var maxImageId = await Fixture.DbContext.Images.MaxAsync(i => (long?)i.Id) ?? 0;
        var nonExistentImageId = maxImageId + 1000;

        var createDto = new CreateMainPageDto
        {
            Title = "Valid title",
            Description = "Valid description",
            MainDonations = new CreateMainDonationsDto
            {
                Title = "Valid donations title",
                Description = "Valid donations description",
                ImageId = nonExistentImageId,
            },
        };

        var content = new StringContent(
            JsonSerializer.Serialize(createDto),
            Encoding.UTF8,
            "application/json");

        var response = await Fixture.HttpClient.PostAsync(_endpointUri, content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
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
