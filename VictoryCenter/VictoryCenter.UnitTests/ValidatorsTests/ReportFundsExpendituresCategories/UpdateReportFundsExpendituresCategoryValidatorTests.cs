using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Admin.ReportFundsExpendituresCategories.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresCategories;
using VictoryCenter.BLL.Validators.ReportFundsExpendituresCategories;

namespace VictoryCenter.UnitTests.ValidatorsTests.ReportFundsExpendituresCategories;

public class UpdateReportFundsExpendituresCategoryValidatorTests
{
    private readonly UpdateReportFundsExpendituresCategoryValidator _validator;

    public UpdateReportFundsExpendituresCategoryValidatorTests()
    {
        _validator = new UpdateReportFundsExpendituresCategoryValidator();
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenDtoIsNull()
    {
        // Arrange
        var command = new UpdateReportFundsExpendituresCategoryCommand(null!, 1);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UpdateReportFundsExpendituresCategoryDto);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_WhenNameIsEmpty(string? name)
    {
        // Arrange
        var dto = GetValidDto() with { Name = name! };
        var command = new UpdateReportFundsExpendituresCategoryCommand(dto, 1);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UpdateReportFundsExpendituresCategoryDto.Name)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(ReportFundsExpendituresCategoryDto.Name)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenNameExceedsMaxLength()
    {
        // Arrange
        var dto = GetValidDto() with
        {
            Name = new string('A', ReportFundsExpendituresCategoryConstants.NameMaxLength + 1)
        };
        var command = new UpdateReportFundsExpendituresCategoryCommand(dto, 1);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UpdateReportFundsExpendituresCategoryDto.Name)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(ReportFundsExpendituresCategoryDto.Name),
                ReportFundsExpendituresCategoryConstants.NameMaxLength));
    }

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenDataIsValid()
    {
        // Arrange
        var command = new UpdateReportFundsExpendituresCategoryCommand(GetValidDto(), 1);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    private static UpdateReportFundsExpendituresCategoryDto GetValidDto() => new()
    {
        Name = "Valid Category"
    };
}
