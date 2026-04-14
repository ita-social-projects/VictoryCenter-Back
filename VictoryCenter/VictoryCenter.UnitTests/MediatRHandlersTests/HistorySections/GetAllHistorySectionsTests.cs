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

    [Fact]
    public async Task Handle_MapsSectionsOrderedByOrder()
    {
        var sectionWithHigherOrder = new HistorySection
        {
            Id = 3,
            Template = HistorySectionTemplate.TextOnly,
            Order = 2,
            CreatedAt = DateTimeOffset.UtcNow,
            Contents = []
        };

        var sectionWithLowestOrder = new HistorySection
        {
            Id = 1,
            Template = HistorySectionTemplate.TextOnly,
            Order = 0,
            CreatedAt = DateTimeOffset.UtcNow,
            Contents = []
        };

        var sectionWithMiddleOrder = new HistorySection
        {
            Id = 2,
            Template = HistorySectionTemplate.TextOnly,
            Order = 1,
            CreatedAt = DateTimeOffset.UtcNow,
            Contents = []
        };

        var sections = new[]
        {
            sectionWithHigherOrder,
            sectionWithMiddleOrder,
            sectionWithLowestOrder
        };

        var orderedIds = new[]
        {
            sectionWithLowestOrder.Id,
            sectionWithMiddleOrder.Id,
            sectionWithHigherOrder.Id
        };

        var expected = new List<HistorySectionDto>();

        _repositoryWrapper.Setup(r => r.HistorySectionsRepository).Returns(_historySectionsRepository.Object);
        _repositoryWrapper.Setup(r => r.ImageRepository).Returns(_imageRepository.Object);

        _historySectionsRepository
            .Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<HistorySection>>()))
            .ReturnsAsync((QueryOptions<HistorySection>? options) =>
            {
                var query = sections.AsQueryable();

                if (options?.OrderByASC is not null)
                {
                    query = query.OrderBy(options.OrderByASC);
                }

                if (options?.OrderByDESC is not null)
                {
                    query = query.OrderByDescending(options.OrderByDESC);
                }

                return query.ToList();
            });

        _mapper
            .Setup(m => m.Map<List<HistorySectionDto>>(It.IsAny<IEnumerable<HistorySection>>()))
            .Returns(expected);

        var sut = new GetAllHistorySectionsQueryHandler(_mapper.Object, _repositoryWrapper.Object);

        var result = await sut.Handle(new GetAllHistorySectionsQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Same(expected, result.Value);
        _historySectionsRepository.Verify(
            r => r.GetAllAsync(It.Is<QueryOptions<HistorySection>>(options => options.OrderByASC != null)),
            Times.Once);
        _mapper.Verify(
            m => m.Map<List<HistorySectionDto>>(It.Is<IEnumerable<HistorySection>>(mappedSections =>
                mappedSections.Select(section => section.Id).SequenceEqual(orderedIds))),
            Times.Once);
    }

    [Fact]
    public async Task Handle_MapsSectionContentsOrderedByOrder()
    {
        var section = new HistorySection
        {
            Id = 1,
            Template = HistorySectionTemplate.TextOnly,
            Order = 0,
            CreatedAt = DateTimeOffset.UtcNow,
            Contents =
            [
                new DescriptionHistoryContent
                {
                    ContentType = ContentType.Description,
                    Order = 2,
                    Description = "Description 2"
                },
                new TitleHistoryContent
                {
                    ContentType = ContentType.Title,
                    Order = 0,
                    Title = "Title"
                },
                new DescriptionHistoryContent
                {
                    ContentType = ContentType.Description,
                    Order = 1,
                    Description = "Description 1"
                },
            ]
        };

        var expectedContentOrders = new[] { 0, 1, 2 };
        var expected = new List<HistorySectionDto>();

        var sut = CreateSut([section], [], expected);

        var result = await sut.Handle(new GetAllHistorySectionsQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Same(expected, result.Value);
        _mapper.Verify(
            m => m.Map<List<HistorySectionDto>>(It.Is<IEnumerable<HistorySection>>(mappedSections =>
                mappedSections
                    .Single(s => s.Id == section.Id)
                    .Contents
                    .Select(content => content.Order)
                    .SequenceEqual(expectedContentOrders))),
            Times.Once);
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