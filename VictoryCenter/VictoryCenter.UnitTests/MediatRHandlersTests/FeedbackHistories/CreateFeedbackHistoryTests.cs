using AutoMapper;
using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.FeedbackHistories.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.FeedbackHistories;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.FeedbackHistories;

public class CreateFeedbackHistoryTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IValidator<CreateFeedbackHistoryCommand>> _validatorMock;
    private readonly TimeProvider _timeProvider = TimeProvider.System;

    private readonly CreateFeedbackHistoryDto _createFeedbackHistoryDto = new()
    {
        Title = "Successful Recovery",
        Story = "Detailed feedback story text describing the experience.",
        ImageId = null,
        Priority = 1,
        Status = Status.Draft
    };

    private readonly FeedbackHistory _feedbackHistory = new()
    {
        Id = 1,
        Title = "Successful Recovery",
        Story = "Detailed feedback story text describing the experience.",
        ImageId = null,
        CreatedAt = DateTimeOffset.UtcNow.AddMinutes(-5),
        Priority = 1,
        Status = Status.Draft
    };

    private readonly FeedbackHistoryDto _feedbackHistoryDto = new()
    {
        Id = 1,
        Title = "Successful Recovery",
        Story = "Detailed feedback story text describing the experience.",
        Image = null,
        Priority = 1,
        Status = Status.Draft
    };

    public CreateFeedbackHistoryTests()
    {
        _mapperMock = new Mock<IMapper>();
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _validatorMock = new Mock<IValidator<CreateFeedbackHistoryCommand>>();
    }

    [Fact]
    public async Task Handle_WhenCreationIsValid_ShouldReturnFeedbackHistoryDto()
    {
        SetupDependencies(_feedbackHistoryDto, _feedbackHistory, 1);
        var handler = new CreateFeedbackHistoryHandler(_mapperMock.Object, _repositoryWrapperMock.Object, _validatorMock.Object, _timeProvider);

        Result<FeedbackHistoryDto> result =
            await handler.Handle(new CreateFeedbackHistoryCommand(_createFeedbackHistoryDto), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(_feedbackHistoryDto, result.Value);
    }

    [Fact]
    public async Task Handle_WhenCreationIsValid_ShouldSetCreatedAtFromTimeProvider()
    {
        var expectedTime = new DateTimeOffset(2025, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var mockTimeProvider = new Mock<TimeProvider>();
        mockTimeProvider.Setup(t => t.GetUtcNow()).Returns(expectedTime);

        var feedbackHistory = new FeedbackHistory();
        _mapperMock.Setup(m => m.Map<FeedbackHistory>(It.IsAny<CreateFeedbackHistoryDto>())).Returns(feedbackHistory);
        _mapperMock.Setup(m => m.Map<FeedbackHistoryDto>(It.IsAny<FeedbackHistory>())).Returns(_feedbackHistoryDto);
        _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<CreateFeedbackHistoryCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _repositoryWrapperMock.Setup(r => r.FeedbackHistoriesRepository.CreateAsync(It.IsAny<FeedbackHistory>()))
            .ReturnsAsync(feedbackHistory);
        _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        var handler = new CreateFeedbackHistoryHandler(_mapperMock.Object, _repositoryWrapperMock.Object, _validatorMock.Object, mockTimeProvider.Object);

        var result = await handler.Handle(new CreateFeedbackHistoryCommand(_createFeedbackHistoryDto), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(expectedTime, feedbackHistory.CreatedAt);
    }

    [Fact]
    public async Task Handle_WhenSaveChangesFails_ShouldReturnFailure()
    {
        var failMessage = ErrorMessagesConstants.FailedToCreateEntity(typeof(FeedbackHistory));
        SetupDependencies(_feedbackHistoryDto, _feedbackHistory, 0);

        var handler = new CreateFeedbackHistoryHandler(_mapperMock.Object, _repositoryWrapperMock.Object, _validatorMock.Object, _timeProvider);

        Result<FeedbackHistoryDto> result =
            await handler.Handle(new CreateFeedbackHistoryCommand(_createFeedbackHistoryDto), CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Null(result.ValueOrDefault);
        Assert.Equal(failMessage, result.Errors[0].Message);
    }

    [Fact]
    public async Task Handle_WhenDbExceptionThrown_ShouldReturnFailure()
    {
        SetupDependencies(_feedbackHistoryDto, _feedbackHistory, 1);
        _repositoryWrapperMock
            .Setup(r => r.FeedbackHistoriesRepository.CreateAsync(It.IsAny<FeedbackHistory>()))
            .ThrowsAsync(new DbUpdateException("Database error"));

        var failMessage = ErrorMessagesConstants.FailedToCreateEntityInDatabase(typeof(FeedbackHistory));
        var handler = new CreateFeedbackHistoryHandler(_mapperMock.Object, _repositoryWrapperMock.Object, _validatorMock.Object, _timeProvider);

        Result<FeedbackHistoryDto> result =
            await handler.Handle(new CreateFeedbackHistoryCommand(_createFeedbackHistoryDto), CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Null(result.ValueOrDefault);
        Assert.Equal(failMessage, result.Errors[0].Message);
    }

    private void SetupDependencies(FeedbackHistoryDto dto, FeedbackHistory entity, int saveResult)
    {
        _mapperMock.Setup(m => m.Map<FeedbackHistory>(It.IsAny<CreateFeedbackHistoryDto>())).Returns(entity);
        _mapperMock.Setup(m => m.Map<FeedbackHistoryDto>(It.IsAny<FeedbackHistory>())).Returns(dto);

        _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<CreateFeedbackHistoryCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _repositoryWrapperMock.Setup(r => r.FeedbackHistoriesRepository.CreateAsync(It.IsAny<FeedbackHistory>()))
            .ReturnsAsync(entity);
        _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(saveResult);
    }
}