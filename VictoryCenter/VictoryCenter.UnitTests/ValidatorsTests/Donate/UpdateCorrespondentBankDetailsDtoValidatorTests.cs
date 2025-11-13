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
        // Arrange
        var dto = GetValidDto() with { Swift = swift! };
        var command = new UpdateCorrespondentBankDetailsCommand(dto, 1L);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.UpdateCorrespondentBankDetailsDto.Swift)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CorrespondentBankDetailsDto.Swift)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenSwiftIsTooShort()
    {
        // Arrange
        var dto = GetValidDto() with { Swift = new string('A', CorrespondentBankDetailsConstants.Swift.MinLength - 1) };
        var command = new UpdateCorrespondentBankDetailsCommand(dto, 1L);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.UpdateCorrespondentBankDetailsDto.Swift)
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
        var command = new UpdateCorrespondentBankDetailsCommand(dto, 1L);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.UpdateCorrespondentBankDetailsDto.Swift)
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
        var command = new UpdateCorrespondentBankDetailsCommand(dto, 1L);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.UpdateCorrespondentBankDetailsDto.Iban)
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
        var command = new UpdateCorrespondentBankDetailsCommand(dto, 1L);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.UpdateCorrespondentBankDetailsDto.Name)
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
        var command = new UpdateCorrespondentBankDetailsCommand(dto, 1L);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.UpdateCorrespondentBankDetailsDto.Account)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CorrespondentBankDetailsDto.Account)));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_ShouldNotHaveError_WhenIbanIsNullOrEmpty(string? iban)
    {
        // Arrange
        var dto = GetValidDto() with { Iban = iban };
        var command = new UpdateCorrespondentBankDetailsCommand(dto, 1L);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(c => c.UpdateCorrespondentBankDetailsDto.Iban);
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenDataIsValid()
    {
        // Arrange
        var dto = GetValidDto();
        var command = new UpdateCorrespondentBankDetailsCommand(dto, 1L);

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
        var command = new UpdateCorrespondentBankDetailsCommand(dto, 1L);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.UpdateCorrespondentBankDetailsDto.Name)
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
        var command = new UpdateCorrespondentBankDetailsCommand(dto, 1L);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.UpdateCorrespondentBankDetailsDto.Account)
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
        var command = new UpdateCorrespondentBankDetailsCommand(dto, 1L);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    private static UpdateCorrespondentBankDetailsDto GetValidDto() => new()
    {
        Swift = "VALIDSWIFTT",
        Iban = "UA123456789012345678901234567",
        Name = "Valid Name",
        Account = "Valid Account",
        ForeignBankDetailsId = 1
    };
}
