using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Partners;
using VictoryCenter.BLL.Validators.Partners;

namespace VictoryCenter.UnitTests.ValidatorsTests.Partners;

public class CreatePartnerImageValidatorTests
{
    private readonly CreatePartnerImageValidator _validator;

    public CreatePartnerImageValidatorTests()
    {
        _validator = new CreatePartnerImageValidator();
    }

    [Fact]
    public void Validate_Base64IsNull_ShouldHaveError()
    {
        // Arrange
        var model = new CreatePartnerImageDto { Base64 = null!, MimeType = "image/png" };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Base64);
    }

    [Fact]
    public void Validate_Base64IsInvalidFormat_ShouldHaveError()
    {
        // Arrange
        // Рядок містить невалідні для Base64 символи
        var model = new CreatePartnerImageDto { Base64 = "this-is-not-base64!", MimeType = "image/png" };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Base64)
              .WithErrorMessage(ImageConstants.Base64ValidationError);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("application/pdf")]
    [InlineData("image/gif")]
    public void Validate_MimeTypeIsInvalid_ShouldHaveError(string invalidMimeType)
    {
        // Arrange
        var model = new CreatePartnerImageDto { Base64 = "SGVsbG8=", MimeType = invalidMimeType };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MimeType);
    }

    [Fact]
    public void Validate_ValidDto_ShouldNotHaveErrors()
    {
        // Arrange
        var model = new CreatePartnerImageDto
        {
            // "Hello" в Base64
            Base64 = "SGVsbG8=",
            MimeType = PartnerConstants.AllowedImageMimeTypes[0] // Беремо перший дозволений тип
        };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
