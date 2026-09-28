using AutoMapper;
using Moq;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.VideoReviews;
using VictoryCenter.BLL.Interfaces.Search;
using VictoryCenter.BLL.Queries.Admin.VideoReviews.Search;
using VictoryCenter.BLL.Services.Search;
using VictoryCenter.BLL.Validators.VideoReviews;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.VideoReviews;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.VideoReviews;

public class SearchVideoReviewTests
{
    [Fact]
    public async Task Handle_SearchQuery_ShouldMatchTitleOrLinkAndRejectNonMatch()
    {
        var mapper = new Mock<IMapper>();
        var repository = new Mock<IRepositoryWrapper>();
        var videoReviewRepository = new Mock<IVideoReviewsRepository>();
        QueryOptions<VideoReview>? capturedOptions = null;
        var entity = new VideoReview { Id = 1, Title = "Great Recovery", Link = "https://youtube.com/watch?v=123" };
        var dto = new VideoReviewDto { Id = 1, Title = "Great Recovery" };

        repository.SetupGet(x => x.VideoReviewsRepository).Returns(videoReviewRepository.Object);
        videoReviewRepository.Setup(x => x.GetAllAsync(It.IsAny<QueryOptions<VideoReview>>()))
            .Callback<QueryOptions<VideoReview>>(options => capturedOptions = options)
            .ReturnsAsync((QueryOptions<VideoReview> options) =>
                options.Filter!.Compile()(entity) ? [entity] : []);
        videoReviewRepository.Setup(x => x.CountAsync(It.IsAny<QueryOptions<VideoReview>>()))
            .ReturnsAsync((QueryOptions<VideoReview> options) => options.Filter!.Compile()(entity) ? 1 : 0);
        mapper.Setup(x => x.Map<List<VideoReviewDto>>(It.IsAny<IEnumerable<VideoReview>>()))
            .Returns([dto]);

        var handler = new SearchVideoReviewHandler(
            mapper.Object,
            repository.Object,
            new SearchVideoReviewValidator(),
            new SearchService<VideoReview>());

        var result = await handler.Handle(
            new SearchVideoReviewQuery(new SearchVideoReviewDto { SearchQuery = "Recovery" }),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(dto, Assert.Single(result.Value!.Items));
        Assert.NotNull(capturedOptions?.Filter);
        var predicate = capturedOptions.Filter.Compile();
        Assert.True(predicate(entity));
        Assert.False(predicate(new VideoReview { Title = "Unrelated", Link = "https://youtube.com/watch?v=999" }));

        var linkSearchResult = await handler.Handle(
            new SearchVideoReviewQuery(new SearchVideoReviewDto { SearchQuery = "youtube" }),
            CancellationToken.None);

        Assert.True(linkSearchResult.IsSuccess);
    }

    [Fact]
    public async Task Handle_InvalidQuery_ShouldReturnValidationError()
    {
        var handler = new SearchVideoReviewHandler(
            new Mock<IMapper>().Object,
            new Mock<IRepositoryWrapper>().Object,
            new SearchVideoReviewValidator(),
            new Mock<ISearchService<VideoReview>>().Object);

        var result = await handler.Handle(
            new SearchVideoReviewQuery(new SearchVideoReviewDto { SearchQuery = "" }),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains(
            ErrorMessagesConstants.PropertyIsRequired(nameof(SearchVideoReviewDto.SearchQuery)),
            result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_QueryTooShort_ShouldReturnValidationError()
    {
        var handler = new SearchVideoReviewHandler(
            new Mock<IMapper>().Object,
            new Mock<IRepositoryWrapper>().Object,
            new SearchVideoReviewValidator(),
            new Mock<ISearchService<VideoReview>>().Object);

        var result = await handler.Handle(
            new SearchVideoReviewQuery(new SearchVideoReviewDto { SearchQuery = "a" }),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_QueryTooLong_ShouldReturnValidationError()
    {
        var handler = new SearchVideoReviewHandler(
            new Mock<IMapper>().Object,
            new Mock<IRepositoryWrapper>().Object,
            new SearchVideoReviewValidator(),
            new Mock<ISearchService<VideoReview>>().Object);

        var result = await handler.Handle(
            new SearchVideoReviewQuery(new SearchVideoReviewDto
            {
                SearchQuery = new string('a', GlobalSearchConstants.DefaultSearchQueryMaxLength + 1),
            }),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
    }
}
