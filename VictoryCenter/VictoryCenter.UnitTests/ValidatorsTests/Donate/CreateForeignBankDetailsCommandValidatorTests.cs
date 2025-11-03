using FluentValidation.TestHelper;
using VictoryCenter.BLL.Commands.Admin.Donate.ForeignBankDetails.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Donate.ForeignBankDetails;
using VictoryCenter.BLL.Validators.Donate.ForeignBankDetails;
using VictoryCenter.DAL.Enums;

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
    public void Validate_ShouldNotHaveError_WhenDataIsValid()
    {
        var command = new CreateForeignBankDetailsCommand(
            new CreateForeignBankDetailsDto
            {
                Swift = new string('A', ForeignBankDetailsConstants.Swift.MinLength),
                Iban = new string('1', ForeignBankDetailsConstants.Iban.MinLength),
                Currency = BankCurrency.Usd,
            });

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
