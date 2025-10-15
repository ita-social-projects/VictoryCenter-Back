using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Admin.Donate.ForeignBankDetails.Update;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Donate.ForeignBankDetails;
using VictoryCenter.BLL.Validators.Donate.ForeignBankDetails;

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
    public void Validate_ShouldNotHaveError_WhenDataIsValid()
    {
        var command = new UpdateForeignBankDetailsCommand(
            new UpdateForeignBankDetailsDto
            {
                Swift = new string('A', ForeignBankDetailsConstants.Swift.MinLength),
                Iban = new string('1', ForeignBankDetailsConstants.Iban.MinLength),
            },
            1L);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
