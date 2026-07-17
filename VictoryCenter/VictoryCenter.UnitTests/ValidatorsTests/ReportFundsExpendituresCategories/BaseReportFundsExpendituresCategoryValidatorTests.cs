using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresCategories;
using VictoryCenter.BLL.Validators.ReportFundsExpendituresCategories;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.UnitTests.ValidatorsTests.ReportFundsExpendituresCategories;

public class BaseReportFundsExpendituresCategoryValidatorTests
{
    private readonly BaseReportFundsExpendituresCategoryValidator _validator;

    public BaseReportFundsExpendituresCategoryValidatorTests()
    {
        _validator = new BaseReportFundsExpendituresCategoryValidator();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_WhenNameIsEmpty(string? name)
    {
        // Arrange
        var dto = GetValidDto() with { Name = name! };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
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

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(ReportFundsExpendituresCategoryDto.Name),
                ReportFundsExpendituresCategoryConstants.NameMaxLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenTypeIsInvalid()
    {
        // Arrange
        var dto = GetValidDto() with { Type = (ReportFundsExpendituresType)99 };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Type)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBeValidEnum(
                nameof(ReportFundsExpendituresCategoryDto.Type)));
    }

    [Theory]
    [InlineData("ПРОГРАМНІ")]
    [InlineData("програмні тест")]
    [InlineData("Програмні тест 2")]
    public void Validate_ShouldHaveError_WhenNameIsReservedAndTypeIsExpense(string name)
    {
        // Arrange
        var dto = GetValidDto() with { Name = name, Type = ReportFundsExpendituresType.Expense };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage(ReportFundsExpendituresCategoryConstants.ReservedCategoryName);
    }

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenNameIsReservedButTypeIsIncome()
    {
        // Arrange
        var dto = GetValidDto() with { Name = "Програмні тест 2", Type = ReportFundsExpendituresType.Income };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Theory]
    [InlineData(ReportFundsExpendituresType.Income)]
    [InlineData(ReportFundsExpendituresType.Expense)]
    public void Validate_ShouldNotHaveErrors_WhenDataIsValid(ReportFundsExpendituresType type)
    {
        // Arrange
        var dto = GetValidDto() with { Type = type };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    private static CreateReportFundsExpendituresCategoryDto GetValidDto() => new()
    {
        Name = "Valid Category",
        Type = ReportFundsExpendituresType.Income
    };
}
