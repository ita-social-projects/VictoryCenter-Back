using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using VictoryCenter.BLL.DTOs.Admin.MainPages;
using VictoryCenter.DAL.Entities;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

using EntityMainPage = VictoryCenter.DAL.Entities.MainPage;

namespace VictoryCenter.IntegrationTests.ControllerTests.MainPage.Get;

public class GetMainPageTests : BaseTestClass
{
    private IDbContextTransaction? _transaction;
    private readonly Uri _endpointUri = new("/api/MainPage", UriKind.Relative);

    public GetMainPageTests(IntegrationTestDbFixture fixture)
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
    public async Task GetMainPage_WhenExists_ShouldReturnOkAndData()
    {
        var expected = await EnsureMainPageExistsAsync();

        var response = await Fixture.HttpClient.GetAsync(_endpointUri);
        var content = await response.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<MainPageDto>(content, JsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
        Assert.Equal(expected.Title, result.Title);
        Assert.Equal(expected.Description, result.Description);
        Assert.NotNull(result.MainDonations);
        Assert.Equal(expected.MainDonations!.Title, result.MainDonations.Title);
    }

    [Fact]
    public async Task GetMainPage_WhenDoesNotExist_ShouldReturnOkWithEmptyDto()
    {
        Fixture.DbContext.MainPages.RemoveRange(Fixture.DbContext.MainPages);
        await Fixture.DbContext.SaveChangesAsync();

        var response = await Fixture.HttpClient.GetAsync(_endpointUri);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<MainPageDto>();
        Assert.NotNull(result);
        Assert.Equal(0, result.Id);
        Assert.Equal(string.Empty, result.Title);
        Assert.Equal(string.Empty, result.Description);
        Assert.Empty(result.Localizations);
    }

    private async Task<EntityMainPage> EnsureMainPageExistsAsync()
    {
        var existing = await Fixture.DbContext.MainPages
            .Include(m => m.MainDonations)
            .FirstOrDefaultAsync();
        if (existing?.MainDonations is not null)
        {
            return existing;
        }

        if (existing is not null)
        {
            existing.MainDonations = new MainDonations
            {
                Title = "Seed Donations Title",
                Description = "Seed Donations Description",
                MainPageId = existing.Id,
            };

            await Fixture.DbContext.MainDonations.AddAsync(existing.MainDonations);
            await Fixture.DbContext.SaveChangesAsync();
            Fixture.DbContext.ChangeTracker.Clear();

            return existing;
        }

        var image = await EnsureImageExistsAsync();

        var mainPage = new EntityMainPage
        {
            Title = "Seed MainPage Title",
            Description = "Seed MainPage Description",
            ImageId = image.Id,
            MainDonations = new MainDonations
            {
                Title = "Seed Donations Title",
                Description = "Seed Donations Description",
                ImageId = image.Id,
            },
        };

        await Fixture.DbContext.MainPages.AddAsync(mainPage);
        await Fixture.DbContext.SaveChangesAsync();
        Fixture.DbContext.ChangeTracker.Clear();

        return mainPage;
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
