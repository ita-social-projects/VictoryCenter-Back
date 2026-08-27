using AutoMapper;
using FluentResults;
using Moq;
using VictoryCenter.BLL.DTOs.Admin.FeedbackHistories;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Queries.Admin.FeedbackHistories.GetAll;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.FeedbackHistories;

public class GetAllFeedbackHistoriesTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepoWrapper;

    private readonly List<FeedbackHistory> _feedbackHistories = [
        new FeedbackHistory { Id = 1L, Title = "Title 1", Story = "Story 1", ImageId = 10L, CreatedAt = DateTimeOffset.UtcNow.AddMinutes(-15) },
        new FeedbackHistory { Id = 2L, Title = "Title 2", Story = "Story 2", ImageId = null, CreatedAt = DateTimeOffset.UtcNow.AddMinutes(-5) }
    ];

    private readonly List<FeedbackHistoryDto> _feedbackHistoryDtos = [
        new FeedbackHistoryDto { Id = 1L, Title = "Title 1", Story = "Story 1", Image = new ImageDto { Id = 10L } },
        new FeedbackHistoryDto { Id = 2L, Title = "Title 2", Story = "Story 2", Image = null }
    ];

    public GetAllFeedbackHistoriesTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockRepoWrapper = new Mock<IRepositoryWrapper>();
    }

    [Fact]
    public async Task Handle_WhenEntitiesExist_ShouldReturnListOfFeedbackHistoryDtos()
    {
        _mockRepoWrapper.Setup(r => r.FeedbackHistoriesRepository.GetAllAsync(It.IsAny<QueryOptions<FeedbackHistory>>()))
            .ReturnsAsync(_feedbackHistories);

        _mockMapper.Setup(m => m.Map<IEnumerable<FeedbackHistoryDto>>(It.IsAny<IEnumerable<FeedbackHistory>>()))
            .Returns(_feedbackHistoryDtos);

        var handler = new GetAllFeedbackHistoriesHandler(_mockMapper.Object, _mockRepoWrapper.Object);

        Result<IEnumerable<FeedbackHistoryDto>> result = await handler.Handle(new GetAllFeedbackHistoriesQuery(), CancellationToken.None);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(_feedbackHistoryDtos.Count, result.Value.Count());
        Assert.Equal(_feedbackHistoryDtos, result.Value);
    }

    [Fact]
    public async Task Handle_WhenNoEntitiesExist_ShouldReturnEmptyList()
    {
        _mockRepoWrapper.Setup(r => r.FeedbackHistoriesRepository.GetAllAsync(It.IsAny<QueryOptions<FeedbackHistory>>()))
            .ReturnsAsync(new List<FeedbackHistory>());

        _mockMapper.Setup(m => m.Map<IEnumerable<FeedbackHistoryDto>>(It.IsAny<IEnumerable<FeedbackHistory>>()))
            .Returns(new List<FeedbackHistoryDto>());

        var handler = new GetAllFeedbackHistoriesHandler(_mockMapper.Object, _mockRepoWrapper.Object);

        Result<IEnumerable<FeedbackHistoryDto>> result = await handler.Handle(new GetAllFeedbackHistoriesQuery(), CancellationToken.None);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }
}