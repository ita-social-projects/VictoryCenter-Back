using AutoMapper;
using Moq;
using VictoryCenter.BLL.DTOs.Public.FeedbackHistories;
using VictoryCenter.BLL.Queries.Public.FeedbackHistories.GetPublished;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.FeedbackHistories;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.FeedbackHistories;

public class GetPublishedFeedbackHistoriesTests
{
    private readonly Mock<IMapper> _mapper = new();
    private readonly Mock<IRepositoryWrapper> _repositoryWrapper = new();
    private readonly Mock<IFeedbackHistoriesRepository> _repository = new();

    public GetPublishedFeedbackHistoriesTests()
    {
        _repositoryWrapper
            .SetupGet(wrapper => wrapper.FeedbackHistoriesRepository)
            .Returns(_repository.Object);
    }

    [Fact]
    public async Task Handle_ShouldQueryOnlyPublishedHistoriesOrderedByPriorityWithImage()
    {
        QueryOptions<FeedbackHistory>? capturedOptions = null;
        _repository
            .Setup(repository => repository.GetAllAsync(It.IsAny<QueryOptions<FeedbackHistory>>()))
            .Callback<QueryOptions<FeedbackHistory>?>(options => capturedOptions = options)
            .ReturnsAsync([]);
        _mapper
            .Setup(mapper => mapper.Map<List<PublishedFeedbackHistoryDto>>(It.IsAny<IEnumerable<FeedbackHistory>>()))
            .Returns([]);

        await CreateHandler().Handle(new GetPublishedFeedbackHistoriesQuery(), CancellationToken.None);

        Assert.NotNull(capturedOptions);
        Assert.NotNull(capturedOptions.Filter);
        var filter = capturedOptions.Filter.Compile();
        Assert.True(filter(new FeedbackHistory { Status = Status.Published }));
        Assert.False(filter(new FeedbackHistory { Status = Status.Draft }));
        Assert.NotNull(capturedOptions.OrderByASC);
        Assert.NotNull(capturedOptions.Include);
        Assert.True(capturedOptions.AsNoTracking);
    }

    [Fact]
    public async Task Handle_ShouldReturnMappedHistories()
    {
        List<FeedbackHistory> histories = [new() { Id = 1, Title = "Title", Story = "Story", Status = Status.Published }];
        List<PublishedFeedbackHistoryDto> dtos = [new() { Id = 1, Title = "Title", Story = "Story" }];
        _repository
            .Setup(repository => repository.GetAllAsync(It.IsAny<QueryOptions<FeedbackHistory>>()))
            .ReturnsAsync(histories);
        _mapper
            .Setup(mapper => mapper.Map<List<PublishedFeedbackHistoryDto>>(histories))
            .Returns(dtos);

        var result = await CreateHandler().Handle(new GetPublishedFeedbackHistoriesQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(dtos, result.Value);
    }

    [Fact]
    public async Task Handle_NoPublishedHistories_ShouldReturnEmptyList()
    {
        _repository
            .Setup(repository => repository.GetAllAsync(It.IsAny<QueryOptions<FeedbackHistory>>()))
            .ReturnsAsync([]);
        _mapper
            .Setup(mapper => mapper.Map<List<PublishedFeedbackHistoryDto>>(It.IsAny<IEnumerable<FeedbackHistory>>()))
            .Returns([]);

        var result = await CreateHandler().Handle(new GetPublishedFeedbackHistoriesQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }

    private GetPublishedFeedbackHistoriesHandler CreateHandler() =>
        new(_mapper.Object, _repositoryWrapper.Object);
}
