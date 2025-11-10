using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Admin.Donate.ForeignBankDetails.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Donate.ForeignBankDetails;
using VictoryCenter.BLL.Validators.Donate.ForeignBankDetails;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.UnitTests.ValidatorsTests.Donate;
public class CreateForeignBankDetailsCommandValidatorTests
{
    private readonly CreateForeignBankDetailsCommandValidator _validator;

    public CreateForeignBankDetailsCommandValidatorTests()
    {
        _validator = new CreateForeignBankDetailsCommandValidator();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_WhenSwiftIsEmpty(string? swift)
    {
        // Arrange
        var dto = GetValidDto() with { Swift = swift! };
        var command = new CreateForeignBankDetailsCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.CreateForeignBankDetailsDto.Swift)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(ForeignBankDetailsDto.Swift)));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_WhenIbanIsEmpty(string? iban)
    {
        // Arrange
        var dto = GetValidDto() with { Iban = iban! };
        var command = new CreateForeignBankDetailsCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.CreateForeignBankDetailsDto.Iban)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(ForeignBankDetailsDto.Iban)));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_WhenNameIsEmpty(string? name)
    {
        // Arrange
        var dto = GetValidDto() with { Name = name! };
        var command = new CreateForeignBankDetailsCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.CreateForeignBankDetailsDto.Name)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(ForeignBankDetailsDto.Name)));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_WhenReceiverIsEmpty(string? receiver)
    {
        // Arrange
        var dto = GetValidDto() with { Receiver = receiver! };
        var command = new CreateForeignBankDetailsCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.CreateForeignBankDetailsDto.Receiver)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(ForeignBankDetailsDto.Receiver)));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_WhenAddressIsEmpty(string? address)
    {
        // Arrange
        var dto = GetValidDto() with { Address = address! };
        var command = new CreateForeignBankDetailsCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.CreateForeignBankDetailsDto.Address)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(ForeignBankDetailsDto.Address)));
    }

    [Theory]
    [InlineData(BankCurrency.Uah)]
    [InlineData((BankCurrency)99)]
    public void Validate_ShouldHaveError_WhenCurrencyIsInvalid(BankCurrency currency)
    {
        // Arrange
        var dto = GetValidDto() with { Currency = currency };
        var command = new CreateForeignBankDetailsCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.CreateForeignBankDetailsDto.Currency)
            .WithErrorMessage(ForeignBankDetailsConstants.OnlyUsdOrEurMessage);
    }

    [Theory]
    [InlineData(BankCurrency.Usd)]
    [InlineData(BankCurrency.Eur)]
    public void Validate_ShouldNotHaveError_WhenDataIsValid(BankCurrency currency)
    {
        // Arrange
        var dto = GetValidDto() with { Currency = currency };
        var command = new CreateForeignBankDetailsCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    private static CreateForeignBankDetailsDto GetValidDto() => new()
    {
        Swift = "VALIDSWIFT",
        Iban = "UA1234567890123456789",
        Name = "Valid Name",
        Receiver = "Valid Receiver",
        Address = "Valid Address",
        Currency = BankCurrency.Usd
    };
}
