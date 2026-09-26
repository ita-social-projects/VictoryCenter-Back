using AutoMapper;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using VictoryCenter.BLL.Commands.Admin.ReportFundsExpendituresRecords.BatchSave;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresRecords;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresSettings;
using VictoryCenter.BLL.Interfaces.ReportFundsExpendituresRecordHelper;
using VictoryCenter.BLL.Notifications.ReportFunds;
using VictoryCenter.BLL.Validators.ReportFundsExpendituresRecords;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Interfaces.ReportFundsExpendituresCategories;
using VictoryCenter.DAL.Repositories.Interfaces.ReportFundsExpendituresRecords;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.UnitTests.MediatRHandlersTests.ReportFundsExpendituresRecords;

public class BatchSaveReportFundsExpendituresRecordTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IReportFundsExpendituresRecordsRepository> _recordsRepositoryMock;
    private readonly Mock<IReportFundsExpendituresCategoriesRepository> _categoriesRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IDbContextTransaction> _transactionMock;
    private readonly Mock<IReportFundsExpendituresRecordHelper> _helperMock;
    private readonly IValidator<BatchSaveReportFundsExpendituresRecordCommand> _validator;

    private readonly CreateReportFundsExpendituresRecordDto _createDto = new()
    {
        CategoryId = 1,
        Type = ReportFundsExpendituresType.Income,
        ReportingYear = TimeProvider.System.GetUtcNow().Year,
        Amount = 100.50m,
        Currency = ReportFundsExpendituresCurrency.Uah
    };

    private readonly BatchUpdateReportFundsExpendituresRecordDto _updateDto = new()
    {
        Id = 2,
        CategoryId = 2,
        Amount = 200.10m,
        Currency = ReportFundsExpendituresCurrency.Uah
    };

    private readonly long _deleteRecordId = 3;

    private readonly BatchSaveReportFundsExpendituresRecordsDto _batchDto;

    private readonly ReportFundsExpendituresCategory _categoryForCreate = new() { Id = 1, Type = ReportFundsExpendituresType.Income };
    private readonly ReportFundsExpendituresCategory _categoryForUpdate = new() { Id = 2, Type = ReportFundsExpendituresType.Income };

    private readonly ReportFundsExpendituresRecord _existingRecordToUpdate = new() { Id = 2, CategoryId = 2, Type = ReportFundsExpendituresType.Income };
    private readonly ReportFundsExpendituresRecord _existingRecordToDelete = new() { Id = 3, CategoryId = 3, Type = ReportFundsExpendituresType.Expense };

    public BatchSaveReportFundsExpendituresRecordTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _recordsRepositoryMock = new Mock<IReportFundsExpendituresRecordsRepository>();
        _categoriesRepositoryMock = new Mock<IReportFundsExpendituresCategoriesRepository>();
        _mapperMock = new Mock<IMapper>();
        _transactionMock = new Mock<IDbContextTransaction>();
        _helperMock = new Mock<IReportFundsExpendituresRecordHelper>();

        var createDtoValidator = new CreateReportFundsExpendituresRecordDtoValidator(
            new BaseReportFundsExpendituresRecordValidator(),
            TimeProvider.System);

        var updateDtoValidator = new BatchUpdateReportFundsExpendituresRecordDtoValidator(new BaseReportFundsExpendituresRecordValidator());

        _validator = new BatchSaveReportFundsExpendituresRecordsCommandValidator(
            createDtoValidator,
            updateDtoValidator);

        _batchDto = new BatchSaveReportFundsExpendituresRecordsDto
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
        var existingRecords = new List<ReportFundsExpendituresRecord>
        {
            _existingRecordToUpdate,
            _existingRecordToDelete
        };

        var existingCategories = new List<ReportFundsExpendituresCategory>
        {
            _categoryForCreate,
            _categoryForUpdate
        };

        var duplicateRecords = new List<ReportFundsExpendituresRecord>();

        SetupDependencies(existingRecords, existingCategories, duplicateRecords, saveResult: 3);

        var handler = new BatchSaveReportFundsExpendituresRecordHandler(
            _mediatorMock.Object,
            _repositoryWrapperMock.Object,
            _validator,
            _mapperMock.Object,
            _helperMock.Object);

        var command = new BatchSaveReportFundsExpendituresRecordCommand(_batchDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _recordsRepositoryMock.Verify(
            r => r.DeleteRange(
                It.Is<IEnumerable<ReportFundsExpendituresRecord>>(
                    records => records.Any(rec => rec.Id == _deleteRecordId))),
            Times.Once);
        _recordsRepositoryMock.Verify(r => r.CreateRangeAsync(It.IsAny<IEnumerable<ReportFundsExpendituresRecord>>()), Times.Once);
        _repositoryWrapperMock.Verify(w => w.SaveChangesAsync(), Times.Once);
        _transactionMock.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mediatorMock.Verify(m => m.Publish(It.IsAny<ReportFundsChangedNotification>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenRecordToUpdateOrDeleteDoesNotExist()
    {
        // Arrange
        var existingRecords = new List<ReportFundsExpendituresRecord>();

        var existingCategories = new List<ReportFundsExpendituresCategory>();
        var duplicateRecords = new List<ReportFundsExpendituresRecord>();

        SetupDependencies(existingRecords, existingCategories, duplicateRecords);

        var handler = new BatchSaveReportFundsExpendituresRecordHandler(
            _mediatorMock.Object,
            _repositoryWrapperMock.Object,
            _validator,
            _mapperMock.Object,
            _helperMock.Object);

        var command = new BatchSaveReportFundsExpendituresRecordCommand(_batchDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);

        var expectedMissingIds = new List<long> { _updateDto.Id, _deleteRecordId };
        var expectedErrorMessage = ErrorMessagesConstants.NotFound(
            expectedMissingIds,
            typeof(ReportFundsExpendituresRecord));

        Assert.Contains(result.Errors, e => e.Message == expectedErrorMessage);

        _repositoryWrapperMock.Verify(w => w.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
        _repositoryWrapperMock.Verify(w => w.SaveChangesAsync(), Times.Never);
        _mediatorMock.Verify(m => m.Publish(It.IsAny<ReportFundsChangedNotification>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenCategoryDoesNotExist()
    {
        // Arrange
        var existingRecords = new List<ReportFundsExpendituresRecord>
        {
            _existingRecordToUpdate,
            _existingRecordToDelete
        };

        var existingCategories = new List<ReportFundsExpendituresCategory>();

        var duplicateRecords = new List<ReportFundsExpendituresRecord>();

        SetupDependencies(existingRecords, existingCategories, duplicateRecords);

        var handler = new BatchSaveReportFundsExpendituresRecordHandler(
            _mediatorMock.Object,
            _repositoryWrapperMock.Object,
            _validator,
            _mapperMock.Object,
            _helperMock.Object);

        var command = new BatchSaveReportFundsExpendituresRecordCommand(_batchDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);

        var expectedMissingIds = new List<long> { _createDto.CategoryId };
        var expectedErrorMessage = ErrorMessagesConstants.NotFound(
            expectedMissingIds,
            typeof(ReportFundsExpendituresCategory));

        Assert.Contains(result.Errors, e => e.Message == expectedErrorMessage);

        _repositoryWrapperMock.Verify(w => w.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        _repositoryWrapperMock.Verify(w => w.SaveChangesAsync(), Times.Never);
        _mediatorMock.Verify(m => m.Publish(It.IsAny<ReportFundsChangedNotification>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenCategoryTypeDoesNotMatchRecordType()
    {
        // Arrange
        var existingRecords = new List<ReportFundsExpendituresRecord>
        {
            _existingRecordToUpdate,
            _existingRecordToDelete
        };

        var mismatchedCategory = new ReportFundsExpendituresCategory
        {
            Id = _createDto.CategoryId,
            Type = ReportFundsExpendituresType.Expense
        };

        var existingCategories = new List<ReportFundsExpendituresCategory>
        {
            mismatchedCategory,
            _categoryForUpdate
        };

        var duplicateRecords = new List<ReportFundsExpendituresRecord>();

        SetupDependencies(existingRecords, existingCategories, duplicateRecords);

        var handler = new BatchSaveReportFundsExpendituresRecordHandler(
            _mediatorMock.Object,
            _repositoryWrapperMock.Object,
            _validator,
            _mapperMock.Object,
            _helperMock.Object);

        var command = new BatchSaveReportFundsExpendituresRecordCommand(_batchDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, e => e.Message == ReportFundsExpendituresRecordConstants.CategoryTypeMustMatchRecordType);

        _repositoryWrapperMock.Verify(w => w.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        _repositoryWrapperMock.Verify(w => w.SaveChangesAsync(), Times.Never);
        _mediatorMock.Verify(m => m.Publish(It.IsAny<ReportFundsChangedNotification>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenCategoryAlreadyHasRecord()
    {
        // Arrange
        var existingRecords = new List<ReportFundsExpendituresRecord>
        {
            _existingRecordToUpdate,
            _existingRecordToDelete
        };

        var existingCategories = new List<ReportFundsExpendituresCategory>
        {
            _categoryForCreate,
            _categoryForUpdate
        };

        var duplicateRecords = new List<ReportFundsExpendituresRecord>
        {
            new() { Id = 99, CategoryId = _createDto.CategoryId }
        };

        SetupDependencies(existingRecords, existingCategories, duplicateRecords);

        var handler = new BatchSaveReportFundsExpendituresRecordHandler(
            _mediatorMock.Object,
            _repositoryWrapperMock.Object,
            _validator,
            _mapperMock.Object,
            _helperMock.Object);

        var command = new BatchSaveReportFundsExpendituresRecordCommand(_batchDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, e => e.Message == ReportFundsExpendituresRecordConstants.CategoryAlreadyHasRecord);

        _repositoryWrapperMock.Verify(w => w.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        _repositoryWrapperMock.Verify(w => w.SaveChangesAsync(), Times.Never);
        _mediatorMock.Verify(m => m.Publish(It.IsAny<ReportFundsChangedNotification>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenDbUpdateExceptionOccurs()
    {
        // Arrange
        var existingRecords = new List<ReportFundsExpendituresRecord>
        {
            _existingRecordToUpdate,
            _existingRecordToDelete
        };

        var existingCategories = new List<ReportFundsExpendituresCategory>
        {
            _categoryForCreate,
            _categoryForUpdate
        };

        SetupDependencies(existingRecords, existingCategories, []);

        _repositoryWrapperMock
            .Setup(w => w.SaveChangesAsync())
            .ThrowsAsync(new DbUpdateException());

        var handler = new BatchSaveReportFundsExpendituresRecordHandler(
            _mediatorMock.Object,
            _repositoryWrapperMock.Object,
            _validator,
            _mapperMock.Object,
            _helperMock.Object);

        var command = new BatchSaveReportFundsExpendituresRecordCommand(_batchDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, e =>
            e.Message == ErrorMessagesConstants.FailedToSaveEntitiesInDatabase(nameof(ReportFundsExpendituresRecord)));

        _mediatorMock.Verify(m => m.Publish(It.IsAny<ReportFundsChangedNotification>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenValidationExceptionOccurs()
    {
        // Arrange
        var invalidBatchDto = new BatchSaveReportFundsExpendituresRecordsDto
        {
            RecordsToCreate =
            [
                new() { CategoryId = 1, Type = ReportFundsExpendituresType.Income, Amount = 100 }
            ],
            RecordsToUpdate =
            [
                new() { Id = 2, CategoryId = 1, Amount = 200 }
            ],
            RecordIdsToDelete = []
        };

        var handler = new BatchSaveReportFundsExpendituresRecordHandler(
            _mediatorMock.Object,
            _repositoryWrapperMock.Object,
            _validator,
            _mapperMock.Object,
            _helperMock.Object);

        var command = new BatchSaveReportFundsExpendituresRecordCommand(invalidBatchDto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, e =>
            e.Message == ErrorMessagesConstants.CollectionMustContainUniqueValues(nameof(ReportFundsExpendituresRecord.CategoryId)));

        _repositoryWrapperMock.Verify(w => w.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
        _repositoryWrapperMock.Verify(
            w => w.ReportFundsExpendituresRecordsRepository.GetAllAsync(
            It.IsAny<QueryOptions<ReportFundsExpendituresRecord>>()), Times.Never);
    }

    private void SetupDependencies(
        List<ReportFundsExpendituresRecord> existingRecords,
        List<ReportFundsExpendituresCategory> existingCategories,
        List<ReportFundsExpendituresRecord> duplicateRecords,
        int saveResult = 1)
    {
        _repositoryWrapperMock.SetupGet(wrapper => wrapper.ReportFundsExpendituresRecordsRepository)
            .Returns(_recordsRepositoryMock.Object);
        _repositoryWrapperMock.SetupGet(wrapper => wrapper.ReportFundsExpendituresCategoriesRepository)
            .Returns(_categoriesRepositoryMock.Object);

        _repositoryWrapperMock.Setup(w => w.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(_transactionMock.Object);

        _repositoryWrapperMock.Setup(w => w.SaveChangesAsync())
            .ReturnsAsync(saveResult);

        _mediatorMock.Setup(m => m.Publish(It.IsAny<ReportFundsChangedNotification>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _categoriesRepositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<ReportFundsExpendituresCategory>>()))
            .ReturnsAsync(existingCategories);

        _recordsRepositoryMock
            .SetupSequence(r => r.GetAllAsync(It.IsAny<QueryOptions<ReportFundsExpendituresRecord>>()))
            .ReturnsAsync(existingRecords)
            .ReturnsAsync(duplicateRecords);

        _mapperMock
            .Setup(m => m.Map<ReportFundsExpendituresRecord>(It.IsAny<CreateReportFundsExpendituresRecordDto>()))
            .Returns((CreateReportFundsExpendituresRecordDto dto) => new ReportFundsExpendituresRecord
            {
                Id = 4,
                CategoryId = dto.CategoryId,
                Type = dto.Type,
                ReportingYear = dto.ReportingYear
            });

        _helperMock
            .Setup(h => h.GetAndValidateSettingsAsync())
            .ReturnsAsync(Result.Ok(new ReportFundsExpendituresSettingsDto
            {
                ExchangeRate = 40m
            }));

        _helperMock
            .Setup(h => h.CalculateAmounts(
                It.IsAny<decimal>(),
                It.IsAny<ReportFundsExpendituresCurrency>(),
                It.IsAny<decimal>()))
            .Returns((decimal amount, ReportFundsExpendituresCurrency currency, decimal exchangeRate) =>
                currency == ReportFundsExpendituresCurrency.Usd
                    ? (AmountUah: amount * exchangeRate, AmountUsd: amount)
                    : (AmountUah: amount, AmountUsd: amount / exchangeRate));
    }
}
