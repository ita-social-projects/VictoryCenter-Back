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
    public void Validate_ShouldHaveError_WhenEdrpouIsTooShort()
    {
        // Arrange
        var dto = GetValidDto() with { Edrpou = new string('1', UahBankDetailsConstants.Edrpou.MinLength - 1) };
        var command = new CreateUahBankDetailsCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.CreateUahBankDetailsDto.Edrpou)
            .WithErrorMessage(ErrorMessagesConstants
                .PropertyMustHaveAMinimumLengthOfNCharacters(nameof(UahBankDetailsDto.Edrpou), UahBankDetailsConstants.Edrpou.MinLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenEdrpouIsTooLong()
    {
        // Arrange
        var dto = GetValidDto() with { Edrpou = new string('1', UahBankDetailsConstants.Edrpou.MaxLength + 1) };
        var command = new CreateUahBankDetailsCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.CreateUahBankDetailsDto.Edrpou)
            .WithErrorMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(nameof(UahBankDetailsDto.Edrpou), UahBankDetailsConstants.Edrpou.MaxLength));
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
            .WithErrorMessage(ErrorMessagesConstants
                .PropertyMustContainOnlyDigits(nameof(UahBankDetailsDto.Edrpou)));
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

    [Fact]
    public void Validate_ShouldHaveError_WhenIbanIsTooShort()
    {
        // Arrange
        var dto = GetValidDto() with { Iban = "UA" + new string('1', UahBankDetailsConstants.Iban.MinLength - 3) };
        var command = new CreateUahBankDetailsCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.CreateUahBankDetailsDto.Iban)
            .WithErrorMessage(ErrorMessagesConstants
                .PropertyMustHaveAMinimumLengthOfNCharacters(nameof(UahBankDetailsDto.Iban), UahBankDetailsConstants.Iban.MinLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenIbanIsTooLong()
    {
        // Arrange
        var dto = GetValidDto() with { Iban = "UA" + new string('1', UahBankDetailsConstants.Iban.MaxLength + 1) };
        var command = new CreateUahBankDetailsCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.CreateUahBankDetailsDto.Iban)
            .WithErrorMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(nameof(UahBankDetailsDto.Iban), UahBankDetailsConstants.Iban.MaxLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenIbanDoesNotStartWithUa()
    {
        // Arrange
        var dto = GetValidDto() with { Iban = "XX123456789012345678901234567" };
        var command = new CreateUahBankDetailsCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.CreateUahBankDetailsDto.Iban)
            .WithErrorMessage(UahBankDetailsConstants.IbanMustStartWithUaFollowedByDigits);
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
    public void Validate_ShouldHaveError_WhenNameIsTooLong()
    {
        // Arrange
        var dto = GetValidDto() with { Name = new string('A', UahBankDetailsConstants.NameMaxLength + 1) };
        var command = new CreateUahBankDetailsCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.CreateUahBankDetailsDto.Name)
            .WithErrorMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(
                    nameof(UahBankDetailsDto.Name),
                    UahBankDetailsConstants.NameMaxLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenReceiverIsTooLong()
    {
        // Arrange
        var dto = GetValidDto() with { Receiver = new string('A', UahBankDetailsConstants.ReceiverMaxLength + 1) };
        var command = new CreateUahBankDetailsCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.CreateUahBankDetailsDto.Receiver)
            .WithErrorMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(
                    nameof(UahBankDetailsDto.Receiver),
                    UahBankDetailsConstants.ReceiverMaxLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenPaymentPurposeIsTooLong()
    {
        // Arrange
        var dto = GetValidDto() with { PaymentPurpose = new string('A', UahBankDetailsConstants.PaymentPurposeMaxLength + 1) };
        var command = new CreateUahBankDetailsCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.CreateUahBankDetailsDto.PaymentPurpose)
            .WithErrorMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(
                    nameof(UahBankDetailsDto.PaymentPurpose),
                    UahBankDetailsConstants.PaymentPurposeMaxLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenIbanContainsInvalidCharacters()
    {
        // Assert
        var dto = GetValidDto() with { Iban = "UA12345678901234567890123456j" };
        var command = new CreateUahBankDetailsCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Arrange
        result.ShouldHaveValidationErrorFor(c => c.CreateUahBankDetailsDto.Iban)
            .WithErrorMessage(UahBankDetailsConstants.IbanMustStartWithUaFollowedByDigits);
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
        Iban = "UA123456789012345678901234567",
        Name = "Valid Name",
        Receiver = "Valid Receiver",
        PaymentPurpose = "Valid Payment Purpose"
    };
}
