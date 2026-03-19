using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Admin.ReportFundsExpendituresCategories.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresCategories;
using VictoryCenter.BLL.Validators.ReportFundsExpendituresCategories;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.UnitTests.ValidatorsTests.ReportFundsExpendituresCategories;

public class CreateReportFundsExpendituresCategoryValidatorTests
{
    private readonly CreateReportFundsExpendituresCategoryValidator _validator;

    public CreateReportFundsExpendituresCategoryValidatorTests()
    {
        _validator = new CreateReportFundsExpendituresCategoryValidator(
            new BaseReportFundsExpendituresCategoryValidator());
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenDtoIsNull()
    {
        // Arrange
        var command = new CreateReportFundsExpendituresCategoryCommand(null!);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CreateReportFundsExpendituresCategoryDto);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_WhenNameIsEmpty(string? name)
    {
        // Arrange
        var dto = GetValidDto() with { Name = name! };
        var command = new CreateReportFundsExpendituresCategoryCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CreateReportFundsExpendituresCategoryDto.Name)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(ReportFundsExpendituresCategoryDto.Name)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenTypeIsInvalid()
    {
        // Arrange
        var dto = GetValidDto() with { Type = (ReportFundsExpendituresType)99 };
        var command = new CreateReportFundsExpendituresCategoryCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CreateReportFundsExpendituresCategoryDto.Type)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBeValidEnum(
                nameof(ReportFundsExpendituresCategoryDto.Type)));
    }

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenDataIsValid()
    {
        // Arrange
        var command = new CreateReportFundsExpendituresCategoryCommand(GetValidDto());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    private static CreateReportFundsExpendituresCategoryDto GetValidDto() => new()
    {
        Name = "Valid Category",
        Type = ReportFundsExpendituresType.Income
    };
}
