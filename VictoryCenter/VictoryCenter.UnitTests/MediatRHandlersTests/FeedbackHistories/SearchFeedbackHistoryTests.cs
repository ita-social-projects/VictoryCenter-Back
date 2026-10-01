using AutoMapper;
using Moq;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.FeedbackHistories;
using VictoryCenter.BLL.Interfaces.Search;
using VictoryCenter.BLL.Queries.Admin.FeedbackHistories.Search;
using VictoryCenter.BLL.Services.Search;
using VictoryCenter.BLL.Validators.FeedbackHistories;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.FeedbackHistories;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.FeedbackHistories;

public class SearchFeedbackHistoryTests
{
    [Fact]
    public async Task Handle_SearchQuery_ShouldMatchTitleOrStoryAndRejectNonMatch()
    {
        var mapper = new Mock<IMapper>();
        var repository = new Mock<IRepositoryWrapper>();
        var historyRepository = new Mock<IFeedbackHistoriesRepository>();
        QueryOptions<FeedbackHistory>? capturedOptions = null;
        var entity = new FeedbackHistory { Id = 1, Title = "Recovery", Story = "A story about resilience" };
        var dto = new FeedbackHistoryDto { Id = 1, Title = "Recovery" };

        repository.SetupGet(x => x.FeedbackHistoriesRepository).Returns(historyRepository.Object);
        historyRepository.Setup(x => x.GetAllAsync(It.IsAny<QueryOptions<FeedbackHistory>>()))
            .Callback<QueryOptions<FeedbackHistory>>(options => capturedOptions = options)
            .ReturnsAsync((QueryOptions<FeedbackHistory> options) =>
                options.Filter!.Compile()(entity) ? [entity] : []);
        historyRepository.Setup(x => x.CountAsync(It.IsAny<QueryOptions<FeedbackHistory>>()))
            .ReturnsAsync((QueryOptions<FeedbackHistory> options) => options.Filter!.Compile()(entity) ? 1 : 0);
        mapper.Setup(x => x.Map<List<FeedbackHistoryDto>>(It.IsAny<IEnumerable<FeedbackHistory>>()))
            .Returns([dto]);

        var handler = new SearchFeedbackHistoryHandler(
            mapper.Object,
            repository.Object,
            new SearchFeedbackHistoryValidator(),
            new SearchService<FeedbackHistory>());

        var result = await handler.Handle(
            new SearchFeedbackHistoryQuery(new SearchFeedbackHistoryDto { SearchQuery = "Recovery" }),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(dto, Assert.Single(result.Value!.Items));
        Assert.NotNull(capturedOptions?.Filter);
        var predicate = capturedOptions.Filter.Compile();
        Assert.True(predicate(entity));
        Assert.False(predicate(new FeedbackHistory { Title = "Unrelated", Story = "Different content" }));

        var storySearchResult = await handler.Handle(
            new SearchFeedbackHistoryQuery(new SearchFeedbackHistoryDto { SearchQuery = "resilience" }),
            CancellationToken.None);

        Assert.True(storySearchResult.IsSuccess);
    }

    [Fact]
    public async Task Handle_InvalidQuery_ShouldReturnValidationError()
    {
        var handler = new SearchFeedbackHistoryHandler(
            new Mock<IMapper>().Object,
            new Mock<IRepositoryWrapper>().Object,
            new SearchFeedbackHistoryValidator(),
            new Mock<ISearchService<FeedbackHistory>>().Object);

        var result = await handler.Handle(new SearchFeedbackHistoryQuery(new SearchFeedbackHistoryDto { SearchQuery = "" }), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains(
            ErrorMessagesConstants.PropertyIsRequired(nameof(SearchFeedbackHistoryDto.SearchQuery)),
            result.Errors[0].Message);
    }
}