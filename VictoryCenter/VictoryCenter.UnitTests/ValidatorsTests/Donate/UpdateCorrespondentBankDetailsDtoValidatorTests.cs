using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Admin.Donate.CorrespondentBankDetails.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Donate.CorrespondentBankDetails;
using VictoryCenter.BLL.Validators.Donate.CorrespondentBankDetails;

namespace VictoryCenter.UnitTests.ValidatorsTests.Donate;

public class UpdateCorrespondentBankDetailsCommandValidatorTests
{
    private readonly UpdateCorrespondentBankDetailsCommandValidator _validator;

    public UpdateCorrespondentBankDetailsCommandValidatorTests()
    {
        _validator = new UpdateCorrespondentBankDetailsCommandValidator();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_WhenSwiftIsEmpty(string? swift)
    {
        var command = new UpdateCorrespondentBankDetailsCommand(
            new UpdateCorrespondentBankDetailsDto
            {
                Swift = swift,
                Name = "Test",
                Account = "Test",
                ForeignBankDetailsId = 1
            },
            1L);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.UpdateCorrespondentBankDetailsDto.Swift)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CorrespondentBankDetailsDto.Swift)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenSwiftTooShort()
    {
        var command = new UpdateCorrespondentBankDetailsCommand(
            new UpdateCorrespondentBankDetailsDto
            {
                Swift = new string('A', CorrespondentBankDetailsConstants.Swift.MinLength - 1),
                Name = "Test",
                Account = "Test",
                ForeignBankDetailsId = 1
            },
            1L);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.UpdateCorrespondentBankDetailsDto.Swift)
            .WithErrorMessage(ErrorMessagesConstants
                .PropertyMustHaveAMinimumLengthOfNCharacters(nameof(CorrespondentBankDetailsDto.Swift), CorrespondentBankDetailsConstants.Swift.MinLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenSwiftTooLong()
    {
        var command = new UpdateCorrespondentBankDetailsCommand(
            new UpdateCorrespondentBankDetailsDto
            {
                Swift = new string('A', CorrespondentBankDetailsConstants.Swift.MaxLength + 1),
                Name = "Test",
                Account = "Test",
                ForeignBankDetailsId = 1
            },
            1L);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.UpdateCorrespondentBankDetailsDto.Swift)
            .WithErrorMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(nameof(CorrespondentBankDetailsDto.Swift), CorrespondentBankDetailsConstants.Swift.MaxLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenIbanTooShort()
    {
        var command = new UpdateCorrespondentBankDetailsCommand(
            new UpdateCorrespondentBankDetailsDto
            {
                Swift = new string('A', CorrespondentBankDetailsConstants.Swift.MinLength),
                Iban = new string('1', CorrespondentBankDetailsConstants.Iban.MinLength - 1),
                Name = "Test",
                Account = "Test",
                ForeignBankDetailsId = 1
            },
            1L);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.UpdateCorrespondentBankDetailsDto.Iban)
            .WithErrorMessage(ErrorMessagesConstants
                .PropertyMustHaveAMinimumLengthOfNCharacters(nameof(CorrespondentBankDetailsDto.Iban), CorrespondentBankDetailsConstants.Iban.MinLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenIbanTooLong()
    {
        var command = new UpdateCorrespondentBankDetailsCommand(
            new UpdateCorrespondentBankDetailsDto
            {
                Swift = new string('A', CorrespondentBankDetailsConstants.Swift.MinLength),
                Iban = new string('1', CorrespondentBankDetailsConstants.Iban.MaxLength + 1),
                Name = "Test",
                Account = "Test",
                ForeignBankDetailsId = 1
            },
            1L);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.UpdateCorrespondentBankDetailsDto.Iban)
            .WithErrorMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(nameof(CorrespondentBankDetailsDto.Iban), CorrespondentBankDetailsConstants.Iban.MaxLength));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_ShouldNotHaveError_WhenIbanIsNullOrEmpty(string? iban)
    {
        var command = new UpdateCorrespondentBankDetailsCommand(
            new UpdateCorrespondentBankDetailsDto
            {
                Swift = new string('A', CorrespondentBankDetailsConstants.Swift.MinLength),
                Iban = iban,
                Name = "Test",
                Account = "Test",
                ForeignBankDetailsId = 1
            },
            1L);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(c => c.UpdateCorrespondentBankDetailsDto.Iban);
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenDataIsValidWithIban()
    {
        var command = new UpdateCorrespondentBankDetailsCommand(
            new UpdateCorrespondentBankDetailsDto
            {
                Swift = new string('A', CorrespondentBankDetailsConstants.Swift.MinLength),
                Iban = new string('1', CorrespondentBankDetailsConstants.Iban.MinLength),
                Name = "Test",
                Account = "Test",
                ForeignBankDetailsId = 1
            },
            1L);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenDataIsValidWithoutIban()
    {
        var command = new UpdateCorrespondentBankDetailsCommand(
            new UpdateCorrespondentBankDetailsDto
            {
                Swift = new string('A', CorrespondentBankDetailsConstants.Swift.MinLength),
                Iban = null,
                Name = "Test",
                Account = "Test",
                ForeignBankDetailsId = 1
            },
            1L);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
