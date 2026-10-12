using FluentValidation;
using FluentValidation.TestHelper;
using Moq;
using VictoryCenter.BLL.Commands.Admin.ReportProgramExpendituresRecords.BatchSave;
using VictoryCenter.BLL.DTOs.Admin.ReportProgramExpendituresRecords;
using VictoryCenter.BLL.Validators.ReportProgramExpendituresRecords;

namespace VictoryCenter.UnitTests.ValidatorsTests.ReportProgramExpendituresRecords;

public class BatchSaveReportProgramExpendituresRecordsCommandValidatorTests
{
    private readonly BatchSaveReportProgramExpendituresRecordsCommandValidator _validator;
    private readonly Mock<IValidator<CreateReportProgramExpendituresRecordDto>> _createDtoValidatorMock;
    private readonly Mock<IValidator<BatchUpdateReportProgramExpendituresRecordDto>> _updateDtoValidatorMock;

    public BatchSaveReportProgramExpendituresRecordsCommandValidatorTests()
    {
        _createDtoValidatorMock = new Mock<IValidator<CreateReportProgramExpendituresRecordDto>>();
        _updateDtoValidatorMock = new Mock<IValidator<BatchUpdateReportProgramExpendituresRecordDto>>();
        _validator = new BatchSaveReportProgramExpendituresRecordsCommandValidator(
            _createDtoValidatorMock.Object,
            _updateDtoValidatorMock.Object);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenBatchSaveDtoIsNull()
    {
        // Arrange
        var command = new BatchSaveReportProgramExpendituresRecordCommand(null!);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BatchSaveReportProgramExpendituresRecordsDto);
    }

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenAllDataIsValid()
    {
        // Arrange
        var dto = new BatchSaveReportProgramExpendituresRecordsDto
        {
            RecordsToCreate = [new() { HippotherapyProgramCategoryId = 1, AmountUah = 101.25m, AmountUsd = 50.5m }],
            RecordsToUpdate = [new() { Id = 1, HippotherapyProgramCategoryId = 2 }, new() { Id = 2, HippotherapyProgramCategoryId = 3 }],
            RecordIdsToDelete = [10, 11]
        };
        var command = new BatchSaveReportProgramExpendituresRecordCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
