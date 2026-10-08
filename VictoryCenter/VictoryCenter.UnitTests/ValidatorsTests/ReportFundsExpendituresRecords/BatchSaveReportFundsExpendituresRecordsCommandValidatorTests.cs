using FluentValidation;
using FluentValidation.TestHelper;
using Moq;
using VictoryCenter.BLL.Commands.Admin.ReportFundsExpendituresRecords.BatchSave;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresRecords;
using VictoryCenter.BLL.Validators.ReportFundsExpendituresRecords;

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
}
