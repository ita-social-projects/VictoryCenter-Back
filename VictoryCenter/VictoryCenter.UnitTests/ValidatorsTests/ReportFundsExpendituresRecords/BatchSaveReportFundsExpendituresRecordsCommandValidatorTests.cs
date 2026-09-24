using FluentValidation;
using FluentValidation.TestHelper;
using Moq;
using VictoryCenter.BLL.Commands.Admin.ReportFundsExpendituresRecords.BatchSave;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresRecords;
using VictoryCenter.BLL.Validators.ReportFundsExpendituresRecords;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.UnitTests.ValidatorsTests.ReportFundsExpendituresRecords;

public class BatchSaveReportFundsExpendituresRecordsCommandValidatorTests
{
    private readonly BatchSaveReportFundsExpendituresRecordsCommandValidator _validator;
    private readonly Mock<IValidator<CreateReportFundsExpendituresRecordDto>> _createDtoValidatorMock;
    private readonly Mock<IValidator<BatchUpdateReportFundsExpendituresRecordDto>> _updateDtoValidatorMock;

    public BatchSaveReportFundsExpendituresRecordsCommandValidatorTests()
    {
        _createDtoValidatorMock = new Mock<IValidator<CreateReportFundsExpendituresRecordDto>>();
        _updateDtoValidatorMock = new Mock<IValidator<BatchUpdateReportFundsExpendituresRecordDto>>();
        _validator = new BatchSaveReportFundsExpendituresRecordsCommandValidator(
            _createDtoValidatorMock.Object,
            _updateDtoValidatorMock.Object);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenBatchSaveDtoIsNull()
    {
        // Arrange
        var command = new BatchSaveReportFundsExpendituresRecordCommand(null!);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BatchSaveReportFundsExpendituresRecordsDto);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenRecordsToUpdateHaveDuplicateIds()
    {
        // Arrange
        var dto = new BatchSaveReportFundsExpendituresRecordsDto
        {
            RecordsToUpdate =
            [
                new() { Id = 10 },
                new() { Id = 10 }
            ]
        };
        var command = new BatchSaveReportFundsExpendituresRecordCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BatchSaveReportFundsExpendituresRecordsDto.RecordsToUpdate.Select(u => u.Id))
            .WithErrorMessage(ErrorMessagesConstants.CollectionMustContainUniqueValues(
                nameof(BatchSaveReportFundsExpendituresRecordsDto.RecordsToUpdate)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenRecordIdsToDeleteHaveDuplicates()
    {
        // Arrange
        var dto = new BatchSaveReportFundsExpendituresRecordsDto
        {
            RecordIdsToDelete = [1, 2, 2, 3]
        };
        var command = new BatchSaveReportFundsExpendituresRecordCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BatchSaveReportFundsExpendituresRecordsDto.RecordIdsToDelete)
            .WithErrorMessage(ErrorMessagesConstants.CollectionMustContainUniqueValues(
                nameof(BatchSaveReportFundsExpendituresRecordsDto.RecordIdsToDelete)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenRecordIdsToDeleteExceedLimit()
    {
        // Arrange
        var limit = ReportFundsExpendituresRecordConstants.MaxNumberOfRecordsPerBulkDelete;
        var excessiveIds = Enumerable.Range(1, limit + 1).Select(i => (long)i).ToList();

        var dto = new BatchSaveReportFundsExpendituresRecordsDto
        {
            RecordIdsToDelete = excessiveIds
        };
        var command = new BatchSaveReportFundsExpendituresRecordCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BatchSaveReportFundsExpendituresRecordsDto.RecordIdsToDelete)
            .WithErrorMessage(ErrorMessagesConstants.CollectionCannotContainMoreThan(
                nameof(BatchSaveReportFundsExpendituresRecordsDto.RecordIdsToDelete),
                limit));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_ShouldHaveError_WhenRecordIdsToDeleteContainNonPositiveId(long invalidId)
    {
        // Arrange
        var dto = new BatchSaveReportFundsExpendituresRecordsDto
        {
            RecordIdsToDelete = [1, invalidId, 3]
        };
        var command = new BatchSaveReportFundsExpendituresRecordCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BatchSaveReportFundsExpendituresRecordsDto.RecordIdsToDelete)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBePositive(
                nameof(ReportFundsExpendituresRecord.Id)));
    }

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenAllDataIsValid()
    {
        // Arrange
        var dto = new BatchSaveReportFundsExpendituresRecordsDto
        {
            RecordsToCreate = [new() { CategoryId = 1, Amount = 101.25m }],
            RecordsToUpdate = [new() { Id = 1, CategoryId = 2 }, new() { Id = 2, CategoryId = 3 }],
            RecordIdsToDelete = [10, 11]
        };
        var command = new BatchSaveReportFundsExpendituresRecordCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenAllCollectionsAreEmpty()
    {
        // Arrange
        var dto = new BatchSaveReportFundsExpendituresRecordsDto
        {
            RecordsToCreate = [],
            RecordsToUpdate = [],
            RecordIdsToDelete = []
        };
        var command = new BatchSaveReportFundsExpendituresRecordCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BatchSaveReportFundsExpendituresRecordsDto)
            .WithErrorMessage(ErrorMessagesConstants.BatchOperationMustContainAtLeastOneRecord());
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenRecordsToUpdateAndRecordIdsToDeleteContainIntersectingIds()
    {
        // Arrange
        const long intersectingId = 2;

        var dto = new BatchSaveReportFundsExpendituresRecordsDto
        {
            RecordsToCreate = [],
            RecordsToUpdate =
            [
                new() { Id = 1 },
                new() { Id = intersectingId },
                new() { Id = 3 }
            ],
            RecordIdsToDelete = [intersectingId, 4, 5]
        };

        var command = new BatchSaveReportFundsExpendituresRecordCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BatchSaveReportFundsExpendituresRecordsDto)
            .WithErrorMessage(ErrorMessagesConstants.CollectionsCannotContainIntersectingIds(
                nameof(BatchSaveReportFundsExpendituresRecordsDto.RecordsToUpdate),
                nameof(BatchSaveReportFundsExpendituresRecordsDto.RecordIdsToDelete)));
    }
}
