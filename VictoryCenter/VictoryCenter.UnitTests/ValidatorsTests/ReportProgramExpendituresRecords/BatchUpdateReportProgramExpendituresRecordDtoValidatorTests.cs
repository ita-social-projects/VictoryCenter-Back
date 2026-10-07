using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresRecords;
using VictoryCenter.BLL.DTOs.Admin.ReportProgramExpendituresRecords;
using VictoryCenter.BLL.Validators.ReportProgramExpendituresRecords;

namespace VictoryCenter.UnitTests.ValidatorsTests.ReportProgramExpendituresRecords;

public class BatchUpdateReportProgramExpendituresRecordDtoValidatorTests
{
    private readonly BatchUpdateReportProgramExpendituresRecordDtoValidator _validator;

    public BatchUpdateReportProgramExpendituresRecordDtoValidatorTests()
    {
        _validator = new BatchUpdateReportProgramExpendituresRecordDtoValidator(
            new BaseReportProgramExpendituresRecordValidator());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_ShouldHaveError_WhenIdIsNotPositive(long invalidId)
    {
        // Arrange
        var dto = GetValidDto() with { Id = invalidId };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBePositive(
                nameof(ReportFundsExpendituresRecordDto.Id)));
    }

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenDataIsValid()
    {
        // Arrange
        var dto = GetValidDto();

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    private static BatchUpdateReportProgramExpendituresRecordDto GetValidDto() => new()
    {
        Id = 1,
        HippotherapyProgramCategoryId = 1,
        AmountUah = 100.25m,
        AmountUsd = 50.25m
    };
}
