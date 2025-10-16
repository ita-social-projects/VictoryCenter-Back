using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Admin.Donate.CorrespondentBankDetails.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Donate.CorrespondentBankDetails;
using VictoryCenter.BLL.Validators.Donate.CorrespondentBankDetails;

namespace VictoryCenter.UnitTests.ValidatorsTests.Donate;

public class CreateCorrespondentBankDetailsCommandValidatorTests
{
    private readonly CreateCorrespondentBankDetailsCommandValidator _validator;

    public CreateCorrespondentBankDetailsCommandValidatorTests()
    {
        _validator = new CreateCorrespondentBankDetailsCommandValidator();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_WhenSwiftIsEmpty(string? swift)
    {
        var command = new CreateCorrespondentBankDetailsCommand(
            new CreateCorrespondentBankDetailsDto
            {
                Swift = swift,
                Name = "Test",
                Account = "Test",
                ForeignBankDetailsId = 1
            });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.CreateCorrespondentBankDetailsDto.Swift)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CorrespondentBankDetailsDto.Swift)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenSwiftTooShort()
    {
        var command = new CreateCorrespondentBankDetailsCommand(
            new CreateCorrespondentBankDetailsDto
            {
                Swift = new string('A', CorrespondentBankDetailsConstants.Swift.MinLength - 1),
                Name = "Test",
                Account = "Test",
                ForeignBankDetailsId = 1
            });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.CreateCorrespondentBankDetailsDto.Swift)
            .WithErrorMessage(ErrorMessagesConstants
                .PropertyMustHaveAMinimumLengthOfNCharacters(nameof(CorrespondentBankDetailsDto.Swift), CorrespondentBankDetailsConstants.Swift.MinLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenSwiftTooLong()
    {
        var command = new CreateCorrespondentBankDetailsCommand(
            new CreateCorrespondentBankDetailsDto
            {
                Swift = new string('A', CorrespondentBankDetailsConstants.Swift.MaxLength + 1),
                Name = "Test",
                Account = "Test",
                ForeignBankDetailsId = 1
            });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.CreateCorrespondentBankDetailsDto.Swift)
            .WithErrorMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(nameof(CorrespondentBankDetailsDto.Swift), CorrespondentBankDetailsConstants.Swift.MaxLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenIbanTooShort()
    {
        var command = new CreateCorrespondentBankDetailsCommand(
            new CreateCorrespondentBankDetailsDto
            {
                Swift = new string('A', CorrespondentBankDetailsConstants.Swift.MinLength),
                Iban = new string('1', CorrespondentBankDetailsConstants.Iban.MinLength - 1),
                Name = "Test",
                Account = "Test",
                ForeignBankDetailsId = 1
            });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.CreateCorrespondentBankDetailsDto.Iban)
            .WithErrorMessage(ErrorMessagesConstants
                .PropertyMustHaveAMinimumLengthOfNCharacters(nameof(CorrespondentBankDetailsDto.Iban), CorrespondentBankDetailsConstants.Iban.MinLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenIbanTooLong()
    {
        var command = new CreateCorrespondentBankDetailsCommand(
            new CreateCorrespondentBankDetailsDto
            {
                Swift = new string('A', CorrespondentBankDetailsConstants.Swift.MinLength),
                Iban = new string('1', CorrespondentBankDetailsConstants.Iban.MaxLength + 1),
                Name = "Test",
                Account = "Test",
                ForeignBankDetailsId = 1
            });

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.CreateCorrespondentBankDetailsDto.Iban)
            .WithErrorMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(nameof(CorrespondentBankDetailsDto.Iban), CorrespondentBankDetailsConstants.Iban.MaxLength));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_ShouldNotHaveError_WhenIbanIsNullOrEmpty(string? iban)
    {
        var command = new CreateCorrespondentBankDetailsCommand(
            new CreateCorrespondentBankDetailsDto
            {
                Swift = new string('A', CorrespondentBankDetailsConstants.Swift.MinLength),
                Iban = iban,
                Name = "Test",
                Account = "Test",
                ForeignBankDetailsId = 1
            });

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(c => c.CreateCorrespondentBankDetailsDto.Iban);
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenDataIsValidWithIban()
    {
        var command = new CreateCorrespondentBankDetailsCommand(
            new CreateCorrespondentBankDetailsDto
            {
                Swift = new string('A', CorrespondentBankDetailsConstants.Swift.MinLength),
                Iban = new string('1', CorrespondentBankDetailsConstants.Iban.MinLength),
                Name = "Test",
                Account = "Test",
                ForeignBankDetailsId = 1
            });

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenDataIsValidWithoutIban()
    {
        var command = new CreateCorrespondentBankDetailsCommand(
            new CreateCorrespondentBankDetailsDto
            {
                Swift = new string('A', CorrespondentBankDetailsConstants.Swift.MinLength),
                Iban = null,
                Name = "Test",
                Account = "Test",
                ForeignBankDetailsId = 1
            });

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
