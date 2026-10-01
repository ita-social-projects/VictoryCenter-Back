using AutoMapper;
using Moq;
using VictoryCenter.BLL.DTOs.Public.FeedbackReviews;
using VictoryCenter.BLL.Queries.Public.FeedbackReviews.GetPublished;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.FeedbackReviews;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.FeedbackReviews;

public class GetPublishedFeedbackReviewsTests
{
    private readonly Mock<IMapper> _mapper = new();
    private readonly Mock<IRepositoryWrapper> _repositoryWrapper = new();
    private readonly Mock<IFeedbackReviewsRepository> _repository = new();

    public GetPublishedFeedbackReviewsTests()
    {
        _repositoryWrapper
            .SetupGet(wrapper => wrapper.FeedbackReviewsRepository)
            .Returns(_repository.Object);
    }

    [Fact]
    public async Task Handle_ShouldQueryOnlyPublishedReviewsOrderedByPriority()
    {
        QueryOptions<FeedbackReview>? capturedOptions = null;
        _repository
            .Setup(repository => repository.GetAllAsync(It.IsAny<QueryOptions<FeedbackReview>>()))
            .Callback<QueryOptions<FeedbackReview>?>(options => capturedOptions = options)
            .ReturnsAsync([]);
        _mapper
            .Setup(mapper => mapper.Map<List<PublishedFeedbackReviewDto>>(It.IsAny<IEnumerable<FeedbackReview>>()))
            .Returns([]);

        await CreateHandler().Handle(new GetPublishedFeedbackReviewsQuery(), CancellationToken.None);

        Assert.NotNull(capturedOptions);
        Assert.NotNull(capturedOptions.Filter);
        var filter = capturedOptions.Filter.Compile();
        Assert.True(filter(new FeedbackReview { Status = Status.Published }));
        Assert.False(filter(new FeedbackReview { Status = Status.Draft }));
        Assert.NotNull(capturedOptions.OrderByASC);
        Assert.True(capturedOptions.AsNoTracking);
    }

    [Fact]
    public async Task Handle_ShouldReturnMappedReviews()
    {
        List<FeedbackReview> reviews = [new() { Id = 1, AuthorName = "Author", Text = "Review text", Status = Status.Published }];
        List<PublishedFeedbackReviewDto> dtos = [new() { Id = 1, AuthorName = "Author", Text = "Review text" }];
        _repository
            .Setup(repository => repository.GetAllAsync(It.IsAny<QueryOptions<FeedbackReview>>()))
            .ReturnsAsync(reviews);
        _mapper
            .Setup(mapper => mapper.Map<List<PublishedFeedbackReviewDto>>(reviews))
            .Returns(dtos);

        var result = await CreateHandler().Handle(new GetPublishedFeedbackReviewsQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(dtos, result.Value);
    }

    [Fact]
    public async Task Handle_NoPublishedReviews_ShouldReturnEmptyList()
    {
        _repository
            .Setup(repository => repository.GetAllAsync(It.IsAny<QueryOptions<FeedbackReview>>()))
            .ReturnsAsync([]);
        _mapper
            .Setup(mapper => mapper.Map<List<PublishedFeedbackReviewDto>>(It.IsAny<IEnumerable<FeedbackReview>>()))
            .Returns([]);

        var result = await CreateHandler().Handle(new GetPublishedFeedbackReviewsQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }

    private GetPublishedFeedbackReviewsHandler CreateHandler() =>
        new(_mapper.Object, _repositoryWrapper.Object);
}
