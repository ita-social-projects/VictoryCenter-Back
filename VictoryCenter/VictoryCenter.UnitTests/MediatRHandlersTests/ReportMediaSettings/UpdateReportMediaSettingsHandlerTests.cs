using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Moq;
using VictoryCenter.BLL.Commands.Admin.ReportMediaSettings.UpdateReportMediaSettings;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ReportMediaSettings;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.ReportMediaSettings;
public class UpdateReportMediaSettingsHandlerTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IValidator<UpdateReportMediaSettingsCommand>> _mockValidator;

    private readonly UpdateReportMediaSettingsDto _updateDto = new()
    {
        CollectedFundsBlock = new()
        {
            Title = "Collected",
            CollectedFunds = 100,
            ImageId = 1
        },
        ChangedLivesBlock = new()
        {
            Title = "Lives",
            ChangedLives = 50,
            ImageId = 2
        }
    };

    private readonly Image _collectedImage = new() { Id = 1 };
    private readonly Image _changedImage = new() { Id = 2 };

    private readonly CollectedFundsBlock _existingCollected = new()
    {
        Id = 1,
        Title = "Old",
        CollectedAmount = 10,
        ImageId = 5
    };

    private readonly ChangedLivesBlock _existingChanged = new()
    {
        Id = 2,
        Title = "Old",
        ChangedLivesCount = 5,
        ImageId = 6
    };

    private readonly ReportMediaSettingsDto _resultDto = new()
    {
        CollectedFundsBlock = new(),
        ChangedLivesBlock = new()
    };

    public UpdateReportMediaSettingsHandlerTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockValidator = new Mock<IValidator<UpdateReportMediaSettingsCommand>>();
    }

    [Fact]
    public async Task Handle_BlocksDoNotExist_ShouldCreateAndReturnOk()
    {
        // Arrange
        var createdCollected = new CollectedFundsBlock { Id = 1 };
        var createdChanged = new ChangedLivesBlock { Id = 2 };

        SetupRepositoryWrapper(
            collectedImage: _collectedImage,
            changedImage: _changedImage,
            collectedBlock: null,
            changedBlock: null,
            finalCollected: createdCollected,
            finalChanged: createdChanged);

        SetupMapper(_resultDto);

        var command = new UpdateReportMediaSettingsCommand(_updateDto);
        var handler = new UpdateReportMediaSettingsHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockValidator.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(_resultDto, result.Value);

        _mockRepositoryWrapper.Verify(
            r => r.GetRepository<CollectedFundsBlock>().CreateAsync(It.IsAny<CollectedFundsBlock>()),
            Times.Once);

        _mockRepositoryWrapper.Verify(
            r => r.GetRepository<ChangedLivesBlock>().CreateAsync(It.IsAny<ChangedLivesBlock>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_BlocksExist_ShouldUpdateAndReturnOk()
    {
        // Arrange
        SetupRepositoryWrapper(
            collectedImage: _collectedImage,
            changedImage: _changedImage,
            collectedBlock: _existingCollected,
            changedBlock: _existingChanged,
            finalCollected: _existingCollected,
            finalChanged: _existingChanged);

        SetupMapper(_resultDto);

        var command = new UpdateReportMediaSettingsCommand(_updateDto);
        var handler = new UpdateReportMediaSettingsHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockValidator.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(_resultDto, result.Value);

        _mockRepositoryWrapper.Verify(
            r => r.GetRepository<CollectedFundsBlock>().Update(_existingCollected),
            Times.Once);

        _mockRepositoryWrapper.Verify(
            r => r.GetRepository<ChangedLivesBlock>().Update(_existingChanged),
            Times.Once);
    }

    [Fact]
    public async Task Handle_CollectedImageNotFound_ShouldReturnFailure()
    {
        // Arrange
        SetupRepositoryWrapper(
            collectedImage: null,
            changedImage: null,
            collectedBlock: null,
            changedBlock: null,
            finalCollected: null,
            finalChanged: null);

        var command = new UpdateReportMediaSettingsCommand(_updateDto);
        var handler = new UpdateReportMediaSettingsHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockValidator.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_ChangedImageNotFound_ShouldReturnFailure()
    {
        // Arrange
        SetupRepositoryWrapper(
            collectedImage: _collectedImage,
            changedImage: null,
            collectedBlock: null,
            changedBlock: null,
            finalCollected: null,
            finalChanged: null);

        var command = new UpdateReportMediaSettingsCommand(_updateDto);
        var handler = new UpdateReportMediaSettingsHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockValidator.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Handle_DbUpdateException_ShouldReturnFailure()
    {
        // Arrange
        SetupRepositoryWrapper(
            collectedImage: _collectedImage,
            changedImage: _changedImage,
            collectedBlock: _existingCollected,
            changedBlock: _existingChanged,
            finalCollected: _existingCollected,
            finalChanged: _existingChanged);

        _mockRepositoryWrapper
            .Setup(r => r.SaveChangesAsync())
            .ThrowsAsync(new DbUpdateException());

        var command = new UpdateReportMediaSettingsCommand(_updateDto);
        var handler = new UpdateReportMediaSettingsHandler(
            _mockRepositoryWrapper.Object,
            _mockMapper.Object,
            _mockValidator.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal(
            ErrorMessagesConstants.FailedToUpdateEntityInDatabase(typeof(CollectedFundsBlock)),
            result.Errors[0].Message);
    }

    private void SetupMapper(ReportMediaSettingsDto dtoToReturn)
    {
        _mockMapper
            .Setup(m => m.Map<CollectedFundsBlockDto>(It.IsAny<CollectedFundsBlock>()))
            .Returns(dtoToReturn.CollectedFundsBlock);

        _mockMapper
            .Setup(m => m.Map<ChangedLivesBlockDto>(It.IsAny<ChangedLivesBlock>()))
            .Returns(dtoToReturn.ChangedLivesBlock);
    }

    private void SetupRepositoryWrapper(
        Image? collectedImage,
        Image? changedImage,
        CollectedFundsBlock? collectedBlock,
        ChangedLivesBlock? changedBlock,
        CollectedFundsBlock? finalCollected,
        ChangedLivesBlock? finalChanged)
    {
        var collectedRepo = new Mock<IRepositoryBase<CollectedFundsBlock>>();
        var changedRepo = new Mock<IRepositoryBase<ChangedLivesBlock>>();

        collectedRepo
            .SetupSequence(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<CollectedFundsBlock>>()))
            .ReturnsAsync(collectedBlock)
            .ReturnsAsync(finalCollected);

        changedRepo
            .SetupSequence(r => r.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<ChangedLivesBlock>>()))
            .ReturnsAsync(changedBlock)
            .ReturnsAsync(finalChanged);

        _mockRepositoryWrapper
            .Setup(r => r.GetRepository<CollectedFundsBlock>())
            .Returns(collectedRepo.Object);

        _mockRepositoryWrapper
            .Setup(r => r.GetRepository<ChangedLivesBlock>())
            .Returns(changedRepo.Object);

        _mockRepositoryWrapper
            .SetupSequence(r => r.ImageRepository.GetFirstOrDefaultAsync(It.IsAny<QueryOptions<Image>>()))
            .ReturnsAsync(collectedImage)
            .ReturnsAsync(changedImage);

        _mockRepositoryWrapper
            .Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);
    }
}
