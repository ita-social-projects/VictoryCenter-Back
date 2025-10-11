using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Admin.Donate.ForeignBankDetails.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Donate.CorrespondentBankDetails;
using VictoryCenter.BLL.DTOs.Admin.Donate.ForeignBankDetails;
using VictoryCenter.BLL.Validators.Donate.ForeignBankDetails;

namespace VictoryCenter.UnitTests.ValidatorsTests.Donate;
public class CreateForeignBankDetailsCommandValidatorTests
{
    private readonly CreateForeignBankDetailsCommandValidator _validator;

    public CreateForeignBankDetailsCommandValidatorTests()
    {
        _validator = new CreateForeignBankDetailsCommandValidator();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_ShouldHaveError_WhenSwiftIsEmpty(string? swift)
    {
        var command = new CreateForeignBankDetailsCommand(
            new CreateForeignBankDetailsDto { Swift = swift, Iban = "12345678" });
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.CreateForeignBankDetailsDto.Swift)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(ForeignBankDetailsDto.Swift)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenSwiftIsTooShort()
    {
        var command = new CreateForeignBankDetailsCommand(
            new CreateForeignBankDetailsDto { Swift = "12", Iban = "12345678" });
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.CreateForeignBankDetailsDto.Swift)
            .WithErrorMessage(ErrorMessagesConstants
                .PropertyMustHaveAMinimumLengthOfNCharacters(nameof(ForeignBankDetailsDto.Swift), ForeignBankDetailsConstants.Swift.MinLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenIbanContainsNonDigits()
    {
        var command = new CreateForeignBankDetailsCommand(
            new CreateForeignBankDetailsDto { Swift = "VALIDSWIFT", Iban = new string('a', UahBankDetailsConstants.Iban.MinLength) });
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(c => c.CreateForeignBankDetailsDto.Iban)
            .WithErrorMessage(ForeignBankDetailsConstants.OnlyDigitsMessage);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenCorrespondentBankIsInvalid()
    {
        var command = new CreateForeignBankDetailsCommand(
            new CreateForeignBankDetailsDto
            {
                Swift = "VALIDSWIFT",
                Iban = "123456789",
                CorrespondentBanks = new List<CreateCorrespondentBankDetailsDto>
                {
                    new() { Swift = "", Iban = "123" }
                }
            });

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor("CreateForeignBankDetailsDto.CorrespondentBanks[0].Swift");
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenDataIsValid()
    {
        var command = new CreateForeignBankDetailsCommand(
            new CreateForeignBankDetailsDto
            {
                Swift = new string('A', ForeignBankDetailsConstants.Swift.MinLength),
                Iban = new string('1', ForeignBankDetailsConstants.Iban.MinLength),
                CorrespondentBanks = new List<CreateCorrespondentBankDetailsDto>
                {
                new()
                {
                    Swift = new string('B', CorrespondentBankDetailsConstants.Swift.MinLength),
                    Iban = new string('1', CorrespondentBankDetailsConstants.Iban.MinLength)
                }
                }
            });

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
