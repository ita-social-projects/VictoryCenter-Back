using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Admin.Donate.UahBankDetails.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Donate.UahBankDetails;
using VictoryCenter.BLL.Validators.Donate.UahBankDetails;

namespace VictoryCenter.UnitTests.ValidatorsTests.Donate;
public class UpdateUahBankDetailsCommandValidatorTests
{
    private readonly UpdateUahBankDetailsCommandValidator _validator;

    public UpdateUahBankDetailsCommandValidatorTests()
    {
        _validator = new UpdateUahBankDetailsCommandValidator();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_ShouldHaveError_WhenEdrpouIsEmpty(string? edrpou)
    {
        var command = new UpdateUahBankDetailsCommand(
            new UpdateUahBankDetailsDto { Edrpou = edrpou, Iban = "1234567890" },
            1L);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.UpdateUahBankDetailsDto.Edrpou)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UahBankDetailsDto.Edrpou)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenEdrpouTooShort()
    {
        var command = new UpdateUahBankDetailsCommand(
            new UpdateUahBankDetailsDto
            {
                Edrpou = new string('1', UahBankDetailsConstants.Edrpou.MinLength - 1),
                Iban = new string('1', UahBankDetailsConstants.Iban.MinLength)
            },
            1L);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.UpdateUahBankDetailsDto.Edrpou)
            .WithErrorMessage(ErrorMessagesConstants
                .PropertyMustHaveAMinimumLengthOfNCharacters(nameof(UahBankDetailsDto.Edrpou), UahBankDetailsConstants.Edrpou.MinLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenEdrpouNotDigits()
    {
        var command = new UpdateUahBankDetailsCommand(
            new UpdateUahBankDetailsDto
            {
                Edrpou = "ABC123AA",
                Iban = new string('1', UahBankDetailsConstants.Iban.MinLength)
            },
            1L);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.UpdateUahBankDetailsDto.Edrpou)
            .WithErrorMessage(UahBankDetailsConstants.OnlyDigitsMessage);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenIbanTooShort()
    {
        var command = new UpdateUahBankDetailsCommand(
            new UpdateUahBankDetailsDto
            {
                Edrpou = new string('1', UahBankDetailsConstants.Edrpou.MinLength),
                Iban = "UA" + new string('1', UahBankDetailsConstants.Iban.MinLength - 3)
            },
            1L);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.UpdateUahBankDetailsDto.Iban)
            .WithErrorMessage(ErrorMessagesConstants
                .PropertyMustHaveAMinimumLengthOfNCharacters(nameof(UahBankDetailsDto.Iban), UahBankDetailsConstants.Iban.MinLength));
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenDataIsValid()
    {
        var command = new UpdateUahBankDetailsCommand(
            new UpdateUahBankDetailsDto
            {
                Edrpou = new string('1', UahBankDetailsConstants.Edrpou.MinLength),
                Iban = "UA" + new string('1', UahBankDetailsConstants.Iban.MinLength - 2)
            },
            1L);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenIbanDoesNotStartWithUA()
    {
        var command = new UpdateUahBankDetailsCommand(
            new UpdateUahBankDetailsDto
            {
                Edrpou = new string('1', UahBankDetailsConstants.Edrpou.MinLength),
                Iban = "US" + new string('1', UahBankDetailsConstants.Iban.MinLength - 2)
            },
            1L);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.UpdateUahBankDetailsDto.Iban)
            .WithErrorMessage(ErrorMessagesConstants.OnlyDigitsAllowed());
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenIbanContainsInvalidCharacters()
    {
        var command = new UpdateUahBankDetailsCommand(
            new UpdateUahBankDetailsDto
            {
                Edrpou = new string('1', UahBankDetailsConstants.Edrpou.MinLength),
                Iban = "UA12345A7890"
            },
            1L);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.UpdateUahBankDetailsDto.Iban)
            .WithErrorMessage(ErrorMessagesConstants.OnlyDigitsAllowed());
    }
}
