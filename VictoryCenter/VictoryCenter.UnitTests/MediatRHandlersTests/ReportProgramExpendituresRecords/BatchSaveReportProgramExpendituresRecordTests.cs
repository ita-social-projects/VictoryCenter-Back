using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Moq;
using VictoryCenter.BLL.Commands.Admin.ReportProgramExpendituresRecords.BatchSave;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ReportProgramExpendituresRecords;
using VictoryCenter.BLL.Notifications.ReportFunds;
using VictoryCenter.BLL.Validators.ReportProgramExpendituresRecords;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.HippotherapyProgramCategories;
using VictoryCenter.DAL.Repositories.Interfaces.ReportProgramExpendituresRecords;
using VictoryCenter.DAL.Repositories.Options;
using VictoryCenter.UnitTests.Utils;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.ReportProgramExpendituresRecords;

public class BatchSaveReportProgramExpendituresRecordTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IReportProgramExpendituresRecordsRepository> _recordsRepositoryMock;
    private readonly Mock<IHippotherapyProgramCategoriesRepository> _categoriesRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IDbContextTransaction> _transactionMock;
    private readonly Mock<ILogger<BatchSaveReportProgramExpendituresRecordHandler>> _loggerMock;
    private readonly IValidator<BatchSaveReportProgramExpendituresRecordCommand> _validator;

    private readonly CreateReportProgramExpendituresRecordDto _createDto = new()
    {
        HippotherapyProgramCategoryId = 1,
        AmountUah = 100.50m,
        AmountUsd = 50.5m,
        ReportingYear = TimeProvider.System.GetUtcNow().Year
    };

    private readonly BatchUpdateReportProgramExpendituresRecordDto _updateDto = new()
    {
        Id = 2,
        HippotherapyProgramCategoryId = 2,
        AmountUah = 200.10m,
        AmountUsd = 100.5m
    };

    private readonly long _deleteRecordId = 3;
    private readonly BatchSaveReportProgramExpendituresRecordsDto _batchDto;

    private readonly HippotherapyProgramCategory _categoryForCreate = new() { Id = 1 };
    private readonly HippotherapyProgramCategory _categoryForUpdate = new() { Id = 2 };

    private readonly ReportProgramExpendituresRecord _existingRecordToUpdate = new() { Id = 2, HippotherapyProgramCategoryId = 2 };
    private readonly ReportProgramExpendituresRecord _existingRecordToDelete = new() { Id = 3, HippotherapyProgramCategoryId = 3 };

    public BatchSaveReportProgramExpendituresRecordTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _recordsRepositoryMock = new Mock<IReportProgramExpendituresRecordsRepository>();
        _categoriesRepositoryMock = new Mock<IHippotherapyProgramCategoriesRepository>();
        _mapperMock = new Mock<IMapper>();
        _transactionMock = new Mock<IDbContextTransaction>();
        _loggerMock = new Mock<ILogger<BatchSaveReportProgramExpendituresRecordHandler>>();
        var createDtoValidator = new CreateReportProgramExpendituresRecordDtoValidator(new BaseReportProgramExpendituresRecordValidator());
        var updateDtoValidator = new BatchUpdateReportProgramExpendituresRecordDtoValidator(new BaseReportProgramExpendituresRecordValidator());

        _validator = new BatchSaveReportProgramExpendituresRecordsCommandValidator(
            createDtoValidator,
            updateDtoValidator);

        _batchDto = new BatchSaveReportProgramExpendituresRecordsDto
        {
            RecordsToCreate = [_createDto],
            RecordsToUpdate = [_updateDto],
            RecordIdsToDelete = [_deleteRecordId]
        };
    }

    [Fact]
    public async Task Handle_ShouldExecuteBatchSuccessfully()
    {
        // Arrange
        var existingRecords = new List<ReportProgramExpendituresRecord>
        {
            _existingRecordToUpdate,
            _existingRecordToDelete
        };

        var existingCategories = new List<HippotherapyProgramCategory>
        {
            _categoryForCreate,
            _categoryForUpdate
        };

        var duplicateRecords = new List<ReportProgramExpendituresRecord>();

        SetupDependencies(existingRecords, existingCategories, duplicateRecords, saveResult: 3);

        var handler = new BatchSaveReportProgramExpendituresRecordHandler(
            _mediatorMock.Object,
            _repositoryWrapperMock.Object,
            _validator,
            _mapperMock.Object,
            _loggerMock.Object);

        var command = new BatchSaveReportProgramExpendituresRecordCommand(_batchDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _recordsRepositoryMock.Verify(
            r => r.DeleteRange(
                It.Is<IEnumerable<ReportProgramExpendituresRecord>>(
                    records => records.Any(rec => rec.Id == _deleteRecordId))),
            Times.Once);
        _recordsRepositoryMock.Verify(
            r => r.UpdateRange(It.IsAny<IEnumerable<ReportProgramExpendituresRecord>>()),
            Times.Once);
        _recordsRepositoryMock.Verify(
            r => r.CreateRangeAsync(It.IsAny<IEnumerable<ReportProgramExpendituresRecord>>()),
            Times.Once);
        _repositoryWrapperMock.Verify(w => w.SaveChangesAsync(), Times.Once);
        _transactionMock.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mediatorMock.Verify(
            m => m.Publish(It.IsAny<ReportFundsChangedNotification>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenRecordToUpdateOrDeleteDoesNotExist()
    {
        // Arrange
        var existingRecords = new List<ReportProgramExpendituresRecord>();
        var existingCategories = new List<HippotherapyProgramCategory>();
        var duplicateRecords = new List<ReportProgramExpendituresRecord>();

        SetupDependencies(existingRecords, existingCategories, duplicateRecords);

        var handler = new BatchSaveReportProgramExpendituresRecordHandler(
           _mediatorMock.Object,
           _repositoryWrapperMock.Object,
           _validator,
           _mapperMock.Object,
           _loggerMock.Object);

        var command = new BatchSaveReportProgramExpendituresRecordCommand(_batchDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);

        var expectedMissingIds = new List<long> { _updateDto.Id, _deleteRecordId };
        var expectedErrorMessage = ErrorMessagesConstants.NotFound(
            expectedMissingIds,
            typeof(ReportProgramExpendituresRecord));

        Assert.Contains(result.Errors, e => e.Message == expectedErrorMessage);

        _repositoryWrapperMock.Verify(w => w.SaveChangesAsync(), Times.Never);
        _transactionMock.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        _mediatorMock.Verify(
            m => m.Publish(It.IsAny<ReportFundsChangedNotification>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenCategoryDoesNotExist()
    {
        // Arrange
        var recordWithNewCategory = new BatchUpdateReportProgramExpendituresRecordDto
        {
            Id = 2,
            HippotherapyProgramCategoryId = 99,
            AmountUah = 150m,
            AmountUsd = 30m
        };

        var batchDto = new BatchSaveReportProgramExpendituresRecordsDto
        {
            RecordsToCreate = [],
            RecordsToUpdate = [recordWithNewCategory],
            RecordIdsToDelete = []
        };

        var existingRecords = new List<ReportProgramExpendituresRecord>
        {
            _existingRecordToUpdate
        };

        var existingCategories = new List<HippotherapyProgramCategory>();
        var duplicateRecords = new List<ReportProgramExpendituresRecord>();

        SetupDependencies(existingRecords, existingCategories, duplicateRecords);

        var handler = new BatchSaveReportProgramExpendituresRecordHandler(
           _mediatorMock.Object,
           _repositoryWrapperMock.Object,
           _validator,
           _mapperMock.Object,
           _loggerMock.Object);

        var command = new BatchSaveReportProgramExpendituresRecordCommand(batchDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);

        var expectedMissingIds = new List<long> { recordWithNewCategory.HippotherapyProgramCategoryId };
        var expectedErrorMessage = ErrorMessagesConstants.NotFound(
            expectedMissingIds,
            typeof(HippotherapyProgramCategory));

        Assert.Contains(result.Errors, e => e.Message == expectedErrorMessage);

        _repositoryWrapperMock.Verify(w => w.SaveChangesAsync(), Times.Never);
        _transactionMock.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        _mediatorMock.Verify(
            m => m.Publish(It.IsAny<ReportFundsChangedNotification>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenCategoryAlreadyHasRecord()
    {
        // Arrange
        var existingRecords = new List<ReportProgramExpendituresRecord>
        {
            _existingRecordToUpdate,
            _existingRecordToDelete
        };

        var existingCategories = new List<HippotherapyProgramCategory>
        {
            _categoryForCreate,
            _categoryForUpdate
        };

        var duplicateRecords = new List<ReportProgramExpendituresRecord>
        {
            new() { Id = 99, HippotherapyProgramCategoryId = _createDto.HippotherapyProgramCategoryId }
        };

        SetupDependencies(existingRecords, existingCategories, duplicateRecords);

        var handler = new BatchSaveReportProgramExpendituresRecordHandler(
           _mediatorMock.Object,
           _repositoryWrapperMock.Object,
           _validator,
           _mapperMock.Object,
           _loggerMock.Object);

        var command = new BatchSaveReportProgramExpendituresRecordCommand(_batchDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(
            result.Errors,
            e => e.Message == ReportProgramExpendituresRecordConstants.ProgramCategoryAlreadyHasRecord(duplicateRecords[0].HippotherapyProgramCategoryId));

        _repositoryWrapperMock.Verify(w => w.SaveChangesAsync(), Times.Never);
        _transactionMock.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        _mediatorMock.Verify(
            m => m.Publish(It.IsAny<ReportFundsChangedNotification>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionOccurs()
    {
        // Arrange
        var existingRecords = new List<ReportProgramExpendituresRecord>
        {
            _existingRecordToUpdate,
            _existingRecordToDelete
        };

        var existingCategories = new List<HippotherapyProgramCategory>
        {
            _categoryForCreate,
            _categoryForUpdate
        };

        SetupDependencies(existingRecords, existingCategories, []);

        _repositoryWrapperMock
            .Setup(w => w.SaveChangesAsync())
            .ThrowsAsync(new DbUpdateException());

        var handler = new BatchSaveReportProgramExpendituresRecordHandler(
           _mediatorMock.Object,
           _repositoryWrapperMock.Object,
           _validator,
           _mapperMock.Object,
           _loggerMock.Object);

        var command = new BatchSaveReportProgramExpendituresRecordCommand(_batchDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(
            result.Errors,
            e => e.Message == ErrorMessagesConstants.FailedToSaveEntitiesInDatabase(nameof(ReportProgramExpendituresRecord)));

        _transactionMock.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        _mediatorMock.Verify(
            m => m.Publish(It.IsAny<ReportFundsChangedNotification>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenValidationExceptionOccurs()
    {
        // Arrange
        var invalidBatchDto = new BatchSaveReportProgramExpendituresRecordsDto
        {
            RecordsToCreate =
            [
                new() { HippotherapyProgramCategoryId = 1, AmountUah = 100 }
            ],
            RecordsToUpdate =
            [
                new() { Id = 2, HippotherapyProgramCategoryId = 1, AmountUah = 200 }
            ],
            RecordIdsToDelete = []
        };

        var handler = new BatchSaveReportProgramExpendituresRecordHandler(
           _mediatorMock.Object,
           _repositoryWrapperMock.Object,
           _validator,
           _mapperMock.Object,
           _loggerMock.Object);

        var command = new BatchSaveReportProgramExpendituresRecordCommand(invalidBatchDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(
            result.Errors,
            e => e.Message == ErrorMessagesConstants.CollectionMustContainUniqueValues(
                nameof(ReportProgramExpendituresRecord.HippotherapyProgramCategoryId)));

        _repositoryWrapperMock.Verify(w => w.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
        _repositoryWrapperMock.Verify(
            w => w.ReportProgramExpendituresRecordsRepository.GetAllAsync(
                It.IsAny<QueryOptions<ReportProgramExpendituresRecord>>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldSucceed_WhenDeletingAndCreatingRecordsWithSameCategory()
    {
        // Arrange
        var sameCategoryId = 3;
        var createDto = new CreateReportProgramExpendituresRecordDto
        {
            HippotherapyProgramCategoryId = sameCategoryId,
            AmountUah = 100.50m,
            AmountUsd = 50.5m,
            ReportingYear = TimeProvider.System.GetUtcNow().Year
        };

        var deleteRecord = new ReportProgramExpendituresRecord
        {
            Id = _deleteRecordId,
            HippotherapyProgramCategoryId = sameCategoryId
        };

        var batchDto = new BatchSaveReportProgramExpendituresRecordsDto
        {
            RecordsToCreate = [createDto],
            RecordsToUpdate = [],
            RecordIdsToDelete = [_deleteRecordId]
        };

        var existingRecords = new List<ReportProgramExpendituresRecord>
        {
            deleteRecord
        };

        var existingCategories = new List<HippotherapyProgramCategory>
        {
            new() { Id = sameCategoryId }
        };

        SetupDependencies(existingRecords, existingCategories, [], saveResult: 2);

        var handler = new BatchSaveReportProgramExpendituresRecordHandler(
            _mediatorMock.Object,
            _repositoryWrapperMock.Object,
            _validator,
            _mapperMock.Object,
            _loggerMock.Object);

        var command = new BatchSaveReportProgramExpendituresRecordCommand(batchDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _recordsRepositoryMock.Verify(
            r => r.DeleteRange(
                It.Is<IEnumerable<ReportProgramExpendituresRecord>>(
                    records => records.Any(rec => rec.Id == _deleteRecordId))),
            Times.Once);
        _recordsRepositoryMock.Verify(
            r => r.CreateRangeAsync(It.Is<IEnumerable<ReportProgramExpendituresRecord>>(
                records => records.Any(rec => rec.HippotherapyProgramCategoryId == sameCategoryId))),
            Times.Once);
        _recordsRepositoryMock.Verify(
            r => r.UpdateRange(It.IsAny<IEnumerable<ReportProgramExpendituresRecord>>()),
            Times.Never);
        _repositoryWrapperMock.Verify(w => w.SaveChangesAsync(), Times.Once);
        _transactionMock.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mediatorMock.Verify(
            m => m.Publish(It.IsAny<ReportFundsChangedNotification>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenUniqueConstraintIsViolatedOnSave()
    {
        // Arrange
        var existingRecords = new List<ReportProgramExpendituresRecord>
        {
            _existingRecordToUpdate,
            _existingRecordToDelete
        };

        var existingCategories = new List<HippotherapyProgramCategory>
        {
            _categoryForCreate,
            _categoryForUpdate
        };

        SetupDependencies(existingRecords, existingCategories, []);

        _repositoryWrapperMock.Setup(w => w.SaveChangesAsync())
            .ThrowsAsync(SqlExceptionFactory.CreateDbUpdateException(2601, "Unique index violation"));

        var handler = new BatchSaveReportProgramExpendituresRecordHandler(
            _mediatorMock.Object,
            _repositoryWrapperMock.Object,
            _validator,
            _mapperMock.Object,
            _loggerMock.Object);

        var command = new BatchSaveReportProgramExpendituresRecordCommand(_batchDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(
            result.Errors,
            e => e.Message == ReportProgramExpendituresRecordConstants.ProgramCategoryAlreadyHasRecord());

        _transactionMock.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        _mediatorMock.Verify(
            m => m.Publish(It.IsAny<ReportFundsChangedNotification>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private void SetupDependencies(
        List<ReportProgramExpendituresRecord> existingRecords,
        List<HippotherapyProgramCategory> existingCategories,
        List<ReportProgramExpendituresRecord> duplicateRecords,
        int saveResult = 1)
    {
        _repositoryWrapperMock.SetupGet(wrapper => wrapper.ReportProgramExpendituresRecordsRepository)
            .Returns(_recordsRepositoryMock.Object);
        _repositoryWrapperMock.SetupGet(wrapper => wrapper.HippotherapyProgramCategoriesRepository)
            .Returns(_categoriesRepositoryMock.Object);

        _repositoryWrapperMock.Setup(w => w.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(_transactionMock.Object);
        _repositoryWrapperMock.Setup(w => w.SaveChangesAsync())
            .ReturnsAsync(saveResult);

        _mediatorMock.Setup(m => m.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _categoriesRepositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<HippotherapyProgramCategory>>()))
            .ReturnsAsync(existingCategories);

        _recordsRepositoryMock
            .SetupSequence(r => r.GetAllAsync(It.IsAny<QueryOptions<ReportProgramExpendituresRecord>>()))
            .ReturnsAsync(existingRecords)
            .ReturnsAsync(duplicateRecords);

        _mapperMock
            .Setup(m => m.Map<ReportProgramExpendituresRecord>(It.IsAny<CreateReportProgramExpendituresRecordDto>()))
            .Returns((CreateReportProgramExpendituresRecordDto dto) => new ReportProgramExpendituresRecord
            {
                Id = 4,
                HippotherapyProgramCategoryId = dto.HippotherapyProgramCategoryId,
                ReportingYear = dto.ReportingYear,
                AmountUah = dto.AmountUah ?? 0.0m,
                AmountUsd = dto.AmountUsd ?? 0.0m
            });
    }
}
