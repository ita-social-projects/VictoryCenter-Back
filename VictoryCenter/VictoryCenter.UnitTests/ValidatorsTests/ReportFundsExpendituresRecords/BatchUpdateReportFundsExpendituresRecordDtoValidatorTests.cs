using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresRecords;
using VictoryCenter.BLL.Validators.ReportFundsExpendituresRecords;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.UnitTests.ValidatorsTests.ReportFundsExpendituresRecords;

public class BatchUpdateReportFundsExpendituresRecordDtoValidatorTests
{
    private readonly BatchUpdateReportFundsExpendituresRecordDtoValidator _validator;

    public BatchUpdateReportFundsExpendituresRecordDtoValidatorTests()
    {
        _validator = new BatchUpdateReportFundsExpendituresRecordDtoValidator(
            new BaseReportFundsExpendituresRecordValidator());
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

    private static BatchUpdateReportFundsExpendituresRecordDto GetValidDto() => new()
    {
        CategoryId = 1,
        Amount = 100.25m,
        Currency = ReportFundsExpendituresCurrency.Uah,
        Id = 1
    };
}
