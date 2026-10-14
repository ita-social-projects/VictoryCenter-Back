using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Constants.Localization;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageScientificReferencesSection;
using VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageScientificReferencesSection;

namespace VictoryCenter.UnitTests.ValidatorsTests.Localization.HippotherapyLandingPageScientificReferencesSection;

public class HippotherapyLandingPageScientificReferenceLocalizationItemValidatorTests
{
    private readonly HippotherapyLandingPageScientificReferenceLocalizationItemValidator _validator = new();

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_ShouldHaveError_WhenReferenceId_IsNotPositive(long referenceId)
    {
        var model = new UpdateHippotherapyLandingPageScientificReferenceLocalizationItemDto
        {
            ReferenceId = referenceId,
            Name = "Valid reference name",
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.ReferenceId)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(UpdateHippotherapyLandingPageScientificReferenceLocalizationItemDto.ReferenceId)));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_WhenName_IsNullOrEmpty(string? name)
    {
        var model = new UpdateHippotherapyLandingPageScientificReferenceLocalizationItemDto
        {
            ReferenceId = 1,
            Name = name!,
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenName_VisibleTextIsTooShort()
    {
        var visibleText = new string('a', HippotherapyLandingPageScientificReferencesSectionLocalizationConstants.ReferenceNameMinLength - 1);
        var model = new UpdateHippotherapyLandingPageScientificReferenceLocalizationItemDto
        {
            ReferenceId = 1,
            Name = $"<p>{visibleText}</p>",
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenName_VisibleTextExceedsMaxLength()
    {
        var visibleText = new string('a', HippotherapyLandingPageScientificReferencesSectionLocalizationConstants.ReferenceNameMaxLength + 1);
        var model = new UpdateHippotherapyLandingPageScientificReferenceLocalizationItemDto
        {
            ReferenceId = 1,
            Name = $"<p>{visibleText}</p>",
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenName_WithHtmlMarkupHasVisibleTextWithinMaxLength()
    {
        var visibleText = new string('a', HippotherapyLandingPageScientificReferencesSectionLocalizationConstants.ReferenceNameMaxLength);
        var model = new UpdateHippotherapyLandingPageScientificReferenceLocalizationItemDto
        {
            ReferenceId = 1,
            Name = $"<p><strong>{visibleText}</strong></p>",
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenModel_IsValid()
    {
        var model = new UpdateHippotherapyLandingPageScientificReferenceLocalizationItemDto
        {
            ReferenceId = 1,
            Name = "Therapeutic horseback riding for children and veterans.",
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
