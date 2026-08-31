using AutoMapper;
using Moq;
using VictoryCenter.BLL.DTOs.Admin.FeedbackReviews;
using VictoryCenter.BLL.Queries.Admin.FeedbackReviews.GetByFilters;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.FeedbackReviews;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.FeedbackReviews;

public class GetFeedbackReviewsByFiltersTests
{
    private readonly Mock<IMapper> _mapper = new();
    private readonly Mock<IRepositoryWrapper> _repositoryWrapper = new();
    private readonly Mock<IFeedbackReviewsRepository> _repository = new();

    public GetFeedbackReviewsByFiltersTests()
    {
        _repositoryWrapper
            .SetupGet(wrapper => wrapper.FeedbackReviewsRepository)
            .Returns(_repository.Object);
    }

    [Fact]
    public async Task Handle_ExistingReviews_ReturnsItemsAndTotalCount()
    {
        SetupRepository(Reviews(), totalCount: 2);
        var handler = CreateHandler();

        var result = await handler.Handle(Query(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.TotalItemsCount);
        Assert.Equal(2, result.Value.Items.Count());
    }

    [Fact]
    public async Task Handle_NoReviews_ReturnsEmptyResult()
    {
        SetupRepository([], totalCount: 0);
        var handler = CreateHandler();

        var result = await handler.Handle(Query(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value.Items);
        Assert.Equal(0, result.Value.TotalItemsCount);
    }

    [Fact]
    public async Task Handle_NoFilterProvided_UsesDefaultPagingAndOrdersByPriority()
    {
        QueryOptions<FeedbackReview>? capturedOptions = null;

        _repository
            .Setup(repository => repository.GetAllAsync(It.IsAny<QueryOptions<FeedbackReview>>()))
            .Callback<QueryOptions<FeedbackReview>?>(options => capturedOptions = options)
            .ReturnsAsync([]);

        _repository
            .Setup(repository => repository.CountAsync(It.IsAny<QueryOptions<FeedbackReview>>()))
            .ReturnsAsync(0);

        _mapper
            .Setup(mapper => mapper.Map<FeedbackReviewDto[]>(It.IsAny<IEnumerable<FeedbackReview>>()))
            .Returns([]);

        var handler = CreateHandler();

        await handler.Handle(Query(), CancellationToken.None);

        Assert.NotNull(capturedOptions);
        Assert.True(capturedOptions.AsNoTracking);
        Assert.NotNull(capturedOptions.OrderByASC);
        Assert.Equal(0, capturedOptions.Offset);
        Assert.Equal(20, capturedOptions.Limit);
    }

    [Fact]
    public async Task Handle_FilterProvided_AppliesOffsetAndLimit()
    {
        QueryOptions<FeedbackReview>? capturedOptions = null;

        _repository
            .Setup(repository => repository.GetAllAsync(It.IsAny<QueryOptions<FeedbackReview>>()))
            .Callback<QueryOptions<FeedbackReview>?>(options => capturedOptions = options)
            .ReturnsAsync([]);

        _repository
            .Setup(repository => repository.CountAsync(It.IsAny<QueryOptions<FeedbackReview>>()))
            .ReturnsAsync(0);

        _mapper
            .Setup(mapper => mapper.Map<FeedbackReviewDto[]>(It.IsAny<IEnumerable<FeedbackReview>>()))
            .Returns([]);

        var handler = CreateHandler();

        await handler.Handle(
            new GetFeedbackReviewsByFiltersQuery(new FeedbackReviewsFilterDto { Offset = 10, Limit = 5 }),
            CancellationToken.None);

        Assert.NotNull(capturedOptions);
        Assert.Equal(10, capturedOptions.Offset);
        Assert.Equal(5, capturedOptions.Limit);
    }

    private void SetupRepository(List<FeedbackReview> reviews, int totalCount)
    {
        _repository
            .Setup(repository => repository.GetAllAsync(It.IsAny<QueryOptions<FeedbackReview>>()))
            .ReturnsAsync(reviews);

        _repository
            .Setup(repository => repository.CountAsync(It.IsAny<QueryOptions<FeedbackReview>>()))
            .ReturnsAsync(totalCount);

        _mapper
            .Setup(mapper => mapper.Map<FeedbackReviewDto[]>(It.IsAny<IEnumerable<FeedbackReview>>()))
            .Returns((IEnumerable<FeedbackReview> items) =>
                items.Select(review => new FeedbackReviewDto
                {
                    Id = review.Id,
                    AuthorName = review.AuthorName,
                    Text = review.Text,
                    Status = review.Status,
                    Priority = review.Priority,
                    CreatedAt = review.CreatedAt
                }).ToArray());
    }

    private GetFeedbackReviewsByFiltersHandler CreateHandler() =>
        new(_mapper.Object, _repositoryWrapper.Object);

    private static GetFeedbackReviewsByFiltersQuery Query() =>
        new(new FeedbackReviewsFilterDto());

    private static List<FeedbackReview> Reviews() =>
    [
        new() { Id = 1, AuthorName = "Anastasiia", Text = "First review", Status = Status.Published, Priority = 0 },
        new() { Id = 2, AuthorName = "Viktoriia", Text = "Second review", Status = Status.Draft, Priority = 1 }
    ];
}
