using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.DTOs.Admin.MainPages;
using VictoryCenter.DAL.Entities;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

using EntityMainPage = VictoryCenter.DAL.Entities.MainPage;

namespace VictoryCenter.IntegrationTests.ControllerTests.MainPage.Get;

public class GetMainPageTests : BaseTestClass
{
    private readonly Uri _endpointUri = new("/api/MainPage", UriKind.Relative);

    public GetMainPageTests(IntegrationTestDbFixture fixture)
        : base(fixture)
    {
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
    }

    [Fact]
    public async Task GetMainPage_WhenDoesNotExist_ShouldReturnNotFound()
    {
        Fixture.DbContext.MainPages.RemoveRange(Fixture.DbContext.MainPages);
        await Fixture.DbContext.SaveChangesAsync();

        var response = await Fixture.HttpClient.GetAsync(_endpointUri);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<EntityMainPage> EnsureMainPageExistsAsync()
    {
        var existing = await Fixture.DbContext.MainPages.FirstOrDefaultAsync();
        if (existing is not null)
        {
            return existing;
        }

        var image = await EnsureImageExistsAsync();

        var mainPage = new EntityMainPage
        {
            Title = "Seed MainPage Title",
            Description = "Seed MainPage Description",
            ImageId = image.Id,
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