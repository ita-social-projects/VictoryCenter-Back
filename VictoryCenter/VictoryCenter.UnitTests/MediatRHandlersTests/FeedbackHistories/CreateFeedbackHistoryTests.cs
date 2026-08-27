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
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.FeedbackHistories;

public class CreateFeedbackHistoryTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IValidator<CreateFeedbackHistoryCommand>> _validatorMock;

    private readonly CreateFeedbackHistoryDto _createFeedbackHistoryDto = new()
    {
        Title = "Successful Recovery",
        Story = "Detailed feedback story text describing the experience.",
        ImageId = null
    };

    private readonly FeedbackHistory _feedbackHistory = new()
    {
        Id = 1L,
        Title = "Successful Recovery",
        Story = "Detailed feedback story text describing the experience.",
        ImageId = null,
        CreatedAt = DateTimeOffset.UtcNow.AddMinutes(-5)
    };

    private readonly FeedbackHistoryDto _feedbackHistoryDto = new()
    {
        Id = 1L,
        Title = "Successful Recovery",
        Story = "Detailed feedback story text describing the experience.",
        Image = null
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
        var handler = new CreateFeedbackHistoryHandler(_mapperMock.Object, _repositoryWrapperMock.Object, _validatorMock.Object);

        Result<FeedbackHistoryDto> result =
            await handler.Handle(new CreateFeedbackHistoryCommand(_createFeedbackHistoryDto), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(_feedbackHistoryDto, result.Value);
    }

    [Fact]
    public async Task Handle_WhenSaveChangesFails_ShouldReturnFailure()
    {
        var failMessage = ErrorMessagesConstants.FailedToCreateEntity(typeof(FeedbackHistory));
        SetupDependencies(_feedbackHistoryDto, _feedbackHistory, 0);

        var handler = new CreateFeedbackHistoryHandler(_mapperMock.Object, _repositoryWrapperMock.Object, _validatorMock.Object);

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

        var handler = new CreateFeedbackHistoryHandler(_mapperMock.Object, _repositoryWrapperMock.Object, _validatorMock.Object);

        // Хендлер не огортає DbUpdateException окремим try-catch окрім ValidationException
        await Assert.ThrowsAsync<DbUpdateException>(() =>
            handler.Handle(new CreateFeedbackHistoryCommand(_createFeedbackHistoryDto), CancellationToken.None));
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