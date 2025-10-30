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
        var command = new CreateUahBankDetailsCommand(
            new CreateUahBankDetailsDto { Edrpou = edrpou, Iban = new string('1', UahBankDetailsConstants.Iban.MinLength) });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.CreateUahBankDetailsDto.Edrpou)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UahBankDetailsDto.Edrpou)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenEdrpouTooShort()
    {
        var command = new CreateUahBankDetailsCommand(
            new CreateUahBankDetailsDto
            {
                Edrpou = new string('1', UahBankDetailsConstants.Edrpou.MinLength - 1),
                Iban = new string('1', UahBankDetailsConstants.Iban.MinLength)
            });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.CreateUahBankDetailsDto.Edrpou)
            .WithErrorMessage(ErrorMessagesConstants
                .PropertyMustHaveAMinimumLengthOfNCharacters(nameof(UahBankDetailsDto.Edrpou), UahBankDetailsConstants.Edrpou.MinLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenEdrpouTooLong()
    {
        var command = new CreateUahBankDetailsCommand(
            new CreateUahBankDetailsDto
            {
                Edrpou = new string('1', UahBankDetailsConstants.Edrpou.MaxLength + 1),
                Iban = new string('1', UahBankDetailsConstants.Iban.MinLength)
            });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.CreateUahBankDetailsDto.Edrpou)
            .WithErrorMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(nameof(UahBankDetailsDto.Edrpou), UahBankDetailsConstants.Edrpou.MaxLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenEdrpouNotDigits()
    {
        var command = new CreateUahBankDetailsCommand(
            new CreateUahBankDetailsDto
            {
                Edrpou = "A2345678",
                Iban = new string('1', UahBankDetailsConstants.Iban.MinLength)
            });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.CreateUahBankDetailsDto.Edrpou)
            .WithErrorMessage(UahBankDetailsConstants.OnlyDigitsMessage);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_WhenIbanIsEmpty(string? iban)
    {
        var command = new CreateUahBankDetailsCommand(
            new CreateUahBankDetailsDto { Edrpou = new string('1', UahBankDetailsConstants.Edrpou.MinLength), Iban = iban });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.CreateUahBankDetailsDto.Iban)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UahBankDetailsDto.Iban)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenIbanTooShort()
    {
        var command = new CreateUahBankDetailsCommand(
            new CreateUahBankDetailsDto
            {
                Edrpou = new string('1', UahBankDetailsConstants.Edrpou.MinLength),
                Iban = new string('1', UahBankDetailsConstants.Iban.MinLength - 1)
            });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.CreateUahBankDetailsDto.Iban)
            .WithErrorMessage(ErrorMessagesConstants
                .PropertyMustHaveAMinimumLengthOfNCharacters(nameof(UahBankDetailsDto.Iban), UahBankDetailsConstants.Iban.MinLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenIbanTooLong()
    {
        var command = new CreateUahBankDetailsCommand(
            new CreateUahBankDetailsDto
            {
                Edrpou = new string('1', UahBankDetailsConstants.Edrpou.MinLength),
                Iban = new string('1', UahBankDetailsConstants.Iban.MaxLength + 1)
            });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.CreateUahBankDetailsDto.Iban)
            .WithErrorMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(nameof(UahBankDetailsDto.Iban), UahBankDetailsConstants.Iban.MaxLength));
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenDataIsValid()
    {
        var command = new CreateUahBankDetailsCommand(
            new CreateUahBankDetailsDto
            {
                Edrpou = new string('1', UahBankDetailsConstants.Edrpou.MinLength),
                Iban = new string('1', UahBankDetailsConstants.Iban.MinLength)
            });

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
