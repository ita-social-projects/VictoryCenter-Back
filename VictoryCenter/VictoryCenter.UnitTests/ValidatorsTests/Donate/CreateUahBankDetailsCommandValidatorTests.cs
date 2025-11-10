using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Admin.Donate.UahBankDetails.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Donate.UahBankDetails;
using VictoryCenter.BLL.Validators.Donate.UahBankDetails;

namespace VictoryCenter.UnitTests.ValidatorsTests.Donate;
public class CreateUahBankDetailsCommandValidatorTests
{
    private readonly CreateUahBankDetailsCommandValidator _validator;

    public CreateUahBankDetailsCommandValidatorTests()
    {
        _validator = new CreateUahBankDetailsCommandValidator();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_WhenEdrpouIsEmpty(string? edrpou)
    {
        // Arrange
        var dto = GetValidDto() with { Edrpou = edrpou! };
        var command = new CreateUahBankDetailsCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.CreateUahBankDetailsDto.Edrpou)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UahBankDetailsDto.Edrpou)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenEdrpouNotDigits()
    {
        // Arrange
        var dto = GetValidDto() with { Edrpou = "A2345678" };
        var command = new CreateUahBankDetailsCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.CreateUahBankDetailsDto.Edrpou)
            .WithErrorMessage(UahBankDetailsConstants.OnlyDigitsMessage);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_WhenIbanIsEmpty(string? iban)
    {
        // Arrange
        var dto = GetValidDto() with { Iban = iban! };
        var command = new CreateUahBankDetailsCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.CreateUahBankDetailsDto.Iban)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UahBankDetailsDto.Iban)));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_WhenNameIsEmpty(string? name)
    {
        // Arrange
        var dto = GetValidDto() with { Name = name! };
        var command = new CreateUahBankDetailsCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.CreateUahBankDetailsDto.Name)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UahBankDetailsDto.Name)));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_WhenReceiverIsEmpty(string? receiver)
    {
        // Arrange
        var dto = GetValidDto() with { Receiver = receiver! };
        var command = new CreateUahBankDetailsCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.CreateUahBankDetailsDto.Receiver)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UahBankDetailsDto.Receiver)));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_WhenPaymentPurposeIsEmpty(string? paymentPurpose)
    {
        // Arrange
        var dto = GetValidDto() with { PaymentPurpose = paymentPurpose! };
        var command = new CreateUahBankDetailsCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.CreateUahBankDetailsDto.PaymentPurpose)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UahBankDetailsDto.PaymentPurpose)));
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenDataIsValid()
    {
        // Arrange
        var dto = GetValidDto();
        var command = new CreateUahBankDetailsCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    private static CreateUahBankDetailsDto GetValidDto() => new()
    {
        Edrpou = "12345678",
        Iban = "UA1234567890123456789012345",
        Name = "Valid Name",
        Receiver = "Valid Receiver",
        PaymentPurpose = "Valid Payment Purpose"
    };
}
