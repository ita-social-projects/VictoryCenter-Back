using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Donate.CorrespondentBankDetails;
using VictoryCenter.BLL.Validators.Donate.CorrespondentBankDetails;

namespace VictoryCenter.UnitTests.ValidatorsTests.Donate;
public class UpdateCorrespondentBankDetailsDtoValidatorTests
{
    private readonly UpdateCorrespondentBankDetailsDtoValidator _validator;

    public UpdateCorrespondentBankDetailsDtoValidatorTests()
    {
        _validator = new UpdateCorrespondentBankDetailsDtoValidator();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_WhenSwiftIsEmpty(string? swift)
    {
        var dto = new UpdateCorrespondentBankDetailsDto
        {
            Swift = swift,
            Iban = new string('1', CorrespondentBankDetailsConstants.Iban.MinLength)
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(d => d.Swift)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CorrespondentBankDetailsDto.Swift)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenSwiftTooShort()
    {
        var dto = new UpdateCorrespondentBankDetailsDto
        {
            Swift = new string('A', CorrespondentBankDetailsConstants.Swift.MinLength - 1),
            Iban = new string('1', CorrespondentBankDetailsConstants.Iban.MinLength)
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(d => d.Swift)
            .WithErrorMessage(ErrorMessagesConstants
                .PropertyMustHaveAMinimumLengthOfNCharacters(nameof(CorrespondentBankDetailsDto.Swift), CorrespondentBankDetailsConstants.Swift.MinLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenSwiftTooLong()
    {
        var dto = new UpdateCorrespondentBankDetailsDto
        {
            Swift = new string('A', CorrespondentBankDetailsConstants.Swift.MaxLength + 1),
            Iban = new string('1', CorrespondentBankDetailsConstants.Iban.MinLength)
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(d => d.Swift)
            .WithErrorMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(nameof(CorrespondentBankDetailsDto.Swift), CorrespondentBankDetailsConstants.Swift.MaxLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenIbanTooShort()
    {
        var dto = new UpdateCorrespondentBankDetailsDto
        {
            Swift = new string('A', CorrespondentBankDetailsConstants.Swift.MinLength),
            Iban = new string('1', CorrespondentBankDetailsConstants.Iban.MinLength - 1)
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(d => d.Iban)
            .WithErrorMessage(ErrorMessagesConstants
                .PropertyMustHaveAMinimumLengthOfNCharacters(nameof(CorrespondentBankDetailsDto.Iban), CorrespondentBankDetailsConstants.Iban.MinLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenIbanTooLong()
    {
        var dto = new UpdateCorrespondentBankDetailsDto
        {
            Swift = new string('A', CorrespondentBankDetailsConstants.Swift.MinLength),
            Iban = new string('1', CorrespondentBankDetailsConstants.Iban.MaxLength + 1)
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(d => d.Iban)
            .WithErrorMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(nameof(CorrespondentBankDetailsDto.Iban), CorrespondentBankDetailsConstants.Iban.MaxLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenIbanNotDigits()
    {
        var dto = new UpdateCorrespondentBankDetailsDto
        {
            Swift = new string('A', CorrespondentBankDetailsConstants.Swift.MinLength),
            Iban = new string('a', CorrespondentBankDetailsConstants.Iban.MinLength)
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(d => d.Iban)
            .WithErrorMessage(CorrespondentBankDetailsConstants.OnlyDigitsMessage);
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenDataIsValid()
    {
        var dto = new UpdateCorrespondentBankDetailsDto
        {
            Swift = new string('A', CorrespondentBankDetailsConstants.Swift.MinLength),
            Iban = new string('1', CorrespondentBankDetailsConstants.Iban.MinLength)
        };

        var result = _validator.TestValidate(dto);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
