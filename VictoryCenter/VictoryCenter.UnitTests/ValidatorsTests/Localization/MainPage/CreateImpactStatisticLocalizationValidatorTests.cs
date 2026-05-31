using FluentValidation.TestHelper;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.MainPage;
using VictoryCenter.BLL.Validators.Localization.MainPage;

namespace VictoryCenter.UnitTests.ValidatorsTests.Localization.MainPage;

public class CreateImpactStatisticLocalizationValidatorTests
{
    private readonly CreateImpactStatisticLocalizationValidator _validator = new();

    [Fact]
    public void Validate_ShouldNotHaveErrors_WhenDataIsValid()
    {
        var dto = new CreateImpactStatisticLocalizationDto
        {
            LanguageId = 1,
            Title = new string('a', MainPageConstants.ImpactStatistic.TitleConstraints.MinLength),
        };

        var result = _validator.TestValidate(dto);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_ShouldHaveError_WhenLanguageIdIsNotPositive(long languageId)
    {
        var dto = new CreateImpactStatisticLocalizationDto
        {
            LanguageId = languageId,
            Title = new string('a', MainPageConstants.ImpactStatistic.TitleConstraints.MinLength),
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.LanguageId)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustBePositive(
                nameof(CreateImpactStatisticLocalizationDto.LanguageId)));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_ShouldHaveError_WhenTitleIsEmpty(string? title)
    {
        var dto = new CreateImpactStatisticLocalizationDto { LanguageId = 1, Title = title };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(CreateImpactStatisticLocalizationDto.Title)));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenTitleIsTooShort()
    {
        var dto = new CreateImpactStatisticLocalizationDto
        {
            LanguageId = 1,
            Title = new string('a', MainPageConstants.ImpactStatistic.TitleConstraints.MinLength - 1),
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(CreateImpactStatisticLocalizationDto.Title),
                MainPageConstants.ImpactStatistic.TitleConstraints.MinLength));
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenTitleIsTooLong()
    {
        var dto = new CreateImpactStatisticLocalizationDto
        {
            LanguageId = 1,
            Title = new string('a', MainPageConstants.ImpactStatistic.TitleConstraints.MaxLength + 1),
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(CreateImpactStatisticLocalizationDto.Title),
                MainPageConstants.ImpactStatistic.TitleConstraints.MaxLength));
    }
}
