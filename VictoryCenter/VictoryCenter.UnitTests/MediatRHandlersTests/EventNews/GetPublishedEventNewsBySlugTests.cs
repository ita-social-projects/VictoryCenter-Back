using System.Linq.Expressions;
using AutoMapper;
using Moq;
using VictoryCenter.BLL.Queries.Public.EventNews.GetPublishedBySlug;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;
using EventNewsEntity = VictoryCenter.DAL.Entities.EventNews;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.EventNews;

public class GetPublishedEventNewsBySlugTests
{
    private const string Slug = "how-horses-support-children-and-veterans";

    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper = new();

    [Fact]
    public async Task Handle_WhenPublishedNewsExists_ShouldReturnMappedDto()
    {
        var entity = new EventNewsEntity { Id = 5, Slug = Slug, Status = Status.Published };
        var dto = new PublishedEventNewsDetailsDto { Id = 5, Slug = Slug };
        _mockRepositoryWrapper
            .Setup(x => x.EventNewsRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<EventNewsEntity>>()))
            .ReturnsAsync(entity);
        _mapperMock
            .Setup(x => x.Map<PublishedEventNewsDetailsDto>(entity))
            .Returns(dto);

        var result = await CreateHandler().Handle(
            new GetPublishedEventNewsBySlugQuery(Slug),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Same(dto, result.Value);
    }

    [Fact]
    public async Task Handle_WhenNewsDoesNotExist_ShouldReturnFailure()
    {
        _mockRepositoryWrapper
            .Setup(x => x.EventNewsRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<EventNewsEntity>>()))
            .ReturnsAsync((EventNewsEntity?)null);

        var result = await CreateHandler().Handle(
            new GetPublishedEventNewsBySlugQuery("unknown-slug"),
            CancellationToken.None);

        Assert.True(result.IsFailed);
        _mapperMock.Verify(
            x => x.Map<PublishedEventNewsDetailsDto>(It.IsAny<object>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldFilterBySlugAndPublishedStatusOnly()
    {
        Expression<Func<EventNewsEntity, bool>>? captured = null;
        _mockRepositoryWrapper
            .Setup(x => x.EventNewsRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<EventNewsEntity>>()))
            .Callback<QueryOptions<EventNewsEntity>>(o => captured = o.Filter)
            .ReturnsAsync((EventNewsEntity?)null);

        await CreateHandler().Handle(new GetPublishedEventNewsBySlugQuery(Slug), CancellationToken.None);

        Assert.NotNull(captured);
        List<EventNewsEntity> data =
        [
            new() { Id = 1, Slug = Slug, Status = Status.Published },
            new() { Id = 2, Slug = Slug, Status = Status.Draft },
            new() { Id = 3, Slug = "another-slug", Status = Status.Published },
        ];

        var matched = data.Where(captured.Compile()).Select(e => e.Id).ToList();

        Assert.Equal([1L], matched);
    }

    [Fact]
    public async Task Handle_ShouldUseReadOnlyQueryWithIncludes()
    {
        _mockRepositoryWrapper
            .Setup(x => x.EventNewsRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<EventNewsEntity>>()))
            .ReturnsAsync((EventNewsEntity?)null);

        await CreateHandler().Handle(new GetPublishedEventNewsBySlugQuery(Slug), CancellationToken.None);

        _mockRepositoryWrapper.Verify(
            x => x.EventNewsRepository.GetFirstOrDefaultAsync(
                It.Is<QueryOptions<EventNewsEntity>>(o =>
                    o.Include != null &&
                    o.AsNoTracking &&
                    o.AsSplitQuery)),
            Times.Once);
    }

    private GetPublishedEventNewsBySlugHandler CreateHandler() =>
       new(_mapperMock.Object, _mockRepositoryWrapper.Object);
}
