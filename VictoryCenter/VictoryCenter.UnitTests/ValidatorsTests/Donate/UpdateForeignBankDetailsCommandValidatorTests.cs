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
    [InlineData(" ")]
    public void Validate_ShouldHaveError_WhenSwiftIsEmpty(string? swift)
    {
        // Arrange
        var dto = GetValidDto() with { Swift = swift! };
        var command = new UpdateForeignBankDetailsCommand(dto, 1L);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.UpdateForeignBankDetailsDto.Swift)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(ForeignBankDetailsDto.Swift)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenSwiftIsTooShort()
    {
        // Arrange
        var dto = GetValidDto() with { Swift = new string('A', ForeignBankDetailsConstants.Swift.MinLength - 1) };
        var command = new UpdateForeignBankDetailsCommand(dto, 1L);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.UpdateForeignBankDetailsDto.Swift)
            .WithErrorMessage(ErrorMessagesConstants
                .PropertyMustHaveAMinimumLengthOfNCharacters(
                    nameof(ForeignBankDetailsDto.Swift),
                    ForeignBankDetailsConstants.Swift.MinLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenSwiftIsTooLong()
    {
        // Arrange
        var dto = GetValidDto() with { Swift = new string('A', ForeignBankDetailsConstants.Swift.MaxLength + 1) };
        var command = new UpdateForeignBankDetailsCommand(dto, 1L);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.UpdateForeignBankDetailsDto.Swift)
            .WithErrorMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(
                    nameof(ForeignBankDetailsDto.Swift),
                    ForeignBankDetailsConstants.Swift.MaxLength));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_WhenIbanIsEmpty(string? iban)
    {
        // Arrange
        var dto = GetValidDto() with { UkrainianIban = iban! };
        var command = new UpdateForeignBankDetailsCommand(dto, 1L);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.UpdateForeignBankDetailsDto.UkrainianIban)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(ForeignBankDetailsDto.UkrainianIban)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenIbanIsTooShort()
    {
        // Arrange
        var dto = GetValidDto() with { UkrainianIban = "UA" + new string('1', ForeignBankDetailsConstants.UkrainianIban.MinLength - 3) };
        var command = new UpdateForeignBankDetailsCommand(dto, 1L);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.UpdateForeignBankDetailsDto.UkrainianIban)
            .WithErrorMessage(ErrorMessagesConstants
                .PropertyMustHaveAMinimumLengthOfNCharacters(
                    nameof(ForeignBankDetailsDto.UkrainianIban),
                    ForeignBankDetailsConstants.UkrainianIban.MinLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenIbanIsTooLong()
    {
        // Arrange
        var dto = GetValidDto() with { UkrainianIban = "UA" + new string('1', ForeignBankDetailsConstants.UkrainianIban.MaxLength + 1) };
        var command = new UpdateForeignBankDetailsCommand(dto, 1L);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.UpdateForeignBankDetailsDto.UkrainianIban)
            .WithErrorMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(
                    nameof(ForeignBankDetailsDto.UkrainianIban),
                    ForeignBankDetailsConstants.UkrainianIban.MaxLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenIbanDoesNotStartWithUa()
    {
        // Arrange
        var dto = GetValidDto() with { UkrainianIban = "XX123456789012345678901234567" };
        var command = new UpdateForeignBankDetailsCommand(dto, 1L);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.UpdateForeignBankDetailsDto.UkrainianIban)
            .WithErrorMessage(ForeignBankDetailsConstants.UkrainianIbanMustStartWithUaFollowedByDigits);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_WhenNameIsEmpty(string? name)
    {
        // Arrange
        var dto = GetValidDto() with { Name = name! };
        var command = new UpdateForeignBankDetailsCommand(dto, 1L);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.UpdateForeignBankDetailsDto.Name)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(ForeignBankDetailsDto.Name)));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_WhenReceiverIsEmpty(string? receiver)
    {
        // Arrange
        var dto = GetValidDto() with { Receiver = receiver! };
        var command = new UpdateForeignBankDetailsCommand(dto, 1L);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.UpdateForeignBankDetailsDto.Receiver)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(ForeignBankDetailsDto.Receiver)));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_WhenAddressIsEmpty(string? address)
    {
        // Arrange
        var dto = GetValidDto() with { Address = address! };
        var command = new UpdateForeignBankDetailsCommand(dto, 1L);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.UpdateForeignBankDetailsDto.Address)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(ForeignBankDetailsDto.Address)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenNameIsTooLong()
    {
        // Arrange
        var dto = GetValidDto() with { Name = new string('A', ForeignBankDetailsConstants.NameMaxLength + 1) };
        var command = new UpdateForeignBankDetailsCommand(dto, 1L);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.UpdateForeignBankDetailsDto.Name)
            .WithErrorMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(
                    nameof(ForeignBankDetailsDto.Name),
                    ForeignBankDetailsConstants.NameMaxLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenReceiverIsTooLong()
    {
        // Arrange
        var dto = GetValidDto() with { Receiver = new string('A', ForeignBankDetailsConstants.ReceiverMaxLength + 1) };
        var command = new UpdateForeignBankDetailsCommand(dto, 1L);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.UpdateForeignBankDetailsDto.Receiver)
            .WithErrorMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(
                    nameof(ForeignBankDetailsDto.Receiver),
                    ForeignBankDetailsConstants.ReceiverMaxLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenAddressIsTooLong()
    {
        // Arrange
        var dto = GetValidDto() with { Address = new string('A', ForeignBankDetailsConstants.AddressMaxLength + 1) };
        var command = new UpdateForeignBankDetailsCommand(dto, 1L);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.UpdateForeignBankDetailsDto.Address)
            .WithErrorMessage(ErrorMessagesConstants
                .PropertyMustHaveAMaximumLengthOfNCharacters(
                    nameof(ForeignBankDetailsDto.Address),
                    ForeignBankDetailsConstants.AddressMaxLength));
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenDataIsValid()
    {
        // Arrange
        var dto = GetValidDto();
        var command = new UpdateForeignBankDetailsCommand(dto, 1L);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    private static UpdateForeignBankDetailsDto GetValidDto() => new()
    {
        Swift = "VALIDSWIFT",
        UkrainianIban = "UA123456789012345678901234567",
        Name = "Valid Name",
        Receiver = "Valid Receiver",
        Address = "Valid Address"
    };
}
