using VictoryCenter.BLL.DTOs.Admin.HistorySection;
using VictoryCenter.BLL.Helpers;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.HistoryContents;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.UnitTests.HelperTests;

public class HistorySectionsBuilderTests
{
    private static readonly DateTimeOffset CreatedAt = new(2025, 12, 17, 1, 2, 3, TimeSpan.Zero);

    [Fact]
    public void Build_SectionsNull_ReturnsEmpty()
    {
        var result = HistorySectionsBuilder.Build(null, CreatedAt, new Dictionary<long, Image>());

        Assert.Empty(result);
    }

    [Fact]
    public void Build_SectionsEmpty_ReturnsEmpty()
    {
        var result = HistorySectionsBuilder.Build([], CreatedAt, new Dictionary<long, Image>());

        Assert.Empty(result);
    }

    [Fact]
    public void Build_TwoSections_ReturnsTwo()
    {
        var result = Build([Section(order: 1), Section(order: 2)]);

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void Build_MapsSectionCreatedAt()
    {
        var result = Build([Section(order: 7)]);

        Assert.Equal(CreatedAt, result[0].CreatedAt);
    }

    [Fact]
    public void Build_MapsSectionOrder()
    {
        var result = Build([Section(order: 7)]);

        Assert.Equal(7, result[0].Order);
    }

    [Fact]
    public void Build_MapsSectionTemplate()
    {
        var dto = Section(order: 1) with { Template = HistorySectionTemplate.SingleImageBottom };

        var result = Build([dto]);

        Assert.Equal(HistorySectionTemplate.SingleImageBottom, result[0].Template);
    }

    [Fact]
    public void Build_NullContents_ReturnsEmptyContents()
    {
        var dto = new CreateHistorySectionDto
        {
            Template = default,
            Order = 1,
            Contents = null
        };

        var result = Build([dto]);

        Assert.Empty(result[0].Contents);
    }

    [Fact]
    public void Build_SortsContentsByOrder()
    {
        var dto = Section(
            order: 1,
            Title(order: 2, "T2"),
            Title(order: 0, "T0"),
            Title(order: 1, "T1"));

        var result = Build([dto]);
        var orders = result[0].Contents.Select(x => x.Order).ToList();

        Assert.True(orders.Count == 3 && orders[0] == 0 && orders[1] == 1 && orders[2] == 2);
    }

    [Fact]
    public void Build_TrimsTitle()
    {
        var dto = Section(order: 1, Title(order: 0, "  Title A  "), Description(order: 1, "Desc 1"));

        var result = Build([dto]);
        var title = (TitleHistoryContent)result[0].Contents.First(x => x.ContentType == ContentType.Title);

        Assert.Equal("Title A", title.Title);
    }

    [Fact]
    public void Build_TrimsDescription()
    {
        var dto = Section(order: 1, Title(order: 0, "Title"), Description(order: 1, "  Desc 1 "));

        var result = Build([dto]);
        var description = (DescriptionHistoryContent)result[0].Contents.First(x => x.ContentType == ContentType.Description);

        Assert.Equal("Desc 1", description.Description);
    }

    [Fact]
    public void Build_CreatesImageHistoryContent()
    {
        var image = MakeImage(10);
        var dto = Section(order: 1, Title(order: 0, "Title"), Description(order: 1, "Desc 1"), Image(order: 2, 10));

        var result = Build([dto], new Dictionary<long, Image> { [10] = image });
        var content = result[0].Contents.First(x => x.ContentType == ContentType.Image);

        Assert.IsType<ImageHistoryContent>(content);
    }

    [Fact]
    public void Build_SetsImageReference()
    {
        var image = MakeImage(10);
        var dto = Section(order: 1, Title(order: 0, "Title"), Description(order: 1, "Desc 1"), Image(order: 2, 10));

        var result = Build([dto], new Dictionary<long, Image> { [10] = image });
        var content = (ImageHistoryContent)result[0].Contents.First(x => x.ContentType == ContentType.Image);

        Assert.Same(image, content.Image);
    }

    [Fact]
    public void Build_SkipsMissingImages()
    {
        var image = MakeImage(10);
        var dto = Section(
            order: 1,
            Title(order: 0, "Title"),
            Description(order: 1, "Desc 1"),
            Image(order: 2, 10),
            Image(order: 3, 999));

        var result = Build([dto], new Dictionary<long, Image> { [10] = image });

        Assert.Equal(3, result[0].Contents.Count);
    }

    [Fact]
    public void Build_SkipsImageWithInvalidId()
    {
        var dto = Section(
            order: 1,
            Title(order: 0, "Title"),
            Description(order: 1, "Desc 1"),
            new CreateHistorySectionContentDto { ContentType = ContentType.Image, Order = 2, ImageId = 0 });

        var result = Build([dto]);

        Assert.Equal(2, result[0].Contents.Count);
    }

    [Fact]
    public void Build_SkipsUnknownContentType()
    {
        var dto = Section(
            order: 1,
            Title(order: 0, "Title"),
            new CreateHistorySectionContentDto { ContentType = ContentType.Card, Order = 1 });

        var result = Build([dto]);

        Assert.Single(result[0].Contents);
    }

    private static List<HistorySection> Build(
        List<CreateHistorySectionDto> sections,
        IReadOnlyDictionary<long, Image>? imagesById = null)
    {
        return HistorySectionsBuilder.Build(
            sections,
            CreatedAt,
            imagesById ?? new Dictionary<long, Image>());
    }

    private static CreateHistorySectionDto Section(
        int order,
        params CreateHistorySectionContentDto[] contents)
    {
        return new CreateHistorySectionDto
        {
            Template = default,
            Order = order,
            Contents = [.. contents]
        };
    }

    private static CreateHistorySectionContentDto Title(int order, string title)
    {
        return new CreateHistorySectionContentDto
        {
            ContentType = ContentType.Title,
            Order = order,
            Title = title
        };
    }

    private static CreateHistorySectionContentDto Description(int order, string description)
    {
        return new CreateHistorySectionContentDto
        {
            ContentType = ContentType.Description,
            Order = order,
            Description = description
        };
    }

    private static CreateHistorySectionContentDto Image(int order, long imageId)
    {
        return new CreateHistorySectionContentDto
        {
            ContentType = ContentType.Image,
            Order = order,
            ImageId = imageId
        };
    }

    private static Image MakeImage(long id)
    {
        return new Image
        {
            Id = id,
            CreatedAt = DateTimeOffset.UtcNow,
            BlobName = "blob",
            MimeType = "image/png",
            Url = "https://example.com/image.png"
        };
    }
}