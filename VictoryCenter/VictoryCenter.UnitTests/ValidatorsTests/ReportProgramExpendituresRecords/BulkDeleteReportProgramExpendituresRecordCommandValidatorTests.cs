using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Admin.ReportProgramExpendituresRecords.BulkDelete;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Validators.ReportProgramExpendituresRecords;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.UnitTests.ValidatorsTests.ReportProgramExpendituresRecords;

public class BulkDeleteReportProgramExpendituresRecordCommandValidatorTests
{
    private readonly BulkDeleteReportProgramExpendituresRecordCommandValidator _validator;

    public BulkDeleteReportProgramExpendituresRecordCommandValidatorTests()
    {
        _validator = new BulkDeleteReportProgramExpendituresRecordCommandValidator();
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenIdsAreEmpty()
    {
        // Arrange
        var command = new BulkDeleteReportProgramExpendituresRecordCommand(Array.Empty<long>());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Ids)
            .WithErrorMessage(ErrorMessagesConstants.CollectionCannotBeEmpty(
                nameof(BulkDeleteReportProgramExpendituresRecordCommand.Ids)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenIdsContainInvalidId()
    {
        // Arrange
        var command = new BulkDeleteReportProgramExpendituresRecordCommand(new[] { 1L, 0L });

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Ids)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBePositive(
                nameof(ReportProgramExpendituresRecord.Id)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenIdsAreNotUnique()
    {
        // Arrange
        var command = new BulkDeleteReportProgramExpendituresRecordCommand(new[] { 1L, 1L });

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Ids)
            .WithErrorMessage(ErrorMessagesConstants.CollectionMustContainUniqueValues(
                nameof(BulkDeleteReportProgramExpendituresRecordCommand.Ids)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenIdsExceedMaximumCount()
    {
        // Arrange
        var maxCount = ReportProgramExpendituresRecordConstants.MaxNumberOfRecordsPerBulkDelete;
        var ids = Enumerable.Range(1, maxCount + 1).Select(i => (long)i).ToArray();
        var command = new BulkDeleteReportProgramExpendituresRecordCommand(ids);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Ids)
            .WithErrorMessage(ErrorMessagesConstants.CollectionCannotContainMoreThan(
                nameof(BulkDeleteReportProgramExpendituresRecordCommand.Ids),
                maxCount));
    }

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenDataIsValid()
    {
        // Arrange
        var command = new BulkDeleteReportProgramExpendituresRecordCommand(new[] { 1L, 2L, 3L });

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
