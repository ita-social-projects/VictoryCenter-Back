using VictoryCenter.BLL.DTOs.Admin.WhoWeAreContent;
using VictoryCenter.BLL.Services.WhoWeAreContentFactory;
using VictoryCenter.DAL.Entities.WhoWeAreContents;

namespace VictoryCenter.UnitTests.ServiceTests;

public class WhoWeAreContentFactoryTests
{
    private readonly WhoWeAreContentFactory _factory;

    public WhoWeAreContentFactoryTests()
    {
        _factory = new WhoWeAreContentFactory();
    }

    [Fact]
    public void UpdateTitle_ShouldUpdateTitle_WhenEntityIsTitleContent()
    {
        var dto = new UpdateWhoWeAreContentDto { Title = "New Title" };
        var entity = new TitleContent { Title = "Old Title" };

        var result = _factory.UpdateTitle(dto, entity);

        Assert.Equal("New Title", result.Title);
    }

    [Fact]
    public void UpdateTitle_ShouldThrow_WhenEntityIsNotTitleContent()
    {
        var dto = new UpdateWhoWeAreContentDto { Title = "New Title" };
        var entity = new ImageContent();

        Assert.Throws<InvalidOperationException>(() => _factory.UpdateTitle(dto, entity));
    }

    [Fact]
    public void UpdateImage_ShouldUpdateImageId_WhenEntityIsImageContent()
    {
        var dto = new UpdateWhoWeAreContentDto { ImageId = 123 };
        var entity = new ImageContent { ImageId = 1 };

        var result = _factory.UpdateImage(dto, entity);

        Assert.Equal(123, result.ImageId);
    }

    [Fact]
    public void UpdateImage_ShouldThrow_WhenEntityIsNotImageContent()
    {
        var dto = new UpdateWhoWeAreContentDto { ImageId = 123 };
        var entity = new TitleContent();

        Assert.Throws<InvalidOperationException>(() => _factory.UpdateImage(dto, entity));
    }

    [Fact]
    public void UpdateDescription_ShouldUpdateDescription_WhenEntityIsDescriptionContent()
    {
        var dto = new UpdateWhoWeAreContentDto { Description = "New Description" };
        var entity = new DescriptionContent { Description = "Old Description" };

        var result = _factory.UpdateDescription(dto, entity);

        Assert.Equal("New Description", result.Description);
    }

    [Fact]
    public void UpdateDescription_ShouldThrow_WhenEntityIsNotDescriptionContent()
    {
        var dto = new UpdateWhoWeAreContentDto { Description = "New Description" };
        var entity = new TitleContent();

        Assert.Throws<InvalidOperationException>(() => _factory.UpdateDescription(dto, entity));
    }

    [Fact]
    public void UpdateCard_ShouldUpdateDescriptionAndImageId_WhenEntityIsCardContent()
    {
        var dto = new UpdateWhoWeAreContentDto { Description = "Description", ImageId = 55 };
        var entity = new CardContent { Description = "Old Description", ImageId = 1 };

        var result = _factory.UpdateCard(dto, entity);

        Assert.Equal("Description", result.Description);
        Assert.Equal(55, result.ImageId);
    }

    [Fact]
    public void UpdateCard_ShouldThrow_WhenEntityIsNotCardContent()
    {
        var dto = new UpdateWhoWeAreContentDto { Description = "Desc", ImageId = 55 };
        var entity = new DescriptionContent();

        Assert.Throws<InvalidOperationException>(() => _factory.UpdateCard(dto, entity));
    }
}
