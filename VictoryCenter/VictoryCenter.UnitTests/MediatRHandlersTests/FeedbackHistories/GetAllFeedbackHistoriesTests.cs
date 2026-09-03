using AutoMapper;
using FluentResults;
using Moq;
using VictoryCenter.BLL.DTOs.Admin.FeedbackHistories;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Queries.Admin.FeedbackHistories.GetAll;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.FeedbackHistories;

public class GetAllFeedbackHistoriesTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepoWrapper;

    private readonly List<FeedbackHistory> _feedbackHistories = [
        new FeedbackHistory { Id = 1, Title = "Title 1", Story = "Story 1", ImageId = 10, CreatedAt = DateTimeOffset.UtcNow.AddMinutes(-15), Status = Status.Draft },
        new FeedbackHistory { Id = 2, Title = "Title 2", Story = "Story 2", ImageId = null, CreatedAt = DateTimeOffset.UtcNow.AddMinutes(-5), Status = Status.Published }
    ];

    private readonly List<FeedbackHistoryDto> _feedbackHistoryDtos = [
        new FeedbackHistoryDto { Id = 1, Title = "Title 1", Story = "Story 1", Image = new ImageDto { Id = 10 }, Status = Status.Draft },
        new FeedbackHistoryDto { Id = 2, Title = "Title 2", Story = "Story 2", Image = null, Status = Status.Published }
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

        _mockRepoWrapper.Setup(r => r.ImageRepository.GetAllAsync(It.IsAny<QueryOptions<Image>>()))
            .ReturnsAsync([new Image { Id = 10 }]);

        _mockMapper.Setup(m => m.Map<IEnumerable<FeedbackHistoryDto>>(It.IsAny<IEnumerable<FeedbackHistory>>()))
            .Returns(_feedbackHistoryDtos);

        var handler = new GetAllFeedbackHistoriesHandler(_mockMapper.Object, _mockRepoWrapper.Object);

        Result<IEnumerable<FeedbackHistoryDto>> result = await handler.Handle(new GetAllFeedbackHistoriesQuery(), CancellationToken.None);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(_feedbackHistoryDtos.Count, result.Value.Count());
        Assert.Equal(_feedbackHistoryDtos, result.Value);
        _mockRepoWrapper.Verify(r => r.ImageRepository.GetAllAsync(It.IsAny<QueryOptions<Image>>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenEntitiesHaveNoImageIds_ShouldNotCallImageRepository()
    {
        List<FeedbackHistory> historiesWithoutImages = [
            new() { Id = 1, Title = "Title 1", Story = "Story 1", ImageId = null, Status = Status.Draft },
            new() { Id = 2, Title = "Title 2", Story = "Story 2", ImageId = null, Status = Status.Published }
        ];

        List<FeedbackHistoryDto> dtosWithoutImages = [
            new() { Id = 1, Title = "Title 1", Story = "Story 1", Image = null, Status = Status.Draft },
            new() { Id = 2, Title = "Title 2", Story = "Story 2", Image = null, Status = Status.Published }
        ];

        _mockRepoWrapper.Setup(r => r.FeedbackHistoriesRepository.GetAllAsync(It.IsAny<QueryOptions<FeedbackHistory>>()))
            .ReturnsAsync(historiesWithoutImages);

        _mockMapper.Setup(m => m.Map<IEnumerable<FeedbackHistoryDto>>(It.IsAny<IEnumerable<FeedbackHistory>>()))
            .Returns(dtosWithoutImages);

        var handler = new GetAllFeedbackHistoriesHandler(_mockMapper.Object, _mockRepoWrapper.Object);

        Result<IEnumerable<FeedbackHistoryDto>> result = await handler.Handle(new GetAllFeedbackHistoriesQuery(), CancellationToken.None);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(dtosWithoutImages.Count, result.Value.Count());
        _mockRepoWrapper.Verify(r => r.ImageRepository.GetAllAsync(It.IsAny<QueryOptions<Image>>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenNoEntitiesExist_ShouldReturnEmptyList()
    {
        _mockRepoWrapper.Setup(r => r.FeedbackHistoriesRepository.GetAllAsync(It.IsAny<QueryOptions<FeedbackHistory>>()))
            .ReturnsAsync([]);

        _mockMapper.Setup(m => m.Map<IEnumerable<FeedbackHistoryDto>>(It.IsAny<IEnumerable<FeedbackHistory>>()))
            .Returns([]);

        var handler = new GetAllFeedbackHistoriesHandler(_mockMapper.Object, _mockRepoWrapper.Object);

        Result<IEnumerable<FeedbackHistoryDto>> result = await handler.Handle(new GetAllFeedbackHistoriesQuery(), CancellationToken.None);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
        _mockRepoWrapper.Verify(r => r.ImageRepository.GetAllAsync(It.IsAny<QueryOptions<Image>>()), Times.Never);
    }
}
