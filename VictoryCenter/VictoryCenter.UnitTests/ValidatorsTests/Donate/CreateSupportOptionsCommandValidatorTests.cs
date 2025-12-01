using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Admin.Donate.SupportOptions.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Donate.SupportOptions;
using VictoryCenter.BLL.Validators.Donate.SupportOptions;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.UnitTests.ValidatorsTests.Donate;
public class CreateSupportOptionsCommandValidatorTests
{
    private readonly CreateSupportOptionsCommandValidator _validator;

    public CreateSupportOptionsCommandValidatorTests()
    {
        _validator = new CreateSupportOptionsCommandValidator();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_WhenNameIsEmpty(string? name)
    {
        // Arrange
        var dto = GetValidDto() with { Name = name! };
        var command = new CreateSupportOptionsCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.CreateSupportOptionsDto.Name)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(SupportOptionsDto.Name)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenNameExceedsMaxLength()
    {
        // Arrange
        var longName = new string('a', SupportOptionsConstants.NameMaxLength + 1);
        var dto = GetValidDto() with { Name = longName };
        var command = new CreateSupportOptionsCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.CreateSupportOptionsDto.Name)
            .WithErrorMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(
                    nameof(SupportOptionsDto.Name),
                    SupportOptionsConstants.NameMaxLength));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_WhenValueIsEmpty(string? value)
    {
        // Arrange
        var dto = GetValidDto() with { Value = value! };
        var command = new CreateSupportOptionsCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.CreateSupportOptionsDto.Value)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(SupportOptionsDto.Value)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenValueExceedsMaxLength()
    {
        // Arrange
        var longValue = new string('a', SupportOptionsConstants.ValueMaxLength + 1);
        var dto = GetValidDto() with { Value = longValue };
        var command = new CreateSupportOptionsCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.CreateSupportOptionsDto.Value)
            .WithErrorMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(
                    nameof(SupportOptionsDto.Value),
                    SupportOptionsConstants.ValueMaxLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenCurrencyIsInvalid()
    {
        // Arrange
        var dto = GetValidDto() with { Currency = (BankCurrency)99 };
        var command = new CreateSupportOptionsCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.CreateSupportOptionsDto.Currency)
            .WithErrorMessage(SupportOptionsConstants.OnlyUsdOrEurOrUahMessage);
    }

    [Theory]
    [InlineData(BankCurrency.Uah)]
    [InlineData(BankCurrency.Usd)]
    [InlineData(BankCurrency.Eur)]
    public void Validate_ShouldNotHaveError_WhenDataIsValid(BankCurrency currency)
    {
        // Arrange
        var dto = GetValidDto() with { Currency = currency };
        var command = new CreateSupportOptionsCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    private static CreateSupportOptionsDto GetValidDto() => new()
    {
        Name = "Valid Name",
        Value = "Valid Value",
        Currency = BankCurrency.Uah
    };
}
