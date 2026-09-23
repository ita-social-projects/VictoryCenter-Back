using System.Linq.Expressions;
using AutoMapper;
using Moq;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.FeedbackReviews;
using VictoryCenter.BLL.Interfaces.Search;
using VictoryCenter.BLL.Queries.Admin.FeedbackReviews.Search;
using VictoryCenter.BLL.Services.Search.Helpers;
using VictoryCenter.BLL.Validators.FeedbackReviews;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.FeedbackReviews;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.FeedbackReviews;

public class SearchFeedbackReviewTests
{
    [Fact]
    public async Task Handle_ExistingAuthorName_ShouldReturnReview()
    {
        var mapper = new Mock<IMapper>();
        var repository = new Mock<IRepositoryWrapper>();
        var reviewRepository = new Mock<IFeedbackReviewsRepository>();
        var searchService = new Mock<ISearchService<FeedbackReview>>();
        var entity = new FeedbackReview { Id = 1, AuthorName = "Anastasiia", Text = "Great" };
        var dto = new FeedbackReviewDto { Id = 1, AuthorName = "Anastasiia", Text = "Great" };

        Expression<Func<FeedbackReview, bool>> authorExpression =
            review => review.AuthorName != null &&
                      review.AuthorName.Contains("Ana");

        Expression<Func<FeedbackReview, bool>> textExpression =
            review => review.Text != null &&
                      review.Text.Contains("Ana");

        searchService
            .SetupSequence(x => x.CreateSearchExpression(
                It.IsAny<SearchTerm<FeedbackReview>>()))
            .Returns(authorExpression)
            .Returns(textExpression);

        repository.SetupGet(x => x.FeedbackReviewsRepository).Returns(reviewRepository.Object);
        reviewRepository.Setup(x => x.GetAllAsync(It.IsAny<QueryOptions<FeedbackReview>>()))
            .ReturnsAsync([entity]);
        reviewRepository.Setup(x => x.CountAsync(It.IsAny<QueryOptions<FeedbackReview>>()))
            .ReturnsAsync(1);
        mapper.Setup(x => x.Map<List<FeedbackReviewDto>>(It.IsAny<IEnumerable<FeedbackReview>>()))
            .Returns([dto]);

        var handler = new SearchFeedbackReviewHandler(
            mapper.Object,
            repository.Object,
            new SearchFeedbackReviewValidator(),
            searchService.Object);

        var result = await handler.Handle(
            new SearchFeedbackReviewQuery(new SearchFeedbackReviewDto { SearchQuery = "Ana" }),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(dto, Assert.Single(result.Value!.Items));
    }

    [Fact]
    public async Task Handle_InvalidQuery_ShouldReturnValidationError()
    {
        var handler = new SearchFeedbackReviewHandler(
            new Mock<IMapper>().Object,
            new Mock<IRepositoryWrapper>().Object,
            new SearchFeedbackReviewValidator(),
            new Mock<ISearchService<FeedbackReview>>().Object);

        var result = await handler.Handle(new SearchFeedbackReviewQuery(new SearchFeedbackReviewDto { SearchQuery = "" }), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains(
            ErrorMessagesConstants.PropertyIsRequired(nameof(SearchFeedbackReviewDto.SearchQuery)),
            result.Errors[0].Message);
    }
}
