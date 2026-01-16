using VictoryCenter.BLL.DTOs.Admin.HippotherapyProgramSection;
using VictoryCenter.BLL.Helpers;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.HippotherapyProgramContents;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.UnitTests.HelperTests;

public class HippotherapyProgramSectionsBuilderTests
{
    private static readonly DateTimeOffset CreatedAt = new(2025, 12, 17, 1, 2, 3, TimeSpan.Zero);

    [Fact]
    public void Build_SectionsNull_ReturnsEmpty()
    {
        var result = HippotherapyProgramSectionsBuilder.Build(null, CreatedAt, new Dictionary<long, Image>());

        Assert.Empty(result);
    }

    [Fact]
    public void Build_SectionsEmpty_ReturnsEmpty()
    {
        var result = HippotherapyProgramSectionsBuilder.Build([], CreatedAt, new Dictionary<long, Image>());

        Assert.Empty(result);
    }

    [Fact]
    public void Build_TwoSections_ReturnsTwo()
    {
        var result = Build([Section(order: 1), Section(order: 2)]);

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void Build_SetsCreatedAt()
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
        var dto = Section(order: 1) with { Template = ProgramSectionTemplate.SingleImageBottom };

        var result = Build([dto]);

        Assert.Equal(ProgramSectionTemplate.SingleImageBottom, result[0].Template);
    }

    [Fact]
    public void Build_NullContents_ReturnsEmptyContents()
    {
        var dto = new CreateHippotherapyProgramSectionDto
        {
            Template = default,
            Order = 1,
            Contents = null!
        };

        var result = Build([dto]);
        var contents = result[0].Contents.ToList();

        Assert.Empty(contents);
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
        var title = (TitleProgramContent)result[0].Contents.First(x => x.ContentType == ContentType.Title);

        Assert.Equal("Title A", title.Title);
    }

    [Fact]
    public void Build_TrimsDescription()
    {
        var dto = Section(order: 1, Title(order: 0, "Title"), Description(order: 1, "  Desc 1 "));

        var result = Build([dto]);
        var description = (DescriptionProgramContent)result[0].Contents.First(x => x.ContentType == ContentType.Description);

        Assert.Equal("Desc 1", description.Description);
    }

    [Fact]
    public void Build_CreatesImageProgramContent()
    {
        var image = MakeImage(10);
        var dto = Section(order: 1, Title(order: 0, "Title"), Description(order: 1, "Desc 1"), Image(order: 2, 10));
        var result = Build([dto], new Dictionary<long, Image> { [10] = image });

        var content = result[0].Contents.First(x => x.ContentType == ContentType.Image);

        Assert.IsType<ImageProgramContent>(content);
    }

    [Fact]
    public void Build_SetsImageReference()
    {
        var image = MakeImage(10);
        var dto = Section(order: 1, Title(order: 0, "Title"), Description(order: 1, "Desc 1"), Image(order: 2, 10));
        var result = Build([dto], new Dictionary<long, Image> { [10] = image });

        var content = (ImageProgramContent)result[0].Contents.First(x => x.ContentType == ContentType.Image);

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
            new CreateProgramSectionContentDto { ContentType = ContentType.Image, Order = 2, ImageId = 0 });

        var result = Build([dto]);

        Assert.Equal(2, result[0].Contents.Count);
    }

    [Fact]
    public void Build_SkipsUnknownContentType()
    {
        var dto = Section(
            order: 1,
            Title(order: 0, "Title"),
            new CreateProgramSectionContentDto { ContentType = ContentType.Card, Order = 1 });

        var result = Build([dto]);

        Assert.Single(result[0].Contents);
    }

    [Fact]
    public void Build_CreatesAuthorProgramContent()
    {
        var dto = Section(order: 1, Title(order: 0, "Title"), Author(order: 1, "Author Name"));

        var result = Build([dto]);

        var content = result[0].Contents.First(x => x.ContentType == ContentType.Author);

        Assert.IsType<AuthorProgramContent>(content);
    }

    [Fact]
    public void Build_TrimsAuthor()
    {
        var dto = Section(order: 1, Title(order: 0, "Title"), Author(order: 1, "  Member 1  "));

        var result = Build([dto]);

        var author = (AuthorProgramContent)result[0].Contents.First(x => x.ContentType == ContentType.Author);

        Assert.Equal("Member 1", author.Name);
    }

    [Fact]
    public void Build_MapsGroupIndex_ForAuthor()
    {
        var dto = Section(
            order: 1,
            Title(order: 0, "Title"),
            new CreateProgramSectionContentDto { ContentType = ContentType.Author, Order = 1, GroupIndex = 2, Author = "Member" });

        var result = Build([dto]);

        var author = (AuthorProgramContent)result[0].Contents.First(x => x.ContentType == ContentType.Author);

        Assert.Equal(2, author.GroupIndex);
    }

    private static List<HippotherapyProgramSection> Build(
        List<CreateHippotherapyProgramSectionDto> sections,
        IReadOnlyDictionary<long, Image>? imagesById = null)
    {
        return HippotherapyProgramSectionsBuilder.Build(
            sections,
            CreatedAt,
            imagesById ?? new Dictionary<long, Image>());
    }

    private static CreateHippotherapyProgramSectionDto Section(
        int order,
        params CreateProgramSectionContentDto[] contents)
    {
        return new CreateHippotherapyProgramSectionDto
        {
            Template = default,
            Order = order,
            Contents = [.. contents]
        };
    }

    private static CreateProgramSectionContentDto Title(int order, string title)
    {
        return new CreateProgramSectionContentDto
        {
            ContentType = ContentType.Title,
            Order = order,
            Title = title
        };
    }

    private static CreateProgramSectionContentDto Description(int order, string description)
    {
        return new CreateProgramSectionContentDto
        {
            ContentType = ContentType.Description,
            Order = order,
            Description = description
        };
    }

    private static CreateProgramSectionContentDto Image(int order, long imageId)
    {
        return new CreateProgramSectionContentDto
        {
            ContentType = ContentType.Image,
            Order = order,
            ImageId = imageId
        };
    }

    private static CreateProgramSectionContentDto Author(int order, string author)
    {
        return new CreateProgramSectionContentDto
        {
            ContentType = ContentType.Author,
            Order = order,
            Author = author
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
