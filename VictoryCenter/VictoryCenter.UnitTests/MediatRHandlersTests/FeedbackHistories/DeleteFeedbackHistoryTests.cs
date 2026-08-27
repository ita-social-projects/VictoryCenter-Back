using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.FeedbackHistories.Delete;
using VictoryCenter.BLL.Constants;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.FeedbackHistories;

public class DeleteFeedbackHistoryTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepoWrapper;

    private readonly FeedbackHistory _existingFeedbackHistory = new()
    {
        Id = 1L,
        Title = "Story Title",
        Story = "Story content to be deleted",
        ImageId = null,
        CreatedAt = DateTimeOffset.UtcNow.AddMinutes(-30)
    };

    public DeleteFeedbackHistoryTests()
    {
        _mockRepoWrapper = new Mock<IRepositoryWrapper>();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(999)]
    public async Task Handle_EntityNotFound_ShouldReturnFailure(long historyId)
    {
        SetupRepositoryWrapper(null);
        var command = new DeleteFeedbackHistoryCommand(historyId);
        var handler = new DeleteFeedbackHistoryHandler(_mockRepoWrapper.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.True(result.IsFailed);
        Assert.Equal(ErrorMessagesConstants.NotFound(historyId, typeof(FeedbackHistory)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_EntityExists_ShouldReturnOk()
    {
        SetupRepositoryWrapper(_existingFeedbackHistory, 1);
        var command = new DeleteFeedbackHistoryCommand(_existingFeedbackHistory.Id);
        var handler = new DeleteFeedbackHistoryHandler(_mockRepoWrapper.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(_existingFeedbackHistory.Id, result.Value);

        _mockRepoWrapper.Verify(r => r.FeedbackHistoriesRepository.Delete(It.IsAny<FeedbackHistory>()), Times.Once);
        _mockRepoWrapper.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_SaveChangesFails_ShouldReturnFailure()
    {
        SetupRepositoryWrapper(_existingFeedbackHistory, 0);
        var command = new DeleteFeedbackHistoryCommand(_existingFeedbackHistory.Id);
        var handler = new DeleteFeedbackHistoryHandler(_mockRepoWrapper.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.True(result.IsFailed);
        Assert.Equal(ErrorMessagesConstants.FailedToDeleteEntity(typeof(FeedbackHistory)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_DbExceptionThrown_ShouldReturnFailure()
    {
        _mockRepoWrapper.Setup(r => r.FeedbackHistoriesRepository.GetFirstOrDefaultAsync(
            It.IsAny<QueryOptions<FeedbackHistory>>())).ReturnsAsync(_existingFeedbackHistory);

        _mockRepoWrapper.Setup(r => r.SaveChangesAsync()).ThrowsAsync(new DbUpdateException());

        var command = new DeleteFeedbackHistoryCommand(_existingFeedbackHistory.Id);
        var handler = new DeleteFeedbackHistoryHandler(_mockRepoWrapper.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.True(result.IsFailed);
        Assert.Equal(ErrorMessagesConstants.FailedToDeleteEntity(typeof(FeedbackHistory)), result.Errors[0].Message);
    }

    private void SetupRepositoryWrapper(FeedbackHistory? entityToDelete, int saveResult = 1)
    {
        _mockRepoWrapper.Setup(r => r.FeedbackHistoriesRepository.GetFirstOrDefaultAsync(
            It.IsAny<QueryOptions<FeedbackHistory>>())).ReturnsAsync(entityToDelete);

        _mockRepoWrapper.Setup(r => r.SaveChangesAsync()).ReturnsAsync(saveResult);
        _mockRepoWrapper.Setup(r => r.FeedbackHistoriesRepository.Delete(It.IsAny<FeedbackHistory>()));
    }
}