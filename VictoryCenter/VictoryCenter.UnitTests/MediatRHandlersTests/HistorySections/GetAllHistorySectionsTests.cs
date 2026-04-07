using AutoMapper;
using Moq;
using VictoryCenter.BLL.DTOs.Admin.HistorySection;
using VictoryCenter.BLL.Queries.Admin.HistorySections.GetAll;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.HistoryContents;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.HistorySections;
using VictoryCenter.DAL.Repositories.Interfaces.Media;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.HistorySections;

public class GetAllHistorySectionsTests
{
    private readonly Mock<IMapper> _mapper = new();
    private readonly Mock<IRepositoryWrapper> _repositoryWrapper = new();
    private readonly Mock<IHistorySectionsRepository> _historySectionsRepository = new();
    private readonly Mock<IImageRepository> _imageRepository = new();

    [Fact]
    public async Task Handle_WithImageContent_LoadsAndAssignsImages()
    {
        var section = new HistorySection
        {
            Id = 1,
            Template = HistorySectionTemplate.SingleImageBottom,
            Order = 0,
            CreatedAt = DateTimeOffset.UtcNow,
            Contents =
            [
                new TitleHistoryContent { ContentType = ContentType.Title, Order = 0, Title = "Title" },
                new DescriptionHistoryContent { ContentType = ContentType.Description, Order = 1, Description = "Description" },
                new ImageHistoryContent { ContentType = ContentType.Image, Order = 2, ImageId = 7 }
            ]
        };

        var image = new Image
        {
            Id = 7,
            CreatedAt = DateTimeOffset.UtcNow,
            BlobName = "blob",
            MimeType = "image/png",
            Url = "https://example.com/image.png"
        };

        var expected = new List<HistorySectionDto> { new() { Id = 1, Template = HistorySectionTemplate.SingleImageBottom, Order = 0 } };

        var sut = CreateSut([section], [image], expected);

        var result = await sut.Handle(new GetAllHistorySectionsQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Same(image, ((ImageHistoryContent)section.Contents.Single(c => c.ContentType == ContentType.Image)).Image);
        Assert.Same(expected, result.Value);
    }

    [Fact]
    public async Task Handle_ImageNotFound_ReturnsFailure()
    {
        var section = new HistorySection
        {
            Id = 1,
            Template = HistorySectionTemplate.SingleImageBottom,
            Order = 0,
            CreatedAt = DateTimeOffset.UtcNow,
            Contents =
            [
                new ImageHistoryContent { ContentType = ContentType.Image, Order = 2, ImageId = 999 }
            ]
        };

        var sut = CreateSut([section], [], []);

        var result = await sut.Handle(new GetAllHistorySectionsQuery(), CancellationToken.None);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_WithoutImageContent_DoesNotQueryImages()
    {
        var section = new HistorySection
        {
            Id = 1,
            Template = HistorySectionTemplate.TextOnly,
            Order = 0,
            CreatedAt = DateTimeOffset.UtcNow,
            Contents =
            [
                new TitleHistoryContent { ContentType = ContentType.Title, Order = 0, Title = "Title" },
                new DescriptionHistoryContent { ContentType = ContentType.Description, Order = 1, Description = "Description" }
            ]
        };

        var expected = new List<HistorySectionDto> { new() { Id = 1, Template = HistorySectionTemplate.TextOnly, Order = 0 } };
        var sut = CreateSut([section], [], expected);

        var result = await sut.Handle(new GetAllHistorySectionsQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        _imageRepository.Verify(r => r.GetAllAsync(It.IsAny<QueryOptions<Image>>()), Times.Never);
    }

    private GetAllHistorySectionsQueryHandler CreateSut(
        IEnumerable<HistorySection> sections,
        IEnumerable<Image> images,
        List<HistorySectionDto> mapped)
    {
        _repositoryWrapper.Setup(r => r.HistorySectionsRepository).Returns(_historySectionsRepository.Object);
        _repositoryWrapper.Setup(r => r.ImageRepository).Returns(_imageRepository.Object);

        _historySectionsRepository
            .Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<HistorySection>>()))
            .ReturnsAsync(sections);

        _imageRepository
            .Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<Image>>()))
            .ReturnsAsync(images);

        _mapper
            .Setup(m => m.Map<List<HistorySectionDto>>(It.IsAny<IEnumerable<HistorySection>>()))
            .Returns(mapped);

        return new GetAllHistorySectionsQueryHandler(_mapper.Object, _repositoryWrapper.Object);
    }
}