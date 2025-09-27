using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Donate.CorrespondentBankDetails;
using VictoryCenter.BLL.Validators.Donate;

namespace VictoryCenter.UnitTests.ValidatorsTests.Donate;
public class CreateCorrespondentBankDetailsDtoValidatorTests
{
    private readonly CreateCorrespondentBankDetailsDtoValidator _validator;

    public CreateCorrespondentBankDetailsDtoValidatorTests()
    {
        _validator = new CreateCorrespondentBankDetailsDtoValidator();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_WhenSwiftIsEmpty(string? swift)
    {
        var dto = new CreateCorrespondentBankDetailsDto { Swift = swift, Iban = "1234567890" };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(d => d.Swift)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CorrespondentBankDetailsDto.Swift)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenSwiftIsTooShort()
    {
        var dto = new CreateCorrespondentBankDetailsDto { Swift = "12", Iban = "1234567890" };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(d => d.Swift)
            .WithErrorMessage(ErrorMessagesConstants
                .PropertyMustHaveAMinimumLengthOfNCharacters(nameof(CorrespondentBankDetailsDto.Swift), CorrespondentBankDetailsConstants.Swift.MinLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenSwiftIsTooLong()
    {
        var dto = new CreateCorrespondentBankDetailsDto
        {
            Swift = new string('A', CorrespondentBankDetailsConstants.Swift.MaxLength + 1),
            Iban = "1234567890"
        };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(d => d.Swift)
            .WithErrorMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(nameof(CorrespondentBankDetailsDto.Swift), CorrespondentBankDetailsConstants.Swift.MaxLength));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_ShouldHaveError_WhenIbanIsEmpty(string? iban)
    {
        var dto = new CreateCorrespondentBankDetailsDto { Swift = "VALIDSWIFT", Iban = iban };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(d => d.Iban)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CorrespondentBankDetailsDto.Iban)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenIbanIsTooShort()
    {
        var dto = new CreateCorrespondentBankDetailsDto { Swift = "VALIDSWIFT", Iban = "12" };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(d => d.Iban)
            .WithErrorMessage(ErrorMessagesConstants
                .PropertyMustHaveAMinimumLengthOfNCharacters(nameof(CorrespondentBankDetailsDto.Iban), CorrespondentBankDetailsConstants.Iban.MinLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenIbanIsTooLong()
    {
        var dto = new CreateCorrespondentBankDetailsDto
        {
            Swift = "VALIDSWIFT",
            Iban = new string('1', CorrespondentBankDetailsConstants.Iban.MaxLength + 1)
        };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(d => d.Iban)
            .WithErrorMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(nameof(CorrespondentBankDetailsDto.Iban), CorrespondentBankDetailsConstants.Iban.MaxLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenIbanContainsNonDigits()
    {
        var dto = new CreateCorrespondentBankDetailsDto { Swift = "VALIDSWIFT", Iban = new string('a', UahBankDetailsConstants.Iban.MinLength) };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(d => d.Iban)
            .WithErrorMessage(CorrespondentBankDetailsConstants.OnlyDigitsMessage);
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenDataIsValid()
    {
        var dto = new CreateCorrespondentBankDetailsDto
        {
            Swift = new string('A', CorrespondentBankDetailsConstants.Swift.MinLength),
            Iban = new string('1', CorrespondentBankDetailsConstants.Iban.MinLength)
        };

        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
