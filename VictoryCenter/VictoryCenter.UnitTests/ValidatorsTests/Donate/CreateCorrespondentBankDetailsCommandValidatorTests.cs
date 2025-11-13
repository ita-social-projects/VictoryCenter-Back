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
        // Arrange
        var dto = GetValidDto() with { Swift = swift! };
        var command = new CreateCorrespondentBankDetailsCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.CreateCorrespondentBankDetailsDto.Swift)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CorrespondentBankDetailsDto.Swift)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenSwiftIsTooShort()
    {
        // Arrange
        var dto = GetValidDto() with { Swift = new string('A', CorrespondentBankDetailsConstants.Swift.MinLength - 1) };
        var command = new CreateCorrespondentBankDetailsCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.CreateCorrespondentBankDetailsDto.Swift)
            .WithErrorMessage(ErrorMessagesConstants
                .PropertyMustHaveAMinimumLengthOfNCharacters(
                    nameof(CorrespondentBankDetailsConstants.Swift),
                    CorrespondentBankDetailsConstants.Swift.MinLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenSwiftIsTooLong()
    {
        // Arrange
        var dto = GetValidDto() with { Swift = new string('A', CorrespondentBankDetailsConstants.Swift.MaxLength + 1) };
        var command = new CreateCorrespondentBankDetailsCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.CreateCorrespondentBankDetailsDto.Swift)
            .WithErrorMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(
                    nameof(CorrespondentBankDetailsConstants.Swift),
                    CorrespondentBankDetailsConstants.Swift.MaxLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenIbanIsTooLong()
    {
        // Arrange
        var dto = GetValidDto() with { Iban = new string('A', CorrespondentBankDetailsConstants.Iban.MaxLength + 1) };
        var command = new CreateCorrespondentBankDetailsCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.CreateCorrespondentBankDetailsDto.Iban)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(CorrespondentBankDetailsConstants.Iban),
                CorrespondentBankDetailsConstants.Iban.MaxLength));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_WhenNameIsEmpty(string? name)
    {
        // Arrange
        var dto = GetValidDto() with { Name = name! };
        var command = new CreateCorrespondentBankDetailsCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.CreateCorrespondentBankDetailsDto.Name)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CorrespondentBankDetailsDto.Name)));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_WhenAccountIsEmpty(string? account)
    {
        // Arrange
        var dto = GetValidDto() with { Account = account! };
        var command = new CreateCorrespondentBankDetailsCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.CreateCorrespondentBankDetailsDto.Account)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CorrespondentBankDetailsDto.Account)));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_ShouldNotHaveError_WhenIbanIsNullOrEmpty(string? iban)
    {
        // Arrange
        var dto = GetValidDto() with { Iban = iban };
        var command = new CreateCorrespondentBankDetailsCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(c => c.CreateCorrespondentBankDetailsDto.Iban);
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenDataIsValid()
    {
        // Arrange
        var dto = GetValidDto();
        var command = new CreateCorrespondentBankDetailsCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenNameIsTooLong()
    {
        // Arrange
        var dto = GetValidDto() with { Name = new string('A', CorrespondentBankDetailsConstants.NameMaxLength + 1) };
        var command = new CreateCorrespondentBankDetailsCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.CreateCorrespondentBankDetailsDto.Name)
            .WithErrorMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(
                    nameof(CorrespondentBankDetailsDto.Name),
                    CorrespondentBankDetailsConstants.NameMaxLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenAccountIsTooLong()
    {
        // Arrange
        var dto = GetValidDto() with { Account = new string('A', CorrespondentBankDetailsConstants.AccountMaxLength + 1) };
        var command = new CreateCorrespondentBankDetailsCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.CreateCorrespondentBankDetailsDto.Account)
            .WithErrorMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(
                    nameof(CorrespondentBankDetailsDto.Account),
                    CorrespondentBankDetailsConstants.AccountMaxLength));
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenDataIsValidWithoutIban()
    {
        // Arrange
        var dto = GetValidDto() with { Iban = null };
        var command = new CreateCorrespondentBankDetailsCommand(dto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    private static CreateCorrespondentBankDetailsDto GetValidDto() => new()
    {
        Swift = "VALIDSWIFTT",
        Iban = "UA123456789012345678901234567",
        Name = "Valid Name",
        Account = "Valid Account",
        ForeignBankDetailsId = 1
    };
}
