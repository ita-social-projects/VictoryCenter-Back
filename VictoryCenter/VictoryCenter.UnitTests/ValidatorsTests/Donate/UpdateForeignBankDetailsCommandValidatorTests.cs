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

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_WhenIbanIsEmpty(string? iban)
    {
        // Arrange
        var dto = GetValidDto() with { Iban = iban! };
        var command = new UpdateForeignBankDetailsCommand(dto, 1L);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.UpdateForeignBankDetailsDto.Iban)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(ForeignBankDetailsDto.Iban)));
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
        Iban = "UA1234567890123456789",
        Name = "Valid Name",
        Receiver = "Valid Receiver",
        Address = "Valid Address"
    };
}
