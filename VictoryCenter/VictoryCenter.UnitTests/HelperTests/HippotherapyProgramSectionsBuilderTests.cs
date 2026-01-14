using VictoryCenter.BLL.DTOs.Admin.HippotherapyProgramSection;
using VictoryCenter.BLL.Helpers;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.HippotherapyProgramContents;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.UnitTests.HelperTests;

public class HippotherapyProgramSectionsBuilderTests
{
    [Fact]
    public void Build_SectionsNull_ReturnsEmpty()
    {
        var result = HippotherapyProgramSectionsBuilder.Build(
            null,
            new DateTimeOffset(2025, 12, 17, 0, 0, 0, TimeSpan.Zero),
            new Dictionary<long, Image>());

        Assert.Empty(result);
    }

    [Fact]
    public void Build_SectionsEmpty_ReturnsEmpty()
    {
        var result = HippotherapyProgramSectionsBuilder.Build(
            [],
            new DateTimeOffset(2025, 12, 17, 0, 0, 0, TimeSpan.Zero),
            new Dictionary<long, Image>());

        Assert.Empty(result);
    }

    [Fact]
    public void Build_TwoSections_ReturnsCountTwo()
    {
        var createdAt = new DateTimeOffset(2025, 12, 17, 0, 0, 0, TimeSpan.Zero);
        var result = HippotherapyProgramSectionsBuilder.Build(
            [MakeSectionDto(order: 1), MakeSectionDto(order: 2)],
            createdAt,
            new Dictionary<long, Image>());

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void Build_SetsCreatedAt()
    {
        var createdAt = new DateTimeOffset(2025, 12, 17, 1, 2, 3, TimeSpan.Zero);
        var result = HippotherapyProgramSectionsBuilder.Build(
            [MakeSectionDto(order: 7)],
            createdAt,
            new Dictionary<long, Image>());

        Assert.Equal(createdAt, result[0].CreatedAt);
    }

    [Fact]
    public void Build_MapsOrder()
    {
        var createdAt = new DateTimeOffset(2025, 12, 17, 1, 2, 3, TimeSpan.Zero);
        var result = HippotherapyProgramSectionsBuilder.Build(
            [MakeSectionDto(order: 7)],
            createdAt,
            new Dictionary<long, Image>());

        Assert.Equal(7, result[0].Order);
    }

    [Fact]
    public void Build_MapsTemplate()
    {
        var createdAt = new DateTimeOffset(2025, 12, 17, 1, 2, 3, TimeSpan.Zero);

        var dto = MakeSectionDto(order: 1);
        dto.Template = default;

        var result = HippotherapyProgramSectionsBuilder.Build(
            [dto],
            createdAt,
            new Dictionary<long, Image>());

        Assert.Equal(dto.Template, result[0].Template);
    }

    [Fact]
    public void Build_FullDto_ContentsCountIsSix()
    {
        var (result, _, _) = BuildFullDto();

        Assert.Equal(6, result[0].Contents.ToList().Count);
    }

    [Fact]
    public void Build_FullDto_Content0_IsTitleProgramContent()
    {
        var (result, _, _) = BuildFullDto();
        var contents = result[0].Contents.ToList();

        Assert.IsType<TitleProgramContent>(contents[0]);
    }

    [Fact]
    public void Build_FullDto_Content2_IsDescriptionProgramContent()
    {
        var (result, _, _) = BuildFullDto();
        var contents = result[0].Contents.ToList();

        Assert.IsType<DescriptionProgramContent>(contents[2]);
    }

    [Fact]
    public void Build_FullDto_Content4_IsImageProgramContent()
    {
        var (result, _, _) = BuildFullDto();
        var contents = result[0].Contents.ToList();

        Assert.IsType<ImageProgramContent>(contents[4]);
    }

    [Fact]
    public void Build_FullDto_Content0_ContentTypeIsTitle()
    {
        var (result, _, _) = BuildFullDto();
        var contents = result[0].Contents.ToList();

        Assert.Equal(ContentType.Title, contents[0].ContentType);
    }

    [Fact]
    public void Build_FullDto_Content2_ContentTypeIsDescription()
    {
        var (result, _, _) = BuildFullDto();
        var contents = result[0].Contents.ToList();

        Assert.Equal(ContentType.Description, contents[2].ContentType);
    }

    [Fact]
    public void Build_FullDto_Content4_ContentTypeIsImage()
    {
        var (result, _, _) = BuildFullDto();
        var contents = result[0].Contents.ToList();

        Assert.Equal(ContentType.Image, contents[4].ContentType);
    }

    [Fact]
    public void Build_FullDto_LastContentOrderIsFive()
    {
        var (result, _, _) = BuildFullDto();
        var contents = result[0].Contents.ToList();

        Assert.Equal(5, contents[5].Order);
    }

    [Fact]
    public void Build_FullDto_TitleTrimApplied()
    {
        var (result, _, _) = BuildFullDto();
        var contents = result[0].Contents.ToList();

        Assert.Equal("Title A", ((TitleProgramContent)contents[0]).Title);
    }

    [Fact]
    public void Build_FullDto_DescriptionTrimApplied()
    {
        var (result, _, _) = BuildFullDto();
        var contents = result[0].Contents.ToList();

        Assert.Equal("Desc 1", ((DescriptionProgramContent)contents[2]).Description);
    }

    [Fact]
    public void Build_FullDto_ImageReferenceSet()
    {
        var (result, image1, _) = BuildFullDto();
        var contents = result[0].Contents.ToList();

        Assert.Same(image1, ((ImageProgramContent)contents[4]).Image);
    }

    [Fact]
    public void Build_SkipsMissingImages_ContentsCountIsThree()
    {
        var createdAt = new DateTimeOffset(2025, 12, 17, 0, 0, 0, TimeSpan.Zero);
        var image1 = MakeImage(10);
        var imagesById = new Dictionary<long, Image> { [10] = image1 };

        var dto = new CreateHippotherapyProgramSectionDto
        {
            Template = default,
            Order = 1,
            Contents =
            [
                new CreateProgramSectionContentDto { ContentType = ContentType.Title, Order = 0, Title = "T" },
                new CreateProgramSectionContentDto { ContentType = ContentType.Description, Order = 1, Description = "D" },
                new CreateProgramSectionContentDto { ContentType = ContentType.Image, Order = 2, ImageId = 10 },
                new CreateProgramSectionContentDto { ContentType = ContentType.Image, Order = 3, ImageId = 999 }
            ]
        };

        var result = HippotherapyProgramSectionsBuilder.Build([dto], createdAt, imagesById);
        var contents = result[0].Contents.ToList();

        Assert.Equal(3, contents.Count);
    }

    [Fact]
    public void Build_SkipsMissingImages_LastImageIdIsTen()
    {
        var createdAt = new DateTimeOffset(2025, 12, 17, 0, 0, 0, TimeSpan.Zero);
        var image1 = MakeImage(10);
        var imagesById = new Dictionary<long, Image> { [10] = image1 };

        var dto = new CreateHippotherapyProgramSectionDto
        {
            Template = default,
            Order = 1,
            Contents =
            [
                new CreateProgramSectionContentDto { ContentType = ContentType.Title, Order = 0, Title = "T" },
                new CreateProgramSectionContentDto { ContentType = ContentType.Description, Order = 1, Description = "D" },
                new CreateProgramSectionContentDto { ContentType = ContentType.Image, Order = 2, ImageId = 10 },
                new CreateProgramSectionContentDto { ContentType = ContentType.Image, Order = 3, ImageId = 999 }
            ]
        };

        var result = HippotherapyProgramSectionsBuilder.Build([dto], createdAt, imagesById);
        var contents = result[0].Contents.ToList();

        Assert.Equal(10, ((ImageProgramContent)contents[2]).ImageId);
    }

    [Fact]
    public void Build_DtoWithNullLists_ContentsEmpty()
    {
        var createdAt = new DateTimeOffset(2025, 12, 17, 0, 0, 0, TimeSpan.Zero);

        var dto = new CreateHippotherapyProgramSectionDto
        {
            Template = default,
            Order = 1,
            Contents = null!
        };

        var result = HippotherapyProgramSectionsBuilder.Build([dto], createdAt, new Dictionary<long, Image>());
        var contents = result[0].Contents.ToList();

        Assert.Empty(contents);
    }

    [Fact]
    public void Build_MultipleSections_SecondSection_FirstContentOrderIsZero()
    {
        var createdAt = new DateTimeOffset(2025, 12, 17, 0, 0, 0, TimeSpan.Zero);
        var image = MakeImage(10);
        var imagesById = new Dictionary<long, Image> { [10] = image };

        var dto1 = new CreateHippotherapyProgramSectionDto
        {
            Template = default,
            Order = 2,
            Contents =
            [
                new CreateProgramSectionContentDto { ContentType = ContentType.Title, Order = 0, Title = "A" }
            ]
        };

        var dto2 = new CreateHippotherapyProgramSectionDto
        {
            Template = default,
            Order = 1,
            Contents =
            [
                new CreateProgramSectionContentDto { ContentType = ContentType.Description, Order = 0, Description = "B" },
                new CreateProgramSectionContentDto { ContentType = ContentType.Image, Order = 1, ImageId = 10 }
            ]
        };

        var result = HippotherapyProgramSectionsBuilder.Build([dto1, dto2], createdAt, imagesById);
        var contents = result[1].Contents.ToList();

        Assert.Equal(0, contents[0].Order);
    }

    [Fact]
    public void Build_MultipleSections_SecondSection_SecondContentOrderIsOne()
    {
        var createdAt = new DateTimeOffset(2025, 12, 17, 0, 0, 0, TimeSpan.Zero);
        var image = MakeImage(10);
        var imagesById = new Dictionary<long, Image> { [10] = image };

        var dto1 = new CreateHippotherapyProgramSectionDto
        {
            Template = default,
            Order = 2,
            Contents =
            [
                new CreateProgramSectionContentDto { ContentType = ContentType.Title, Order = 0, Title = "A" }
            ]
        };

        var dto2 = new CreateHippotherapyProgramSectionDto
        {
            Template = default,
            Order = 1,
            Contents =
            [
                new CreateProgramSectionContentDto { ContentType = ContentType.Description, Order = 0, Description = "B" },
                new CreateProgramSectionContentDto { ContentType = ContentType.Image, Order = 1, ImageId = 10 }
            ]
        };

        var result = HippotherapyProgramSectionsBuilder.Build([dto1, dto2], createdAt, imagesById);
        var contents = result[1].Contents.ToList();

        Assert.Equal(1, contents[1].Order);
    }

    [Fact]
    public void Build_DtoContentsNotSortedByOrder_ResultContentsSortedByOrder()
    {
        var createdAt = new DateTimeOffset(2025, 12, 17, 0, 0, 0, TimeSpan.Zero);

        var dto = new CreateHippotherapyProgramSectionDto
        {
            Template = default,
            Order = 1,
            Contents =
            [
                new CreateProgramSectionContentDto { ContentType = ContentType.Title, Order = 2, Title = "T2" },
                new CreateProgramSectionContentDto { ContentType = ContentType.Title, Order = 0, Title = "T0" },
                new CreateProgramSectionContentDto { ContentType = ContentType.Title, Order = 1, Title = "T1" }
            ]
        };

        var result = HippotherapyProgramSectionsBuilder.Build([dto], createdAt, new Dictionary<long, Image>());
        var contents = result[0].Contents.ToList();

        Assert.Equal(0, contents[0].Order);
        Assert.Equal(1, contents[1].Order);
        Assert.Equal(2, contents[2].Order);
    }

    private static (List<HippotherapyProgramSection> result, Image image1, Image image2) BuildFullDto()
    {
        var createdAt = new DateTimeOffset(2025, 12, 17, 1, 2, 3, TimeSpan.Zero);

        var image1 = MakeImage(10);
        var image2 = MakeImage(20);

        var imagesById = new Dictionary<long, Image>
        {
            [10] = image1,
            [20] = image2
        };

        var dto = new CreateHippotherapyProgramSectionDto
        {
            Template = default,
            Order = 1,
            Contents =
            [
                new CreateProgramSectionContentDto { ContentType = ContentType.Title, Order = 0, Title = "  Title A  " },
                new CreateProgramSectionContentDto { ContentType = ContentType.Title, Order = 1, Title = "Title B" },
                new CreateProgramSectionContentDto { ContentType = ContentType.Description, Order = 2, Description = "  Desc 1 " },
                new CreateProgramSectionContentDto { ContentType = ContentType.Description, Order = 3, Description = "Desc 2  " },
                new CreateProgramSectionContentDto { ContentType = ContentType.Image, Order = 4, ImageId = 10 },
                new CreateProgramSectionContentDto { ContentType = ContentType.Image, Order = 5, ImageId = 20 }
            ]
        };

        var result = HippotherapyProgramSectionsBuilder.Build([dto], createdAt, imagesById);
        return (result, image1, image2);
    }

    private static CreateHippotherapyProgramSectionDto MakeSectionDto(int order)
    {
        return new CreateHippotherapyProgramSectionDto
        {
            Template = default,
            Order = order,
            Contents = []
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
