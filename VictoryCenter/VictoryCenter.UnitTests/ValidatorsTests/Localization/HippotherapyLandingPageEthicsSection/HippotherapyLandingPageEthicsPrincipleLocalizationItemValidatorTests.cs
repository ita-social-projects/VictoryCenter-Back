using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Constants.Localization;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageEthicsSection;
using VictoryCenter.BLL.Validators.Localization.HippotherapyLandingPageEthicsSection;

namespace VictoryCenter.UnitTests.ValidatorsTests.Localization.HippotherapyLandingPageEthicsSection;

public class HippotherapyLandingPageEthicsPrincipleLocalizationItemValidatorTests
{
    private readonly HippotherapyLandingPageEthicsPrincipleLocalizationItemValidator _validator = new();

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_ShouldHaveError_WhenPrincipleId_IsNotPositive(long principleId)
    {
        var model = new UpdateHippotherapyLandingPageEthicsPrincipleLocalizationItemDto
        {
            PrincipleId = principleId,
            Text = "Valid principle text",
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.PrincipleId)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(UpdateHippotherapyLandingPageEthicsPrincipleLocalizationItemDto.PrincipleId)));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_ShouldHaveError_WhenName_IsNullOrEmpty(string? name)
    {
        var model = new UpdateHippotherapyLandingPageEthicsPrincipleLocalizationItemDto
        {
            PrincipleId = 1,
            Text = name!,
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Text);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenName_VisibleTextIsTooShort()
    {
        var visibleText = new string('a', HippotherapyLandingPageEthicsSectionLocalizationConstants.PrincipleTextMinLength - 1);
        var model = new UpdateHippotherapyLandingPageEthicsPrincipleLocalizationItemDto
        {
            PrincipleId = 1,
            Text = $"<p>{visibleText}</p>",
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Text);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenName_VisibleTextExceedsMaxLength()
    {
        var visibleText = new string('a', HippotherapyLandingPageEthicsSectionLocalizationConstants.PrincipleTextMaxLength + 1);
        var model = new UpdateHippotherapyLandingPageEthicsPrincipleLocalizationItemDto
        {
            PrincipleId = 1,
            Text = $"<p>{visibleText}</p>",
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Text);
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenName_WithHtmlMarkupHasVisibleTextWithinMaxLength()
    {
        var visibleText = new string('a', HippotherapyLandingPageEthicsSectionLocalizationConstants.PrincipleTextMaxLength);
        var model = new UpdateHippotherapyLandingPageEthicsPrincipleLocalizationItemDto
        {
            PrincipleId = 1,
            Text = $"<p><strong>{visibleText}</strong></p>",
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Text);
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenModel_IsValid()
    {
        var model = new UpdateHippotherapyLandingPageEthicsPrincipleLocalizationItemDto
        {
            PrincipleId = 1,
            Text = "Therapeutic horseback riding for children and veterans.",
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
