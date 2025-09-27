using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Admin.Donate.ForeignBankDetails.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Donate.CorrespondentBankDetails;
using VictoryCenter.BLL.DTOs.Admin.Donate.ForeignBankDetails;
using VictoryCenter.BLL.Validators.Donate;

namespace VictoryCenter.UnitTests.ValidatorsTests.Donate;
public class UpdateForeignBankDetailsCommandValidatorTests
{
    private readonly UpdateForeignBankDetailsCommandValidator _validator;

    public UpdateForeignBankDetailsCommandValidatorTests()
    {
        _validator = new UpdateForeignBankDetailsCommandValidator();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_ShouldHaveError_WhenSwiftIsEmpty(string? swift)
    {
        var command = new UpdateForeignBankDetailsCommand(
            new UpdateForeignBankDetailsDto
            {
                Swift = swift,
                Iban = new string('1', ForeignBankDetailsConstants.Iban.MinLength)
            },
            1L);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.UpdateForeignBankDetailsDto.Swift)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(ForeignBankDetailsDto.Swift)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenSwiftTooShort()
    {
        var command = new UpdateForeignBankDetailsCommand(
            new UpdateForeignBankDetailsDto
            {
                Swift = new string('A', ForeignBankDetailsConstants.Swift.MinLength - 1),
                Iban = new string('1', ForeignBankDetailsConstants.Iban.MinLength)
            },
            1L);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.UpdateForeignBankDetailsDto.Swift)
            .WithErrorMessage(ErrorMessagesConstants
                .PropertyMustHaveAMinimumLengthOfNCharacters(nameof(ForeignBankDetailsDto.Swift), ForeignBankDetailsConstants.Swift.MinLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenIbanNotDigits()
    {
        var command = new UpdateForeignBankDetailsCommand(
            new UpdateForeignBankDetailsDto
            {
                Swift = new string('A', ForeignBankDetailsConstants.Swift.MinLength),
                Iban = new string('a', ForeignBankDetailsConstants.Iban.MinLength)
            },
            1L);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.UpdateForeignBankDetailsDto.Iban)
            .WithErrorMessage(ForeignBankDetailsConstants.OnlyDigitsMessage);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenCorrespondentBankInvalid()
    {
        var command = new UpdateForeignBankDetailsCommand(
            new UpdateForeignBankDetailsDto
            {
                Swift = new string('A', ForeignBankDetailsConstants.Swift.MinLength),
                Iban = new string('1', ForeignBankDetailsConstants.Iban.MinLength),
                CorrespondentBanks = new List<UpdateCorrespondentBankDetailsDto>
                {
                    new() { Swift = "", Iban = "123" }
                }
            },
            1L);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor("UpdateForeignBankDetailsDto.CorrespondentBanks[0].Swift");
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenDataIsValid()
    {
        var command = new UpdateForeignBankDetailsCommand(
            new UpdateForeignBankDetailsDto
            {
                Swift = new string('A', ForeignBankDetailsConstants.Swift.MinLength),
                Iban = new string('1', ForeignBankDetailsConstants.Iban.MinLength),
                CorrespondentBanks = new List<UpdateCorrespondentBankDetailsDto>
                {
                    new()
                    {
                        Swift = new string('B', CorrespondentBankDetailsConstants.Swift.MinLength),
                        Iban = new string('1', CorrespondentBankDetailsConstants.Iban.MinLength)
                    }
                }
            },
            1L);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
