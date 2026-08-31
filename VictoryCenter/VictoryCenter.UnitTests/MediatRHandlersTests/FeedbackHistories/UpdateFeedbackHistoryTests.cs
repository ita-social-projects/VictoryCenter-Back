using AutoMapper;
using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.FeedbackHistories.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.FeedbackHistories;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.FeedbackHistories;

public class UpdateFeedbackHistoryTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IValidator<UpdateFeedbackHistoryCommand>> _validatorMock;
    private readonly TimeProvider _timeProvider = TimeProvider.System;

    private readonly FeedbackHistory _existingHistory = new()
    {
        Id = 1,
        Title = "Old Title",
        Story = "Old Story Text",
        ImageId = null,
        CreatedAt = DateTimeOffset.UtcNow.AddMinutes(-30),
        Priority = 1,
        Status = Status.Draft
    };

    private readonly FeedbackHistory _updatedHistory = new()
    {
        Id = 1,
        Title = "New Title",
        Story = "New Story Text",
        ImageId = null,
        CreatedAt = DateTimeOffset.UtcNow.AddMinutes(-30),
        Priority = 2,
        Status = Status.Published
    };

    private readonly UpdateFeedbackHistoryDto _updateDto = new()
    {
        Title = "New Title",
        Story = "New Story Text",
        ImageId = null,
        Priority = 2,
        Status = Status.Published
    };

    private readonly FeedbackHistoryDto _resultDto = new()
    {
        Id = 1,
        Title = "New Title",
        Story = "New Story Text",
        Image = null,
        Priority = 2,
        Status = Status.Published
    };

    public UpdateFeedbackHistoryTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _validatorMock = new Mock<IValidator<UpdateFeedbackHistoryCommand>>();

        _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<UpdateFeedbackHistoryCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldUpdateAndReturnDto()
    {
        SetupRepositoryWrapper(_existingHistory, 1);
        SetupMapper(_updatedHistory, _resultDto);

        var handler = new UpdateFeedbackHistoryHandler(_mockMapper.Object, _mockRepositoryWrapper.Object, _validatorMock.Object, _timeProvider);

        Result<FeedbackHistoryDto> result = await handler.Handle(
            new UpdateFeedbackHistoryCommand(_updateDto, _existingHistory.Id), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(_resultDto, result.Value);
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldSetUpdatedAtFromTimeProvider()
    {
        var expectedTime = new DateTimeOffset(2025, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var mockTimeProvider = new Mock<TimeProvider>();
        mockTimeProvider.Setup(t => t.GetUtcNow()).Returns(expectedTime);

        var entityToUpdate = new FeedbackHistory { Id = _existingHistory.Id };
        SetupRepositoryWrapper(_existingHistory, 1);
        _mockMapper.Setup(x => x.Map(It.IsAny<UpdateFeedbackHistoryDto>(), It.IsAny<FeedbackHistory>()))
            .Returns(entityToUpdate);
        _mockMapper.Setup(x => x.Map<FeedbackHistoryDto>(It.IsAny<FeedbackHistory>()))
            .Returns(_resultDto);

        var handler = new UpdateFeedbackHistoryHandler(_mockMapper.Object, _mockRepositoryWrapper.Object, _validatorMock.Object, mockTimeProvider.Object);

        Result<FeedbackHistoryDto> result = await handler.Handle(
            new UpdateFeedbackHistoryCommand(_updateDto, _existingHistory.Id), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(expectedTime, entityToUpdate.UpdatedAt);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task Handle_EntityNotFound_ShouldReturnNotFoundError(long testId)
    {
        SetupRepositoryWrapper(null);

        var handler = new UpdateFeedbackHistoryHandler(_mockMapper.Object, _mockRepositoryWrapper.Object, _validatorMock.Object, _timeProvider);

        Result<FeedbackHistoryDto> result = await handler.Handle(
            new UpdateFeedbackHistoryCommand(_updateDto, testId), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.NotFound(testId, typeof(FeedbackHistory)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_SaveChangesFails_ShouldReturnFailureError()
    {
        SetupRepositoryWrapper(_existingHistory, 0);
        SetupMapper(_existingHistory, _resultDto);

        var handler = new UpdateFeedbackHistoryHandler(_mockMapper.Object, _mockRepositoryWrapper.Object, _validatorMock.Object, _timeProvider);

        Result<FeedbackHistoryDto> result = await handler.Handle(
            new UpdateFeedbackHistoryCommand(_updateDto, _existingHistory.Id), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessagesConstants.FailedToUpdateEntity(typeof(FeedbackHistory)), result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_DbUpdateException_ShouldReturnFailure()
    {
        SetupRepositoryWrapper(_existingHistory);
        SetupMapper(_existingHistory, _resultDto);

        _mockRepositoryWrapper.Setup(x => x.SaveChangesAsync())
            .ThrowsAsync(new DbUpdateException("Database error"));

        var handler = new UpdateFeedbackHistoryHandler(_mockMapper.Object, _mockRepositoryWrapper.Object, _validatorMock.Object, _timeProvider);

        Result<FeedbackHistoryDto> result = await handler.Handle(
            new UpdateFeedbackHistoryCommand(_updateDto, _existingHistory.Id), CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Null(result.ValueOrDefault);
        Assert.Equal(ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(FeedbackHistory)), result.Errors[0].Message);
    }

    private void SetupMapper(FeedbackHistory updatedEntity, FeedbackHistoryDto resultDto)
    {
        _mockMapper.Setup(x => x.Map(It.IsAny<UpdateFeedbackHistoryDto>(), It.IsAny<FeedbackHistory>()))
            .Returns(updatedEntity);

        _mockMapper.Setup(x => x.Map<FeedbackHistoryDto>(It.IsAny<FeedbackHistory>()))
            .Returns(resultDto);
    }

    private void SetupRepositoryWrapper(FeedbackHistory? entityToFind, int saveResult = 1)
    {
        _mockRepositoryWrapper.Setup(x => x.FeedbackHistoriesRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<FeedbackHistory>>()))
            .ReturnsAsync(entityToFind);

        _mockRepositoryWrapper.Setup(x => x.SaveChangesAsync()).ReturnsAsync(saveResult);
        _mockRepositoryWrapper.Setup(x => x.FeedbackHistoriesRepository.Update(It.IsAny<FeedbackHistory>()));
    }
}